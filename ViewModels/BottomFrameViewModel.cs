using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using OneLastSong.Views;
using OneLastSong.Views.Pages;

namespace OneLastSong.ViewModels
{
    public class BottomFrameViewModel : INotifyPropertyChanged, IAudioStateChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ListeningService _listeningService;
        private NavigationService _navigationService;
        private SidePanelNavigationService _sidePanelNavigationService;
        private Audio _currentAudio = Audio.Default;
        private int _currentProgress = 0;
        private Slider _slider;
        private bool _isPlaying = false;
        public ICommand ChangePlayStateCommand { get; set; }
        public ICommand PlayNextCommand { get; set; }
        public ICommand RestartPlayCommand { get; set; }
        public ICommand ConfigEqualizerCommand { get; set; }
        public ICommand OpenPlayQueueCommand { get; set; }
        public ICommand OpenLyricsCommand { get; set; }

        private bool _isRepeat = false;

        public bool IsRepeat
        {
            get => _isRepeat;
            set
            {
                if (_isRepeat != value)
                {
                    _isRepeat = value;
                    OnPropertyChanged(nameof(IsRepeat));
                }
            }
        }

        private float _volume = 0.5f;

        public float Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        private bool _isFullScreen = false;

        public bool IsFullScreen
        {
            get => _isFullScreen;
            set
            {
                if (_isFullScreen != value)
                {
                    _isFullScreen = value;
                    OnPropertyChanged(nameof(IsFullScreen));
                }
            }
        }

        public BottomFrameViewModel(DispatcherQueue dispatcherQueue, Slider slider)
        {
            _listeningService = ListeningService.Get();
            _sidePanelNavigationService = SidePanelNavigationService.Get();
            _navigationService = NavigationService.Get();
            
            _listeningService.RegisterAudioStateChangeListeners(this);
            
            this._slider = slider;

            ChangePlayStateCommand = new RelayCommand(ChangePlayState);
            PlayNextCommand = new RelayCommand(PlayNext);
            RestartPlayCommand = new RelayCommand(RestartPlay);
            ConfigEqualizerCommand = new RelayCommand(ConfigEqualizer);
            OpenPlayQueueCommand = new RelayCommand(OpenPlayQueue);
            OpenLyricsCommand = new RelayCommand(OpenLyrics);
        }

        public void OnLoaded()
        {
            IsRepeat = _listeningService.IsRepeating;
            Volume = _listeningService.Volume;
            IsFullScreen = (Application.Current as App).IsFullScreen();
        }

        private void OpenLyrics()
        {
            _sidePanelNavigationService.Navigate(typeof(AudioLyricPage), CurrentAudio);
        }

        private void OpenPlayQueue()
        {
            _sidePanelNavigationService.Navigate(typeof(PlayingQueueWithListeningHistoryPage));
        }

        private void ConfigEqualizer()
        {
            _navigationService.Navigate(typeof(EqualizerPage));
        }

        private void RestartPlay()
        {
            _listeningService.RestartPlay();
        }

        private void PlayNext()
        {
            _listeningService.PlayNext();
        }

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

        public int CurrentProgress
        {
            get => _currentProgress;
            set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    OnPropertyChanged(nameof(CurrentProgress));
                }
            }
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying));
                }
            }
        }

        private void ChangePlayState()
        {
            _listeningService.ChangePlayState();
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnAudioChanged(Audio newAudio)
        {
            CurrentAudio = newAudio;
            // _sidePanelNavigationService.Navigate(typeof(AudioLyricPage), newAudio);
            OnPropertyChanged(nameof(CurrentAudio));
        }

        public void OnAudioPlayStateChanged(bool isPlaying)
        {
            IsPlaying = isPlaying;
        }

        public void OnAudioProgressChanged(int progress)
        {
            _slider.Value = progress;
            CurrentProgress = progress;
        }

        public void Dispose()
        {
            _listeningService.UnregisterAudioStateChangeListeners(this);
        }

        public void OnSliderValueChanged(int newValue)
        {
            // Add any additional logic you need when the slider value changes
            _listeningService.Seek(newValue);
        }

        internal void OnSongTitleClicked()
        {          
            _navigationService.NavigateOrReloadOnParameterChanged(typeof(AudioDetailsPage), CurrentAudio.AudioId.ToString());
        }

        internal void OnRepeatButtonClicked()
        {
            IsRepeat = _listeningService.ToggleRepeat();
        }

        internal void OnVolumeSliderValueChanged(float newValue)
        {
            Volume = _listeningService.SetVolume(newValue);
        }

        internal void OnShuffleButtonClicked()
        {
            _listeningService.Shuffle();
        }

        public ListeningService ListeningService
        {
            get => _listeningService;
        }

        public void ToggleFullScreen()
        {
            IsFullScreen = !IsFullScreen;

            (Application.Current as App).SetFullScreen(IsFullScreen);
        }
    }
}
