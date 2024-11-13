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
using OneLastSong.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    public sealed partial class PlaylistMenuFlyout : MenuFlyout
    {
        private Playlist _playlist;
        private XamlRoot _xamlRoot;

        public PlaylistMenuFlyoutViewModel ViewModel { get; set; } = new PlaylistMenuFlyoutViewModel();

        public PlaylistMenuFlyout(XamlRoot xamlRoot, Playlist playlist)
        {
            this.InitializeComponent();
            _playlist = playlist;
            _xamlRoot = xamlRoot;
            ViewModel.XamlRoot = _xamlRoot;
            ViewModel.Playlist = _playlist;
        }

        private void EditPlaylistDetails_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.EditPlaylistDetailsCommand.Execute(null);
        }
    }
}
