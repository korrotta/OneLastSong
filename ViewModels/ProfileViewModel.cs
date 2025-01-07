using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Cores.DataItems;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Utils;
using OneLastSong.Views.Components;
using OneLastSong.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private UserDAO userDAO = UserDAO.Get();
        private PlayHistoryDAO playHistoryDAO = PlayHistoryDAO.Get();
        private PlaylistDAO playlistDAO = PlaylistDAO.Get();
        private string _username;
        private string _profilePictureUrl;
        private string _description;
        private ObservableCollection<Playlist> _playlists;
        private ObservableCollection<PlayHistoryItem> _playHistory;
        private List<PlayHistoryItem> _filteredPlayHistoryItems;
        private int _currentPage;
        private int _maxPage;
        public XamlRoot XamlRoot { get; set; }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string ProfilePictureUrl
        {
            get => _profilePictureUrl;
            set
            {
                _profilePictureUrl = value;
                OnPropertyChanged(nameof(ProfilePictureUrl));
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public ObservableCollection<Playlist> Playlists
        {
            get => _playlists;
            set
            {
                _playlists = value;
                OnPropertyChanged(nameof(Playlists));
            }
        }

        public ObservableCollection<PlayHistoryItem> PlayHistory
        {
            get => _playHistory;
            set
            {
                _playHistory = value;
                OnPropertyChanged(nameof(PlayHistory));
            }
        }

        public int CurrentPage
        {
            get => _currentPage;
            set
            {
                _currentPage = value;
                OnPropertyChanged(nameof(CurrentPage));
                UpdatePlayHistory();
            }
        }

        public int MaxPage
        {
            get => _maxPage;
            set
            {
                _maxPage = value;
                OnPropertyChanged(nameof(MaxPage));
            }
        }

        public ProfileViewModel()
        {
            Username = userDAO.User.Username;
            ProfilePictureUrl = userDAO.User.AvatarUrl;
            Description = userDAO.User.Description;
            _filteredPlayHistoryItems = new List<PlayHistoryItem>();
            _playHistory = new ObservableCollection<PlayHistoryItem>();
            CurrentPage = 1;

            Load();
        }

        private async void Load()
        {
            if (String.IsNullOrEmpty(userDAO.SessionToken))
            {
                SnackbarUtils.ShowSnackbar("You need to login to view your profile", SnackbarType.Warning);
                return;
            }

            await LoadPlayHistory();
            await LoadPlaylists();
        }

        private async Task LoadPlaylists()
        {
            if(String.IsNullOrEmpty(userDAO.SessionToken))
            {
                // SnackbarUtils.ShowSnackbar("You need to login to view your profile", SnackbarType.Warning);
                return;
            }

            var playlists = await playlistDAO.GetUserPlaylists(userDAO.SessionToken);

            Playlists = new ObservableCollection<Playlist>(playlists);
        }

        private async Task LoadPlayHistory()
        {
            if (String.IsNullOrEmpty(userDAO.SessionToken))
            {
                //SnackbarUtils.ShowSnackbar("You need to login to view your profile", SnackbarType.Warning);
                return;
            }

            List<PlayHistory> playHistoryList = await playHistoryDAO.GetUserPlayHistory(userDAO.SessionToken);

            _filteredPlayHistoryItems.Clear();

            foreach (var playHistory in playHistoryList)
            {
                var audio = await AudioDAO.Get().GetAudioById(playHistory.AudioId);
                var audioItem = new AudioItem(audio);
                var playHistoryItem = new PlayHistoryItem(audioItem, playHistory.PlayedAt);
                _filteredPlayHistoryItems.Add(playHistoryItem);
            }

            MaxPage = (int)Math.Ceiling((double)_filteredPlayHistoryItems.Count / 8);
            UpdatePlayHistory();
        }

        private void UpdatePlayHistory()
        {
            PlayHistory.Clear();
            var items = _filteredPlayHistoryItems.Skip((CurrentPage - 1) * 8).Take(8);
            foreach (var item in items)
            {
                PlayHistory.Add(item);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        internal void SortAudios(string v, DataGridColumnEventArgs e, DataGrid dg)
        {
            if (v == "AudioId")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderBy(audio => audio.AudioId).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderByDescending(audio => audio.AudioId).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }
            else if (v == "Title")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderBy(audio => audio.AudioItem.Title).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderByDescending(audio => audio.AudioItem.Title).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }
            else if (v== "RelativePlayedAtString")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderBy(audio => audio.RelativePlayedAtString).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderByDescending(audio => audio.RelativePlayedAtString).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }
            else if(v == "PlayedAt")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderBy(audio => audio.PlayedAt).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderByDescending(audio => audio.PlayedAt).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }
            else if (v == "Artist")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderBy(audio => audio.AudioItem.Artist).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderByDescending(audio => audio.AudioItem.Artist).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Ascending;
                }
            }
            else if (v == "Duration")
            {
                if (e.Column.SortDirection == DataGridSortDirection.Ascending)
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderBy(audio => audio.AudioItem.Duration).ToList();
                    e.Column.SortDirection = DataGridSortDirection.Descending;
                }
                else
                {
                    _filteredPlayHistoryItems = _filteredPlayHistoryItems.OrderByDescending(audio => audio.AudioItem.Duration).ToList();
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

            UpdatePlayHistory();
        }

        internal async void FilterPlayHistory(string text)
        {
            List<PlayHistory> playHistoryList = await playHistoryDAO.GetUserPlayHistory(userDAO.SessionToken);

            _filteredPlayHistoryItems.Clear();

            foreach (var playHistory in playHistoryList)
            {
                var audio = await AudioDAO.Get().GetAudioById(playHistory.AudioId);
                var audioItem = new AudioItem(audio);
                var playHistoryItem = new PlayHistoryItem(audioItem, playHistory.PlayedAt);
                _filteredPlayHistoryItems.Add(playHistoryItem);
            }

            _filteredPlayHistoryItems = _filteredPlayHistoryItems
                .Where(item => item.AudioItem.Title.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                               item.AudioItem.Artist.Contains(text, StringComparison.OrdinalIgnoreCase))
                .ToList();
            MaxPage = (int)Math.Ceiling((double)_filteredPlayHistoryItems.Count / 8);
            CurrentPage = 1;
            UpdatePlayHistory();
        }

        internal void PaginationControl_PageChanged(PaginationControlValueChangedEventArgs e)
        {
            CurrentPage = e.NewValue;
        }

        internal async void EditProfile()
        {
            var user = userDAO.User;
            var token = userDAO.SessionToken;

            if (user == null || this.XamlRoot == null || String.IsNullOrEmpty(token))
            {
                return;
            }

            EditUserProfileDialog dialog = new EditUserProfileDialog(user)
            {
                XamlRoot = this.XamlRoot
            };


            ContentDialogResult result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Secondary)
            {
                return;
            }

            var inputUserProfile = dialog.GetUser();

            if (!ImageUtils.IsValidImageUrl(inputUserProfile.AvatarUrl))
            {
                dialog.Hide();
                await DialogUtils.ShowDialogAsync("Invalid URL", "Please enter a valid URL", this.XamlRoot);
                return;
            }

            try
            {
                await userDAO.UpdateUserProfile(token, inputUserProfile);
                SnackbarUtils.ShowSnackbar("Profile updated successfully", SnackbarType.Success);
            }
            catch (Exception ex)
            {
                SnackbarUtils.ShowSnackbar(ex.Message, SnackbarType.Error);
            }
        }
    }
}
