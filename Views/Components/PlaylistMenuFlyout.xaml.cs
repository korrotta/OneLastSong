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
using OneLastSong.DAOs;
using OneLastSong.Utils;
using System.Threading.Tasks;
using OneLastSong.Views.Dialogs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    public sealed partial class PlaylistMenuFlyout : MenuFlyout
    {
        private Playlist _playlist;
        private XamlRoot _xamlRoot;
        private PlaylistDAO _playlistDAO;
        private UserDAO _userDAO;

        public PlaylistMenuFlyoutViewModel ViewModel { get; set; } = new PlaylistMenuFlyoutViewModel();

        public PlaylistMenuFlyout(XamlRoot xamlRoot, Playlist playlist)
        {
            this.InitializeComponent();
            _playlist = playlist;
            _xamlRoot = xamlRoot;
            ViewModel.XamlRoot = _xamlRoot;
            ViewModel.Playlist = _playlist;
            _playlistDAO = PlaylistDAO.Get();
            _userDAO = UserDAO.Get();
        }

        private void EditPlaylistDetails_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.EditPlaylistDetailsCommand.Execute(null);
        }

        private async void RemovePlaylist_Click(object sender, RoutedEventArgs e)
        {
            await ShowConfirmDeletePlaylistDialog();
        }

        private async void RemovePlaylist()
        {
            try
            {
                await _playlistDAO.DeletePlaylist(_userDAO.SessionToken, _playlist.PlaylistId);
                SnackbarUtils.ShowSnackbar("Playlist removed successfully", SnackbarType.Success);
            }
            catch (Exception ex)
            {
                SnackbarUtils.ShowSnackbar(ex.Message, SnackbarType.Error);
            }
        }

        private async Task ShowConfirmDeletePlaylistDialog()
        {
            SimpleThemedConfirmDialog dialog = new SimpleThemedConfirmDialog
            {
                ConfirmMessage = $"Are you sure you want to delete \"{_playlist.Name}\" playlist?",
                XamlRoot = _xamlRoot
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                RemovePlaylist();
            }
        }
    }
}
