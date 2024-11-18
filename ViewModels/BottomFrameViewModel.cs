using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using OneLastSong.Services;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace OneLastSong.ViewModels
{
    public class BottomFrameViewModel : INotifyPropertyChanged, IAudioStateChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ListeningService _listeningService;
        private Audio _currentAudio = Audio.Default;

        public Audio CurrentAudio
        {
            get => _currentAudio;
            set
            {
                if (_currentAudio != value)
                {
                    _currentAudio = value;
                    OnPropertyChanged(nameof(CurrentAudio));
                }
            }
        }

        public BottomFrameViewModel()
        {
            _listeningService = ListeningService.Get();
            _listeningService.RegisterAudioStateChangeListeners(this);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnAudioChanged(Audio audio)
        {
            CurrentAudio = audio;
            OnPropertyChanged(nameof(CurrentAudio));
        }

        public void OnAudioPlayStateChanged(bool isPlaying)
        {
            throw new NotImplementedException();
        }

        public void OnAudioProgressChanged(TimeSpan progress)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _listeningService.UnregisterAudioStateChangeListeners(this);
        }
    }
}
