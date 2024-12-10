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
using OneLastSong.ViewModels;
using OneLastSong.Views.Components;
using OneLastSong.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PlayingQueuePage : Page, IDisposable
    {
        public PlayingQueuePageViewModel ViewModel { get; set; } = new PlayingQueuePageViewModel();

        public PlayingQueuePage()
        {
            this.InitializeComponent();
            Loaded += PlayingQueuePage_Loaded;
        }

        private void PlayingQueuePage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.XamlRoot = this.XamlRoot;
        }

        public void Dispose()
        {
            ViewModel.Dispose();
        }

        private void AudioItem_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ViewModel.OpenPlaylistOptionsMenu(sender, e);
        }
    }
}
