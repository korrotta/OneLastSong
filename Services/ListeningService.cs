using NAudio.Wave;
using NAudio.Dsp;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.UI.Xaml;

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

        public void PlayAudio(Audio audio)
        {
            _playingQueue.Add(audio);
            if (_currentAudio == null)
            {
                _currentAudio = audio;
            }
        }

        public ListeningService()
        {
            _workerTask = new Task(() =>
            {
                while (_shouldRun)
                {
                    if (_currentAudio != null && !_isPlaying)
                    {
                        // play audio
                        PlayAudioUrl(_currentAudio.Url);
                        // wait for audio to finish
                        _isPlaying = true;
                        // remove audio from queue
                        _playingQueue.Remove(_currentAudio);
                        // set current audio to null
                        _currentAudio = null;
                        _isPlaying = false;
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
                while (wo.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
            _isPlaying = true;
        }

        public static ListeningService Get()
        {
            return (ListeningService)((App)Application.Current).Services.GetService(typeof(ListeningService));
        }
    }
}
