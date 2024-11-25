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
        private AuthService _authService;
        private List<IAudioStateChanged> _audioStateChangeNotifiers = new List<IAudioStateChanged>();
        private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        public ListeningService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
            _workerTask = Task.Run(WorkerLoop, _cancellationTokenSource.Token);
        }

        private async Task WorkerLoop()
        {
            while (_shouldRun)
            {
                var currentAudio = PlayModeData.CurrentAudio;

                if (currentAudio != null)
                {
                    NotifyAudioChanged(currentAudio);
                    await PlayAudioUrlAsync(currentAudio.Url, currentAudio.Duration);
                }

                if (PlayModeData.ListeningSession == null)
                {
                    currentAudio = await PlayModeData.NextAudioAsync();
                }

                await Task.Delay(1000);
            }
        }

        public async void PlayAudio(Audio audio)
        {
            await PlayModeData.PlayMashUpAsync(audio);
            _shouldContinuePlayingCurrentAudio = false;
            NotifyAudioChanged(audio);
            NotifyProgressChanged(0);
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
            foreach (var notifier in _audioStateChangeNotifiers)
            {
                _eventHandler.TryEnqueue(() =>
                {
                    notifier.OnAudioProgressChanged(progress);
                });
            }
        }

        private void NotifyAudioChanged(Audio newAudio)
        {
            foreach (var notifier in _audioStateChangeNotifiers)
            {
                _eventHandler.TryEnqueue(() =>
                {
                    notifier.OnAudioChanged(newAudio);
                });
            }
        }

        public void NotifyPlayStateChanged(bool isPlaying)
        {
            foreach (var notifier in _audioStateChangeNotifiers)
            {
                _eventHandler.TryEnqueue(() =>
                {
                    notifier.OnAudioPlayStateChanged(isPlaying);
                });
            }
        }

        public void RegisterAudioStateChangeListeners(IAudioStateChanged audioStateChangeNotifier)
        {
            if (!_audioStateChangeNotifiers.Contains(audioStateChangeNotifier))
            {
                _audioStateChangeNotifiers.Add(audioStateChangeNotifier);
            }
        }

        public void UnregisterAudioStateChangeListeners(IAudioStateChanged audioStateChangeNotifier)
        {
            _audioStateChangeNotifiers.Remove(audioStateChangeNotifier);
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
            wo = new WasapiOut();
            wo.Init(mf);

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
                NotifyProgressChanged((int)mf.CurrentTime.TotalSeconds);
                await SaveListeningProgressAsync();
                if(mf.CurrentTime >= TimeSpan.FromSeconds(duration))
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

            _listeningSessionDAO.SetListeningService(this);
            _authService.RegisterAuthChangeNotify(this);

            NotifyAudioChanged(PlayModeData.CurrentAudio);
            NotifyPlayStateChanged(IsPlaying);
            NotifyProgressChanged(0);

            await Task.CompletedTask;
            return true;
        }

        internal void AddPlaylistToQueue(Playlist playlist)
        {
            PlayModeData.AddPlaylistToQueueAsync(playlist);
        }

        internal void PlayPlaylist(Playlist playlist)
        {
            PlayModeData.PlayPlaylistAsync(playlist);
            PlayNext();
        }

        public async void OnUserChange(User user)
        {
            if (user != null)
            {
                string token = _userDAO.SessionToken;
                var listeningSession = await _listeningSessionDAO.GetListeningSession(token);
                var audio = await _audioDAO.GetAudioById(listeningSession.AudioId);

                PlayModeData.RetrievePlayingSession(audio, listeningSession.Progress);
                _shouldContinuePlayingCurrentAudio = false;
            }
        }
    }
}
