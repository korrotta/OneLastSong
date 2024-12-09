using NAudio.Wave;
using NAudio.Dsp;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Utils;
using OneLastSong.Cores.AudioSystem;
using Microsoft.UI.Dispatching;
using OneLastSong.DAOs;
using System.Security.Policy;
using OneLastSong.ViewModels;
using OneLastSong.Cores.Equalizer;
using Windows.Devices.AllJoyn;

namespace OneLastSong.Services
{
    public class ListeningService : IDisposable, INotifySubsytemStateChanged, IAuthChangeNotify
    {
        public PlayModeData PlayModeData { get; private set; } = new PlayModeData();
        private Task _workerTask;
        private bool _isPlaying = false;
        private bool _shouldRun = true;
        private bool _shouldContinuePlayingCurrentAudio = true;
        private MediaFoundationReader mf = null;
        private WasapiOut wo = null;
        private DispatcherQueue _eventHandler;
        private ListeningSessionDAO _listeningSessionDAO;
        private UserDAO _userDAO;
        private AudioDAO _audioDAO;
        private PlayHistoryDAO _playHistoryDAO;
        private AuthService _authService;
        private List<IAudioStateChanged> _audioStateChangeNotifiers = new List<IAudioStateChanged>();
        private List<INotifyPlayQueueChanged> _playQueueChangeNotifiers = new List<INotifyPlayQueueChanged>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private Equalizer _equalizer;
        private EqualizerViewModel _equalizerViewModel;

        public EqualizerViewModel EqualizerViewModel
        {
            get => _equalizerViewModel;
        }

        public ListeningService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
            _workerTask = Task.Run(WorkerLoop, _cancellationTokenSource.Token);
            _equalizerViewModel = new EqualizerViewModel();
            EqualizerViewModel.EqualizerBandsChanged += (sender, e) => ApplyEQ();
        }

