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
using Microsoft.Extensions.DependencyInjection;
using OneLastSong.Utils;
using OneLastSong.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LeftRegion : Page, IDisposable
    {
        public LeftFrameViewModel ViewModel { get; set; } = new LeftFrameViewModel();

        public LeftRegion()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            Loaded += LeftRegion_Loaded;
        }

        private void LeftRegion_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.XamlRoot = XamlRoot;
            ViewModel.InitPlaylist();
        }

        private void Grid_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var grid = sender as Grid;
            var playlist = grid.DataContext as Playlist;
            var viewModel = DataContext as LeftFrameViewModel;
            ViewModel?.OpenPlaylistOptionsMenu(sender, e, playlist);
        }

        public void Dispose()
        {
            ViewModel?.Dispose();
        }

        private void PlaylistName_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Playlist playlist)
            {
                ViewModel?.OpenPlaylist(playlist);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel?.Search(SearchBox.Text);
        }
    }

}
