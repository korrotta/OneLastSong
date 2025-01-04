using OneLastSong.DAOs;
using OneLastSong.Models;
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

        public EditPlaylistDetailsDialogViewModel(Playlist playlist)
        {
            _playlist = playlist.Clone() as Playlist;
            _playlistDAO = PlaylistDAO.Get();
            _userDAO = UserDAO.Get();
        }

        internal async Task UpdateCurrentPlaylist()
        {
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
