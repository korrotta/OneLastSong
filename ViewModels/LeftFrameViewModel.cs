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
    public class LeftFrameViewModel : IAuthChangeNotify, INotifyPropertyChanged
    {
        public XamlRoot XamlRoot { get; set; }
        public ICommand CreateNewPlaylistCommand { get; set; }

        public LeftFrameViewModel()
        {
            AuthService.Get().RegisterAuthChangeNotify(this);
            CreateNewPlaylistCommand = new RelayCommand(CreateNewPlaylist);
        }

        public void OpenPlaylistOptionsMenu(object sender, RightTappedRoutedEventArgs e, Playlist playlist)
        {
            var flyout = new PlaylistMenuFlyout(XamlRoot, playlist);
            flyout.ShowAt(sender as FrameworkElement, e.GetPosition(sender as UIElement));
        }

        private ObservableCollection<Playlist> _playlistList;
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

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;


        public async void OnUserChange(User user)
        {
            // User is null when logged out, reset the playlist list
            if (user == null)
            {
                PlaylistList.Clear();
                return;
            }

            var sessionToken = UserDAO.Get().SessionToken;
            var playlists = await PlaylistDAO.Get().GetUserPlaylists(sessionToken, true);
            // add the user's playlists to the list
            PlaylistList = new ObservableCollection<Playlist>(playlists);
        }

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
            AuthService.Get().UnregisterAuthChangeNotify(this);
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
                    var playlist = await PlaylistDAO.Get().AddUserPlaylist(sessionToken, playlistName);
                    PlaylistList.Add(playlist);
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
    }
}
