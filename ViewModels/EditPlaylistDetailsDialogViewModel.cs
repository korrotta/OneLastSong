using Microsoft.UI.Xaml;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Utils;
using OneLastSong.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
   public class EditPlaylistDetailsDialogViewModel : INotifyPropertyChanged
    {
        private Playlist _playlist;
        private PlaylistDAO _playlistDAO;
        private UserDAO _userDAO;

        public XamlRoot XamlRoot { get; set; }

        public EditPlaylistDetailsDialogViewModel(Playlist playlist)
        {
            _playlist = playlist.Clone() as Playlist;
            _playlistDAO = PlaylistDAO.Get();
            _userDAO = UserDAO.Get();
        }

        internal async Task UpdateCurrentPlaylist()
        {
            // Validate input
            // - Playlist name should not be empty or "Liked Playlist"
            // - Image URL should be a valid URL

            string validationMessage = null;

            string likedPlaylistName = ConfigValueUtils.GetConfigValue(ConfigValueUtils.LIKE_PLAYLIST_NAME_KEY);
            if (_playlist.Name == "" || _playlist.Name == likedPlaylistName)
            {
                validationMessage = "Playlist name should not be empty or \"" + likedPlaylistName + "\"";
            }
            else if (!ImageUtils.IsValidImageUrl(_playlist.CoverImageUrl))
            {
                validationMessage = "Image URL should be a valid URL";
            }

            if (validationMessage != null)
            {
                throw new Exception(validationMessage);
            }

            string token = _userDAO.SessionToken;
            await _playlistDAO.UpdatePlaylist(token, _playlist);
        }

        public Playlist Playlist
        {
            get => _playlist;
            set
            {
                if (_playlist != value)
                {
                    _playlist = value;
                    OnPropertyChanged(nameof(Playlist));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
