using Microsoft.UI.Xaml.Controls;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class LeftFrameViewModel : IAuthChangeNotify, INotifyPropertyChanged
    {
        public LeftFrameViewModel()
        {
            AuthService.Get().RegisterAuthChangeNotify(this);
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
            if(user == null)
            {
                return;
            }

            var sessionToken = AuthService.Get().SessionToken();
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
    }
}
