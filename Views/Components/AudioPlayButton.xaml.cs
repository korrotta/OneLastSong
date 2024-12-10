using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System.ComponentModel;
using OneLastSong.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    public sealed partial class AudioPlayButton : UserControl, INotifyPropertyChanged
    {
        public AudioPlayButton()
        {
            this.InitializeComponent();
            Audio = Audio.Default;
        }

        public Audio Audio
        {
            get { return (Audio)GetValue(AudioProperty); }
            set { SetValue(AudioProperty, value); }
        }

        public static readonly DependencyProperty AudioProperty =
            DependencyProperty.Register("Audio", typeof(Audio), typeof(AudioExtraControlsButton), new PropertyMetadata(null));


        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get { return _isPlaying; }
            set
            {
                _isPlaying = value;
                NotifyPropertyChanged(nameof(IsPlaying));
            }
        }

        private string _playButtonVisibility = "Collapsed";
        public string PlayButtonVisibility
        {
            get { return _playButtonVisibility; }
            set
            {
                _playButtonVisibility = value;
                NotifyPropertyChanged(nameof(PlayButtonVisibility));
            }
        }

        public void ChangePlayButtonVisibility(bool isVisible)
        {
            PlayButtonVisibility = isVisible ? "Visible" : "Collapsed";
        }

        private void Play_ButtonClicked(object sender, RoutedEventArgs e)
        {
            IsPlaying = !IsPlaying;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
