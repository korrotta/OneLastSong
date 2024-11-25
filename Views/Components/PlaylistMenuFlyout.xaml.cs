using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.ViewModels;
using OneLastSong.Views.Dialogs;
using System;
using System.Threading.Tasks;

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
        private ListeningService _listeningService;
        private NavigationService _navService = null;

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
            _listeningService = ListeningService.Get();
            _navService = NavigationService.Get();
        }

        private void EditPlaylistDetails_Click(object sender, RoutedEventArgs e)
        {
            LogUtils.Debug("EditPlaylistDetails_Click");
            _navService.Navigate(typeof(PlaylistViewPage), _playlist);
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

        private void AddToQueue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_playlist.Audios == null || _playlist.Audios.Length == 0)
                {
                    SnackbarUtils.ShowSnackbar("The playlist is empty", SnackbarType.Warning);
                    return;
                }

                _listeningService.AddPlaylistToQueue(_playlist);
                SnackbarUtils.ShowSnackbar("Playlist added to queue", SnackbarType.Success);
            }
            catch (Exception ex)
            {
                SnackbarUtils.ShowSnackbar(ex.Message, SnackbarType.Error);
            }
        }

        private void PlayPlaylist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_playlist.Audios == null || _playlist.Audios.Length == 0)
                {
                    SnackbarUtils.ShowSnackbar("The playlist is empty", SnackbarType.Warning);
                    return;
                }

                _listeningService.PlayPlaylist(_playlist);
                SnackbarUtils.ShowSnackbar("Playlist is now playing", SnackbarType.Success);
            }
            catch (Exception ex)
            {
                SnackbarUtils.ShowSnackbar(ex.Message, SnackbarType.Error);
            }
        }
    }
}
