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
using OneLastSong.Utils;
using OneLastSong.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    public sealed partial class AudioExtraControlsFlyout : MenuFlyout
    {
        public AudioExtraControlsFlyoutViewModel ViewModel { get; set; }

        public AudioExtraControlsFlyout()
        {
            this.InitializeComponent();
            ViewModel = new AudioExtraControlsFlyoutViewModel();
        }

        public Audio Audio
        {
            get { return (Audio)GetValue(AudioProperty); }
            set { 
                SetValue(AudioProperty, value); 
                ViewModel.SetAudio(value);
            }
        }

        public static readonly DependencyProperty AudioProperty =
            DependencyProperty.Register("Audio", typeof(Audio), typeof(AudioExtraControlsFlyout), new PropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Play_ButtonClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.PlayAudio();
        }

        private void PlayNext_ButtonClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.PlayNextAudio();
        }

        private void AddToQueue_ButtonClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.AddToQueue();
        }

        private void Like_ButtonClicked(object sender, RoutedEventArgs e)
        {
            LogUtils.Info("Like button clicked");
            LogUtils.Info("Audio: " + Audio.Title);
        }

        private void Details_ButtonClicked(object sender, RoutedEventArgs e)
        {
            ViewModel.GoToDetailsPage();
        }

        private void GoToArtist_ButtonClicked(object sender, RoutedEventArgs e)
        {
            LogUtils.Info("Go to artist button clicked");
            LogUtils.Info("Audio: " + Audio.Title);
        }

        private void GoToAlbum_ButtonClicked(object sender, RoutedEventArgs e)
        {
            LogUtils.Info("Go to album button clicked");
            LogUtils.Info("Audio: " + Audio.Title);
        }

        private void Share_ButtonClicked(object sender, RoutedEventArgs e)
        {
            LogUtils.Info("Share button clicked");
            LogUtils.Info("Audio: " + Audio.Title);
        }
    }
}
