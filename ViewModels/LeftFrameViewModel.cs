using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.Views.Components;
using OneLastSong.Views.Dialogs;
using OneLastSong.Views.Pages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneLastSong.ViewModels
{
    public class LeftFrameViewModel : INotifyPlaylistChanged, INotifyPropertyChanged
    {
        public XamlRoot XamlRoot { get; set; }
        public ICommand CreateNewPlaylistCommand { get; set; }
        private PlaylistDAO _playlistDAO;
        private PlaylistService _playlistService;
        private NavigationService _navigationService;

        public LeftFrameViewModel()
        {
            CreateNewPlaylistCommand = new RelayCommand(CreateNewPlaylist);

            _playlistService = PlaylistService.Get();
            _playlistDAO = PlaylistDAO.Get();
            _navigationService = NavigationService.Get();

            _playlistService.RegisterPlaylistNotifier(this);
        }

        public void OpenPlaylistOptionsMenu(object sender, RightTappedRoutedEventArgs e, Playlist playlist)
        {
            var flyout = new PlaylistMenuFlyout(XamlRoot, playlist);
            flyout.ShowAt(sender as FrameworkElement, e.GetPosition(sender as UIElement));
        }

        private ObservableCollection<Playlist> _playlistList = new ObservableCollection<Playlist>();
        public ObservableCollection<Playlist> PlaylistList
        {
            get => _playlistList;
            set
            {
                if (_playlistList != value)
                {
                    _playlistList = value;
                    OnPropertyChanged(nameof(PlaylistList));
                }
            }
        }

        public async void InitPlaylist()
        {
            string sessionToken = UserDAO.Get().SessionToken;

            if(sessionToken == null || sessionToken == "")
            {
                return;
            }

            try
            {
                var playlists = await PlaylistDAO.Get().GetUserPlaylists(sessionToken);
                PlaylistList = new ObservableCollection<Playlist>(playlists);
            }
            catch (Exception e)
            {
                SnackbarUtils.ShowSnackbar(e.Message, SnackbarType.Error);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private List<Playlist> GenerateRandomPlaylists(int count)
        {
            var random = new Random();
            var playlists = new List<Playlist>();

            for (int i = 0; i < count; i++)
            {
                playlists.Add(new Playlist
                {
                    Name = $"Playlist {i + 1}",
                    ItemCount = random.Next(1, 100),
                });
            }

            return playlists;
        }

        // Unregister the notify when the view model is disposed
        public void Dispose()
        {
            _playlistService.UnregisterPlaylistNotifier(this);
        }

        ~LeftFrameViewModel()
        {
            Dispose();
        }

        public async void CreateNewPlaylist()
        {
            // if player is not signed in show error message
            if (UserDAO.Get().User == null)
            {
                await DialogUtils.ShowDialogAsync(
                    LocalizationUtils.GetString(LocalizationUtils.ERROR_STRING),
                    LocalizationUtils.GetString(LocalizationUtils.NOT_SIGNED_IN_ERROR_STRING),
                    XamlRoot
                );
                return;
            }
            // Show the create new playlist dialog
            await ShowCreateNewPlaylistDialogAsync();
        }

        public async Task ShowCreateNewPlaylistDialogAsync()
        {
            var dialog = new CreateNewPlaylistDialog
            {
                XamlRoot = this.XamlRoot
            };
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var sessionToken = UserDAO.Get().SessionToken;
                var playlistName = dialog.PlaylistName;

                try
                {
                    await PlaylistDAO.Get().AddUserPlaylist(sessionToken, playlistName);
                    SnackbarUtils.ShowSnackbar("Playlist created", SnackbarType.Success);
                }
                catch (Exception e)
                {
                    await DialogUtils.ShowDialogAsync(
                        LocalizationUtils.GetString(LocalizationUtils.ERROR_STRING),
                        e.Message,
                        XamlRoot
                    );
                }
            }
        }

        public void OnPlaylistUpdated(List<Playlist> playlists)
        {
            PlaylistList = new ObservableCollection<Playlist>(playlists);
        }

        internal void OpenPlaylist(Playlist playlist)
        {
            try
            {
                if (playlist.Audios == null || playlist.Audios.Length == 0)
                {
                    SnackbarUtils.ShowSnackbar("The playlist is empty", SnackbarType.Warning);
                    return;
                }
                // Navigate to the playlist songs page
                _navigationService.NavigateOrReloadOnParameterChanged(typeof(PlaylistSongsPage), playlist.PlaylistId);
            }
            catch (Exception ex)
            {
                SnackbarUtils.ShowSnackbar(ex.Message, SnackbarType.Error);
            }
        }
    }
}
