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

        private List<IAudioStateChanged> _audioStateChangeNotifiers = new List<IAudioStateChanged>();

        public void PlayAudio(Audio audio)
        {
            _playingQueue.Add(audio);
            _shouldContinuePlayingCurrentAudio = false;
            _currentAudio = audio;
            NotifyAudioChanged(audio);
        }

        private void NotifyAudioChanged(Audio audio)
        {
            foreach (var notifier in _audioStateChangeNotifiers)
            {
                notifier.OnAudioChanged(audio);
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
            using (var mf = new MediaFoundationReader(url))
            using (var wo = new WasapiOut())
            {
                wo.Init(mf);
                wo.Play();
                _shouldContinuePlayingCurrentAudio = true;
                while (wo.PlaybackState == PlaybackState.Playing && _shouldContinuePlayingCurrentAudio)
                {
                    Thread.Sleep(50);
                }
                wo.Stop();
            }
        }

        public static ListeningService Get()
        {
            return (ListeningService)((App)Application.Current).Services.GetService(typeof(ListeningService));
        }
    }
}
