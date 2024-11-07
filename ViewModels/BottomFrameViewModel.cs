using Microsoft.UI.Xaml;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace OneLastSong.ViewModels
{
    public class BottomFrameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private DispatcherTimer _timer;
        private MediaPlayer _mediaPlayer;
        private bool _isPlaying;

        private TimeSpan _currentTime;
        private TimeSpan _totalTime;
        public string CurrentTimeFormatted => CurrentTime.ToString(@"mm\:ss");
        public string TotalTimeFormatted => TotalTime.ToString(@"mm\:ss");

        private string _songName;
        private string _artist;
        private double _volume = 0.5; // Default volume is 50%

        public BottomFrameViewModel()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            _mediaPlayer = new MediaPlayer();
             _mediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
            _mediaPlayer.CurrentStateChanged += MediaPlayer_CurrentStateChanged;
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }

        public TimeSpan CurrentTime
        {
            get => _currentTime;
            set
            {
                if (_currentTime != value)
                {
                    _currentTime = value;
                    OnPropertyChanged(nameof(CurrentTimeFormatted));
                }
            }
        }

        public TimeSpan TotalTime
        {
            get => _totalTime;
            set
            {
                if (_totalTime != value)
                {
                    _totalTime = value;
                    OnPropertyChanged(nameof(TotalTimeFormatted));
                }
            }
        }

        public string SongName
        {
            get => _songName;
            set
            {
                _songName = value;
                OnPropertyChanged(nameof(SongName));
            }
        }

        public string Artist
        {
            get => _artist;
            set
            {
                _artist = value;
                OnPropertyChanged(nameof(Artist));
            }
        }

        public double Volume
        {
            get => _volume;
            set
            {
                if (_volume != value)
                {
                    _volume = value;
                    _mediaPlayer.Volume = _volume;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        public void PlayPause()
        {
            if (IsPlaying)
            {
                _mediaPlayer.Pause();
                _timer.Stop();
            }
            else
            {
                _mediaPlayer.Play();
                _timer.Start();
            }
            IsPlaying = !IsPlaying;
        }

        public void SetVolume(double newVolume)
        {
            Volume = newVolume;
        }

        private void Timer_Tick(object sender, object e)
        {
            if (_mediaPlayer.PlaybackSession != null)
            {
                CurrentTime = _mediaPlayer.PlaybackSession.Position;
            }
        }

        private void UpdateCurrentTime(object sender, object e)
        {
            if (_mediaPlayer.PlaybackSession != null)
            {
                CurrentTime = _mediaPlayer.PlaybackSession.Position;
                TotalTime = _mediaPlayer.PlaybackSession.NaturalDuration;
            }
        }

        private void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            if (sender.PlaybackSession != null)
            {
                if (sender.PlaybackSession.NaturalDuration != TimeSpan.Zero)
                {
                    TotalTime = sender.PlaybackSession.NaturalDuration;
                }
                else
                {
                    // Retry after a short delay if the duration is not yet available
                    Task.Delay(100).ContinueWith(_ =>
                    {
                        if (sender.PlaybackSession.NaturalDuration != TimeSpan.Zero)
                        {
                            TotalTime = sender.PlaybackSession.NaturalDuration;
                        }
                    });
                }
            }
        }

        private void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            IsPlaying = sender.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void LoadSong(Uri songUri)
        {
            var mediaSource = MediaSource.CreateFromUri(songUri);
            var mediaPlaybackItem = new MediaPlaybackItem(mediaSource);
            mediaPlaybackItem.TimedMetadataTracksChanged += PlaybackItem_MetadataTracksChanged;

            _mediaPlayer.Source = mediaPlaybackItem;
        }

        private void PlaybackItem_MetadataTracksChanged(MediaPlaybackItem sender, IVectorChangedEventArgs args)
        {
            foreach (var track in sender.TimedMetadataTracks)
            {
                if (track.Id == "ART")
                {
                    Artist = track.Label;
                }
                else if (track.Id == "TIT2")
                {
                    SongName = track.Label;
                }
            }
        }
    }
}
