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
using OneLastSong.Models;
using OneLastSong.DAOs;
using OneLastSong.ViewModels;
using OneLastSong.Services;
using OneLastSong.Utils;
using Microsoft.VisualBasic.Devices;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page, IDisposable
    {
        public HomePageViewModel ViewModel { get; set; } = new HomePageViewModel();

        public HomePage()
        {
            this.InitializeComponent();
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            await ViewModel.Load();
            ViewModel.UpdateView();
        }

        // On navigated to
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        public void Dispose()
        {
            ViewModel?.Dispose();
        }

        public void AudioTitleHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is HyperlinkButton hyperlinkButton)
            {
                try
                {
                    ViewModel.NavigateToAudioDetails(((int)hyperlinkButton.Tag).ToString());
                }
                catch (Exception ex)
                {
                    // Log exception
                    SnackbarUtils.ShowSnackbar("There was an error while navigating to audio details", SnackbarType.Error);
                }
            }
        }

        public void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                try
                {
                    int audioId = (int)button.Tag;
                    ViewModel.HandleLikeButtonClick(audioId);
                }
                catch (Exception ex)
                {
                    // Log exception
                    SnackbarUtils.ShowSnackbar("There was an error while liking/disliking the audio", SnackbarType.Error);
                }
            }
        }
    }
}
