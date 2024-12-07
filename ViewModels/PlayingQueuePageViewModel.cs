using OneLastSong.Contracts;
using OneLastSong.Cores.AudioSystem;
using OneLastSong.Models;
using OneLastSong.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class PlayingQueuePageViewModel : INotifyPropertyChanged, INotifyPlayQueueChanged, IAudioStateChanged, IDisposable
    {
        private ListeningService _listeningService;
        private PlayModeData _playModeData;

        private Audio _currentAudio = Audio.Default;
        public Audio CurrentAudio
        {
            get { return _currentAudio; }
            set
            {
                _currentAudio = value;
                NotifyPropertyChanged(nameof(CurrentAudio));
            }
        }

        public PlayingQueuePageViewModel()
        {
            _listeningService = ListeningService.Get();
            _playModeData = _listeningService.PlayModeData;
            _playingQueue = new List<Audio>();

            _listeningService.RegisterPlayQueueChangeListeners(this);
            _listeningService.RegisterAudioStateChangeListeners(this);

            AddAudiosToList(_playModeData.PlayQueue);
        }

        private void AddAudiosToList(List<Audio> playQueue)
        {
            foreach (var audio in playQueue)
            {
                _playingQueue.Add(audio);
            }
        }

        private List<Audio> _playingQueue;

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Dispose()
        {
            _listeningService.UnregisterPlayQueueChangeListeners(this);
            _listeningService.UnregisterAudioStateChangeListeners(this);
        }

        public void OnPlayQueueChanged(List<Audio> audios)
        {
            PlayingQueue = audios;
        }

        public void OnAudioChanged(Audio newAudio)
        {
            CurrentAudio = newAudio;
        }

        public void OnAudioPlayStateChanged(bool isPlaying)
        {

        }

        public void OnAudioProgressChanged(int progress)
        {

        }

        public List<Audio> PlayingQueue
        {
            get { return _playingQueue; }
            set
            {
                _playingQueue = value;
                NotifyPropertyChanged(nameof(PlayingQueue));
            }
        }
    }
}
