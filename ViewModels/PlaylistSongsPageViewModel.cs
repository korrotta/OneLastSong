using Microsoft.UI.Xaml.Data;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class PlaylistSongsPageViewModel : INotifyPropertyChanged
    {
        private AudioDAO _audioDAO;
        private PlaylistDAO _playlistDAO;
        private UserDAO _userDAO;
        private ObservableCollection<Audio> _audios = new ObservableCollection<Audio>();
        private ObservableCollection<Audio> _filteredAudios = new ObservableCollection<Audio>();
        private Playlist _playlist;
        private string _searchText;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<Audio> Audios
        {
            get => _audios;
            set
            {
                if (_audios != value)
                {
                    _audios = value;
                    OnPropertyChanged(nameof(Audios));
                }
            }
        }

        public ObservableCollection<Audio> FilteredAudios
        {
            get => _filteredAudios;
            set
            {
                if (_filteredAudios != value)
                {
                    _filteredAudios = value;
                    OnPropertyChanged(nameof(FilteredAudios));
                }
            }
        }

        public Playlist CurrentPlaylist
        {
            get => _playlist;
            set
            {
                if (_playlist != value)
                {
                    _playlist = value;
                    OnPropertyChanged(nameof(CurrentPlaylist));
                }
            }
        }

        public PlaylistSongsPageViewModel()
        {
            _audioDAO = AudioDAO.Get();
            _playlistDAO = PlaylistDAO.Get();
            _userDAO = UserDAO.Get();
        }

        public async Task Load(int playlistId)
        {
            _audios.Clear();
            _filteredAudios.Clear();
            string token = _userDAO.SessionToken;

            if (string.IsNullOrEmpty(token))
            {
                SnackbarUtils.ShowSnackbar("You need to login first", SnackbarType.Warning);
                return;
            }

            CurrentPlaylist = _playlistDAO.GetCachedPlaylists().FirstOrDefault(playlist => playlist.PlaylistId == playlistId);

            List<Audio> audios = await _playlistDAO.GetAudiosInPlaylist(token, playlistId);
            foreach (Audio audio in CurrentPlaylist.Audios)
            {
                _audios.Add(audio);
                _filteredAudios.Add(audio);
            }
        }

        public void FilterAudios(string searchText)
        {
            _searchText = searchText;
            _filteredAudios.Clear();

            foreach (var audio in _audios)
            {
                if (string.IsNullOrEmpty(_searchText) || audio.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || audio.Artist.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                {
                    _filteredAudios.Add(audio);
                }
            }
        }
    }
}
