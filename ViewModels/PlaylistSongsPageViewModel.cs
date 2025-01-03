using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Utils;
using OneLastSong.Views.Dialogs;
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

        public XamlRoot XamlRoot { get; set; }

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
            FilteredAudios.Clear();

            foreach (var audio in _audios)
            {
                if (string.IsNullOrEmpty(_searchText) || audio.Title.Contains(_searchText, StringComparison.OrdinalIgnoreCase) || audio.Artist.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                {
                    FilteredAudios.Add(audio);
                }
            }
        }

        internal void SortAudios(string v, DataGridColumnEventArgs e, DataGrid dg)
        {
            if (v == "AudioId")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    FilteredAudios = new ObservableCollection<Audio>(_filteredAudios.OrderBy(audio => audio.AudioId));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    FilteredAudios = new ObservableCollection<Audio>(_filteredAudios.OrderByDescending(audio => audio.AudioId));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }
            else if (v == "Title")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    FilteredAudios = new ObservableCollection<Audio>(_filteredAudios.OrderBy(audio => audio.Title));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    FilteredAudios = new ObservableCollection<Audio>(_filteredAudios.OrderByDescending(audio => audio.Title));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }
            else if (v == "Artist")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    FilteredAudios = new ObservableCollection<Audio>(_filteredAudios.OrderBy(audio => audio.Artist));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    FilteredAudios = new ObservableCollection<Audio>(_filteredAudios.OrderByDescending(audio => audio.Artist));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }
            else if (v == "Duration")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    FilteredAudios = new ObservableCollection<Audio>(_filteredAudios.OrderBy(audio => audio.Duration));
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    FilteredAudios = new ObservableCollection<Audio>(_filteredAudios.OrderByDescending(audio => audio.Duration));
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }

            // Remove sorting indicator from other columns
            foreach (var dbColumn in dg.Columns)
            {
                if (dbColumn.Tag != null && dbColumn.Tag.ToString() != v)
                {
                    dbColumn.SortDirection = null;
                }
            }
        }

        public async Task ShowConfirmRemoveAudioFromPlaylist(Audio audio)
        {
            if(XamlRoot == null)
            {
                return;
            }

            SimpleThemedConfirmDialog dialog = new SimpleThemedConfirmDialog
            {
                ConfirmMessage = $"Are you sure you want to delete \"{audio.Title}\" from \"{CurrentPlaylist.Name}\" playlist?",
                XamlRoot = this.XamlRoot,
            };

            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                await RemoveAudioFromPlaylist(audio);    
            }
        }

        internal async Task RemoveAudioFromPlaylist(Audio audio)
        {
            try
            {
                string token = _userDAO.SessionToken;
                if (string.IsNullOrEmpty(token))
                {
                    SnackbarUtils.ShowSnackbar("You need to login first", SnackbarType.Warning);
                    return;
                }
                await _playlistDAO.RemoveAudioFromPlaylist(token, CurrentPlaylist.PlaylistId, audio.AudioId);
                Audios.Remove(audio);
                FilteredAudios.Remove(audio);
                SnackbarUtils.ShowSnackbar("Audio removed from playlist", SnackbarType.Success);
            }
            catch (Exception ex)
            {
                SnackbarUtils.ShowSnackbar("There was an error while removing the audio from the playlist", SnackbarType.Error);
            }
        }
    }
}
