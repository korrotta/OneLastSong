using Microsoft.UI.Xaml.Controls;
using OneLastSong.ViewModels;
using System;

namespace OneLastSong.Views.Components
{
    public sealed partial class BottomFrame : Page
    {
        public BottomFrameViewModel ViewModel { get; set; }

        public BottomFrame()
        {
            this.InitializeComponent();
            this.ViewModel = new BottomFrameViewModel();
            this.DataContext = this.ViewModel;

            ViewModel.LoadSong(new Uri("https://vgmsite.com/soundtracks/minecraft/kvptjmornx/1-18.%20Sweden.mp3"));
        }

        private void PlaybackButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ViewModel.PlayPause();
            PlaybackIcon.Glyph = ViewModel.IsPlaying ? "\uE769" : "\uE768";
        }

        private void RepeatButton_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

        }

        private void ProgressBar_Loaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {
            ProgressBar.Maximum = ViewModel.TotalTime.TotalSeconds;
        }

        private void ProgressBar_Moved(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {

        }


        private void VolumeSlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            ViewModel.SetVolume(e.NewValue / 100);
        }

        private void Fullscreen_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        {

        }
    }
}
