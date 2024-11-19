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

namespace OneLastSong.Services
{
    public class ListeningService : IDisposable
    {
        private List<Audio> _playingQueue = new List<Audio>();
        private Audio _currentAudio = null;
        // worker thread
        private Task _workerTask;
        private bool _isPlaying = false;
        private bool _shouldRun = true;
        private bool _shouldContinuePlayingCurrentAudio = true;
        private MediaFoundationReader mf = null;
        private WasapiOut wo = null;

        private List<IAudioStateChanged> _audioStateChangeNotifiers = new List<IAudioStateChanged>();

        public void PlayAudio(Audio audio)
        {
            _playingQueue.Add(audio);
            _shouldContinuePlayingCurrentAudio = false;
            _currentAudio = audio;
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
                notifier.OnAudioProgressChanged(progress);
            }
        }

        private void NotifyAudioChanged(Audio audio)
        {
            foreach (var notifier in _audioStateChangeNotifiers)
            {
                notifier.OnAudioChanged(audio);
            }
        }

        public void NotifyPlayStateChanged(bool isPlaying)
        {
            foreach (var notifier in _audioStateChangeNotifiers)
            {
                notifier.OnAudioPlayStateChanged(isPlaying);
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

        public ListeningService()
        {
            _workerTask = new Task(() =>
            {
                while (_shouldRun)
                {
                    if (_currentAudio != null)
                    {
                        // play audio
                        PlayAudioUrl(_currentAudio.Url);
                    }
                }
            });
            _workerTask.Start();
        }

        public void Dispose()
        {
            _shouldRun = true;
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
                    Thread.Sleep(1000);

                    foreach (var notifier in _audioStateChangeNotifiers)
                    {
                        notifier.OnAudioProgressChanged((int)(mf.CurrentTime.TotalSeconds));
                    }
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
    }
}