        private async Task WorkerLoop()
        {
            while (_shouldRun)
            {
                Audio currentAudio = await PlayModeData.GetCurrentAudioSafe();

                if (currentAudio != null)
                {
                    try
                    {
                        NotifyAudioChanged(currentAudio);
                        NotifyPlayQueueChanged();
                        NotifyProgressChanged(0);
                        await PlayAudioUrlAsync(currentAudio.Url, currentAudio.Duration);
                        if (currentAudio != null)
                        {
                            SavePlayHistory(currentAudio);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtils.Error(ex.Message);
                        _eventHandler.TryEnqueue(() =>
                        {
                            SnackbarUtils.ShowSnackbar("Failed to play audio", SnackbarType.Error);
                        });
                    }
                }

                if (PlayModeData.ListeningSession == null)
                {
                    currentAudio = await PlayModeData.NextAudioAsync();
                }

                await Task.Delay(1000);
            }
        }

        private void SavePlayHistory(Audio currentAudio)
        {
            string token = _userDAO.SessionToken;
            if (token == null)
            {
                return;
            }

            _playHistoryDAO.AddUserPlayHistory(token, currentAudio.AudioId);
        }

        private async void GenNewQueue(int v)
        {
            if(_audioDAO == null)
            {
                return;
            }

            List<Audio> audios = new List<Audio>();
            // get random audios
            for (int i = 0; i < v; i++)
            {
                Audio newAudio = await _audioDAO.GetRandom();
                audios.Add(newAudio);
            }
            await PlayModeData.AddNewAudiosToQueue(audios);
            NotifyPlayQueueChanged();
        }

        public async void PlayAudio(Audio audio)
        {
            await PlayModeData.PlayMashUpAsync(audio);
            _shouldContinuePlayingCurrentAudio = false;
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    NotifyPlayStateChanged(value);
                }
            }
        }

        public void NotifyProgressChanged(int progress)
        {
            _eventHandler.TryEnqueue(() =>
            {
                foreach (var notifier in _audioStateChangeNotifiers)
                {
                    notifier.OnAudioProgressChanged(progress);
                }
            });
        }

        private void NotifyAudioChanged(Audio newAudio)
        {
            _eventHandler.TryEnqueue(() =>
            {
                foreach (var notifier in _audioStateChangeNotifiers)
                {
                    notifier.OnAudioChanged(newAudio);
                }
            });
        }

        public void NotifyPlayStateChanged(bool isPlaying)
        {
            _eventHandler.TryEnqueue(() =>
            {
                foreach (var notifier in _audioStateChangeNotifiers)
                {
                    notifier.OnAudioPlayStateChanged(isPlaying);
                }
            });
        }

        public void NotifyPlayQueueChanged()
        {
            _eventHandler.TryEnqueue(async () =>
            {
                foreach (var notifier in _playQueueChangeNotifiers)
                {
                    List<Audio> currentQueue = await PlayModeData.GetCurrentQueueSafe();
                    notifier.OnPlayQueueChanged(currentQueue);
                }
            });
        }

        public void RegisterAudioStateChangeListeners(IAudioStateChanged audioStateChangeNotifier)
        {
            _eventHandler.TryEnqueue(() =>
            {
                if (!_audioStateChangeNotifiers.Contains(audioStateChangeNotifier))
                {
                    _audioStateChangeNotifiers.Add(audioStateChangeNotifier);
                }

                if (mf != null)
                {
                    audioStateChangeNotifier.OnAudioProgressChanged((int)mf.CurrentTime.TotalSeconds);
                }
                audioStateChangeNotifier.OnAudioChanged(PlayModeData.CurrentAudio);
                audioStateChangeNotifier.OnAudioPlayStateChanged(IsPlaying);
            });
        }

        public void UnregisterAudioStateChangeListeners(IAudioStateChanged audioStateChangeNotifier)
        {
            _eventHandler.TryEnqueue(() =>
            {
                _audioStateChangeNotifiers.Remove(audioStateChangeNotifier);
            });
        }

        public void RegisterPlayQueueChangeListeners(INotifyPlayQueueChanged playQueueChangeNotifier)
        {
            _eventHandler.TryEnqueue(() =>
            {
                if (!_playQueueChangeNotifiers.Contains(playQueueChangeNotifier))
                {
                    _playQueueChangeNotifiers.Add(playQueueChangeNotifier);
                }
            });
        }

        public void UnregisterPlayQueueChangeListeners(INotifyPlayQueueChanged playQueueChangeNotifier)
        {
            _eventHandler.TryEnqueue(() =>
            {
                _playQueueChangeNotifiers.Remove(playQueueChangeNotifier);
            });
        }

        public void Dispose()
        {
            _shouldRun = false;
            _cancellationTokenSource.Cancel();
            _workerTask?.Dispose();
            _authService?.UnregisterAuthChangeNotify(this);
            mf?.Dispose();
            wo?.Dispose();
            _cancellationTokenSource.Dispose();
        }

        private async Task PlayAudioUrlAsync(string url, int duration)
        {
            mf = new MediaFoundationReader(url);
            _equalizer = new Equalizer(mf.ToSampleProvider(), _equalizerViewModel.Bands.Select(b => new EqualizerBand { Frequency = b.Frequency, Gain = (float)b.Gain }).ToArray());
            wo = new WasapiOut();
            wo.Init(_equalizer);

            if (PlayModeData.ListeningSession != null)
            {
                IsPlaying = false;
                NotifyProgressChanged(PlayModeData.ListeningSession.Progress);
                _shouldContinuePlayingCurrentAudio = true;
                mf.CurrentTime = TimeSpan.FromSeconds(PlayModeData.ListeningSession.Progress);
                wo.Pause();
                PlayModeData.ListeningSession = null;
            }
            else
            {
                wo.Play();
                _shouldContinuePlayingCurrentAudio = true;
                IsPlaying = true;
            }

            while (_shouldContinuePlayingCurrentAudio)
            {
                if (IsPlaying)
                {
                    NotifyProgressChanged((int)mf.CurrentTime.TotalSeconds);
                    await SaveListeningProgressAsync();
                }
                if (mf.CurrentTime >= TimeSpan.FromSeconds(duration))
                {
                    PlayNext();
                }
                await Task.Delay(1000);
            }
            IsPlaying = false;
            wo.Stop();
        }

        private async Task SaveListeningProgressAsync()
        {
            if (_userDAO.SessionToken == null)
            {
                return;
            }

            try
            {
                await _listeningSessionDAO.SaveListeningSession(_userDAO.SessionToken, new ListeningSession
                {
                    AudioId = PlayModeData.CurrentAudio.AudioId,
                    Progress = (int)mf.CurrentTime.TotalSeconds
                });
            }
            catch (Exception ex)
            {
                LogUtils.Error(ex.Message);
            }
        }

        public static ListeningService Get()
        {
            return (ListeningService)((App)Application.Current).Services.GetService(typeof(ListeningService));
        }

        public void Seek(int seconds)
        {
            if (mf != null)
            {
                mf.CurrentTime = TimeSpan.FromSeconds(seconds);
                NotifyProgressChanged(seconds);
            }
        }

        public void ChangePlayState()
        {
            if (wo == null)
            {
                SnackbarUtils.ShowSnackbar("No audio is playing", SnackbarType.Warning);
                return;
            }

            if (_isPlaying)
            {
                wo.Pause();
            }
            else
            {
                wo.Play();
            }
            IsPlaying = !IsPlaying;
        }

        public void PlayNext()
        {
            _shouldContinuePlayingCurrentAudio = false;
        }

        public void RestartPlay()
        {
            if (mf != null)
            {
                mf.CurrentTime = TimeSpan.FromSeconds(0);
                NotifyProgressChanged(0);
            }
        }

        public async Task<bool> OnSubsystemInitialized()
        {
            _listeningSessionDAO = ListeningSessionDAO.Get();
            _userDAO = UserDAO.Get();
            _audioDAO = AudioDAO.Get();
            _authService = AuthService.Get();
            _playHistoryDAO = PlayHistoryDAO.Get();

            _listeningSessionDAO.SetListeningService(this);
            _authService.RegisterAuthChangeNotify(this);

            GenNewQueue(5);

            NotifyAudioChanged(PlayModeData.CurrentAudio);
            NotifyPlayStateChanged(IsPlaying);
            NotifyProgressChanged(0);

            await Task.CompletedTask;
            return true;
        }

        internal async void AddPlaylistToQueue(Playlist playlist)
        {
            await PlayModeData.AddPlaylistToQueueAsync(playlist);
        }

        internal async void PlayPlaylist(Playlist playlist)
        {
            await PlayModeData.PlayPlaylistAsync(playlist);
            PlayNext();
        }

        public async void OnUserChange(User user, string sesstionken)
        {
            if (user != null)
            {
                string token = sesstionken;
                var listeningSession = await _listeningSessionDAO.GetListeningSession(token);
                
                if(listeningSession == null)
                {
                    return;
                }

                var audio = await _audioDAO.GetAudioById(listeningSession.AudioId);

                PlayModeData.RetrievePlayingSession(audio, listeningSession.Progress);
                _shouldContinuePlayingCurrentAudio = false;
            }
        }

        internal void PlayAudioList(List<Audio> audioList)
        {
            PlayModeData.PlayAudioList(audioList);
            _shouldContinuePlayingCurrentAudio = false;
        }

        public void ApplyEQ()
        {
            if (_equalizer != null)
            {
                _equalizer.Update(_equalizerViewModel.Bands.Select(b => new EqualizerBand { Frequency = b.Frequency, Gain = (float)b.Gain }).ToArray());
            }
        }

        internal async void RemoveAudioFromQueue(int index)
        {
            await PlayModeData.RemoveAudioFromQueue(index);
            NotifyPlayQueueChanged();
        }

        internal async void PlayAudioInQueue(int index)
        {
            await PlayModeData.PlayAudioInQueue(index);
            _shouldContinuePlayingCurrentAudio = false;
        }

        /*
         * Set the next audio to play in the queue
         */
        internal async void PlayNextAudio(Audio audio)
        {
            await PlayModeData.SetFirstInQueue(audio);
            NotifyPlayQueueChanged();
        }

        /*
         * Add audio to the end of the queue
         */
        internal async void AddToQueue(Audio audio)
        {
            await PlayModeData.AddToQueue(audio);
            NotifyPlayQueueChanged();
        }
    }
}
