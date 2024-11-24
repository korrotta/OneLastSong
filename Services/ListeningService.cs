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

namespace OneLastSong.Services
{
    public class ListeningService : IDisposable, INotifySubsytemStateChanged
    {
        public PlayModeData PlayModeData { get; private set; } = new PlayModeData();
        // worker thread
        private Task _workerTask;
        private bool _isPlaying = false;
        private bool _shouldRun = true;
        private bool _shouldContinuePlayingCurrentAudio = true;
        private MediaFoundationReader mf = null;
        private WasapiOut wo = null;
        private DispatcherQueue _eventHandler;

        private List<IAudioStateChanged> _audioStateChangeNotifiers = new List<IAudioStateChanged>();

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
            _audioStateChangeNotifiers.Add(audioStateChangeNotifier);
        }

        public void UnregisterAudioStateChangeListeners(IAudioStateChanged audioStateChangeNotifier)
        {
            _audioStateChangeNotifiers.Remove(audioStateChangeNotifier);
        }

        public ListeningService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;

            _workerTask = new Task(async () =>
            {
                while (_shouldRun)
                {
                    var oldAudio = PlayModeData.CurrentAudio;
                    var currentAudio = await PlayModeData.NextAudioAsync();
                    if (currentAudio != null)
                    {
                        NotifyAudioChanged(currentAudio);
                        // play audio
                        PlayAudioUrl(currentAudio.Url);
                    }
                    Thread.Sleep(1000);
                }
            });
            _workerTask.Start();
        }

        public void Dispose()
        {
            _shouldRun = false;
            _workerTask.Dispose();
        }

        private void PlayAudioUrl(string url)
        {
            // play audio from url
            mf = new MediaFoundationReader(url);
            wo = new WasapiOut();
            {
                wo.Init(mf);
                wo.Play();
                _shouldContinuePlayingCurrentAudio = true;
                IsPlaying = true;
                while (wo.PlaybackState != PlaybackState.Stopped && _shouldContinuePlayingCurrentAudio)
                {
                    NotifyProgressChanged((int)mf.CurrentTime.TotalSeconds);
                    Thread.Sleep(1000);
                }
                IsPlaying = false;
                wo.Stop();
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
            if(wo == null)
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
            // #Todo: Retrieve listening history
            await Task.CompletedTask;
            return true;
        }
    }
}
