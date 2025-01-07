using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using OneLastSong.Utils;
using OneLastSong.ViewModels;
using System;

namespace OneLastSong.Views.Components
{
    public sealed partial class BottomFrame : Page
    {
        public BottomFrameViewModel ViewModel { get; set; }
        DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        public BottomFrame()
        {
            this.InitializeComponent();
            ViewModel = new BottomFrameViewModel(dispatcherQueue, ProgressBar);
            this.DataContext = this.ViewModel;
            this.Loaded += BottomFrame_Loaded;            
        }

        private void BottomFrame_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.OnLoaded();
        }

        private void Slider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            var slider = sender as Slider;
            ViewModel.OnSliderValueChanged((int)slider.Value);
        }

        private void SongTitleHyperlink_Clicked(Hyperlink sender, HyperlinkClickEventArgs args)
        {
            ViewModel.OnSongTitleClicked();
        }

        private void RepeatButton_Clicked(object sender, RoutedEventArgs e)
        {
            ViewModel.OnRepeatButtonClicked();
        }

        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            ViewModel.OnVolumeSliderValueChanged((float)e.NewValue);
        }

        private void ShuffleButton_Clicked(object sender, RoutedEventArgs e)
        {
            ViewModel.OnShuffleButtonClicked();
            SnackbarUtils.ShowSnackbar("The play queue has been shuffled.", SnackbarType.Success);
        }

        private void FullScreenButton_Clicked(object sender, RoutedEventArgs e)
        {
            ViewModel.ToggleFullScreen();
        }
    }
}
