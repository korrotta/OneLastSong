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
using System.Collections.ObjectModel;
using OneLastSong.Models;
using OneLastSong.DAOs;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.Cores.DataItems;
using OneLastSong.Contracts;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    public sealed partial class AddToPlaylistMenuFlyout : MenuFlyout, INotifyPlaylistChanged, IDisposable
    {
        private ObservableCollection<Playlist> playlists = new ObservableCollection<Playlist>();
        private PlaylistDAO playlistDAO;
        private PlaylistService playlistService;
        private UserDAO userDAO;

        public int AudioId
        {
            get { return (int)GetValue(AudioIdProperty); }
            set { SetValue(AudioIdProperty, value); }
        }

        public static readonly DependencyProperty AudioIdProperty =
            DependencyProperty.Register("AudioId", typeof(int), typeof(AddToPlaylistMenuFlyout), new PropertyMetadata(0));

        public AddToPlaylistMenuFlyout()
        {
            this.InitializeComponent();
            playlistDAO = PlaylistDAO.Get();
            playlistService = PlaylistService.Get();
            userDAO = UserDAO.Get();
            playlistService.RegisterPlaylistNotifier(this);

            this.Opened += AddToPlaylistMenuFlyout_Opened;
            // Subscribe to the Closed event
            this.Closed += AddToPlaylistMenuFlyout_Closed;
        }

        private async void AddToPlaylistMenuFlyout_Opened(object sender, object e)
        {
            await InitUserPlaylist();
        }

        private async Task InitUserPlaylist()
        {
            try
            {
                if (string.IsNullOrEmpty(userDAO.SessionToken))
                {
                    return;
                }

                var userPlaylists = await playlistDAO.GetUserPlaylists(userDAO.SessionToken);
                Load(userPlaylists);
            }
            catch (Exception ex)
            {
                SnackbarUtils.ShowSnackbar(ex.Message, SnackbarType.Error);
            }
        }

        private void Load(List<Playlist> userPlaylists)
        {
            // clear submenu
            PlaylistsSubItem.Items.Clear();
            // add user playlists to submenu
            foreach (var playlist in userPlaylists)
            {
                // check if audio already in playlist
                if (playlist.ContainsAudio(AudioId))
                {
                    continue;
                }

                var menuItem = new MenuFlyoutItem
                {
                    Text = playlist.Name,
                    Tag = playlist.PlaylistId,
                    Foreground = ThemeUtils.GetBrush(ThemeUtils.TEXT_PRIMARY)
                };
                menuItem.Click += AddToPlaylist_Click;
                PlaylistsSubItem.Items.Add(menuItem);
            }
        }

        private async void AddToPlaylist_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                await playlistDAO.AddAudioToPlaylist(userDAO.SessionToken, (int)((MenuFlyoutItem)sender).Tag, AudioId);
                SnackbarUtils.ShowSnackbar("Audio added to playlist", SnackbarType.Success);
            }
            catch (Exception ex)
            {
                SnackbarUtils.ShowSnackbar(ex.Message, SnackbarType.Error);
            }
        }

        public void OnPlaylistUpdated(List<Playlist> playlists)
        {
            Load(playlists);
        }

        private void AddToPlaylistMenuFlyout_Closed(object sender, object e)
        {
            Dispose();
        }

        public void Dispose()
        {
            playlistService.UnregisterPlaylistNotifier(this);
        }
    }
}
