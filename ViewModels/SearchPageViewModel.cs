using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Contracts;
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
using static OneLastSong.Views.SearchPage;

namespace OneLastSong.ViewModels
{
    public class SearchPageViewModel : INotifyPropertyChanged
    {
        private string _currentSearchQuery = "";

        public static readonly string ALL_CATEGORY = "ALL_CATEGORY";
        public static readonly string SONG_CATEGORY = "SONG_CATEGORY";
        public static readonly string ARTIST_CATEGORY = "ARTIST_CATEGORY";
        public static readonly string ALBUM_CATEGORY = "ALBUM_CATEGORY";

        private bool _isInitialized = false;

        private ObservableCollection<Audio> _filteredListAudios;
        public ObservableCollection<Audio> FilteredListAudios
        {
            get => _filteredListAudios;
            set
            {
                if (_filteredListAudios != value)
                {
                    _filteredListAudios = value;
                    OnPropertyChanged(nameof(FilteredListAudios));
                }
            }
        }
        public ObservableCollection<Album> _filteredListAlbum;
        public ObservableCollection<Album> FilteredListAlbums
        {
            get => _filteredListAlbum;
            set
            {
                if (_filteredListAlbum != value)
                {
                    _filteredListAlbum = value;
                    OnPropertyChanged(nameof(FilteredListAlbums));
                }
            }
        }

        private ObservableCollection<Audio> _listAudios;
        public ObservableCollection<Audio> ListAudios
        {
            get => _listAudios;
            set
            {
                if (_listAudios != value)
                {
                    _listAudios = value;
                    FilteredListAudios = new ObservableCollection<Audio>(value);
                    OnPropertyChanged(nameof(ListAudios));
                }
            }
        }

        private ObservableCollection<Album> _listAlbums;
        public ObservableCollection<Album> ListAlbums
        {
            get => _listAlbums;
            set
            {
                if (_listAlbums != value)
                {
                    _listAlbums = value;
                    FilteredListAlbums = new ObservableCollection<Album>(value);
                    OnPropertyChanged(nameof(ListAlbums));
                }
            }
        }

        private string _currentCategory = ALL_CATEGORY;
        public async void Init()
        {
            List<Audio> audios = await AudioDAO.Get().GetMostLikeAudios();
            List<Album> albums = await AlbumDAO.Get().GetMostLikeAlbums();

            ListAudios = new ObservableCollection<Audio>(audios);
            ListAlbums = new ObservableCollection<Album>(albums);

            _isInitialized = true;
        }

        public String CurrentCategory
        {
            get => _currentCategory;
            set
            {
                if (_currentCategory != value)
                {
                    _currentCategory = value;
                    OnPropertyChanged(nameof(CurrentCategory));
                }
            }
        }

        public String CurrentSearchQuery
        {
            get => _currentSearchQuery;
            set
            {
                if (_currentSearchQuery != value)
                {
                    _currentSearchQuery = value;
                    OnPropertyChanged(nameof(CurrentSearchQuery));
                }
            }
        }

        public SearchPageViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnSearchEventTrigger(string newSearchQuery)
        {
            if (!_isInitialized)
            {
                Init();
            }

            CurrentSearchQuery = newSearchQuery.Trim();
            FilteredListAudios = new ObservableCollection<Audio>(FilterAudioByTitle(ListAudios.ToList(), CurrentSearchQuery));
            FilteredListAlbums = new ObservableCollection<Album>(FilterAlbumByTitle(ListAlbums.ToList(), CurrentSearchQuery));
        }

        public List<Audio> FilterAudioByTitle(List<Audio> audios, string searchQuery)
        {
            return audios.Where(audio => audio.Title.ToLower().Contains(searchQuery.ToLower())).ToList();
        }

        public List<Album> FilterAlbumByTitle(List<Album> albums, string searchQuery)
        {
            return albums.Where(album => album.Title.ToLower().Contains(searchQuery.ToLower())).ToList();
        }

        public bool IsAllSelected
        {
            get => CurrentCategory == ALL_CATEGORY;
            set
            {
                if (value)
                {
                    CurrentCategory = ALL_CATEGORY;
                    OnPropertyChanged(nameof(IsAllSelected));
                    OnPropertyChanged(nameof(IsSongSelected));
                    OnPropertyChanged(nameof(IsArtistSelected));
                    OnPropertyChanged(nameof(IsAlbumSelected));
                }
            }
        }

        public bool IsSongSelected
        {
            get => CurrentCategory == SONG_CATEGORY;
            set
            {
                if (value)
                {
                    CurrentCategory = SONG_CATEGORY;
                    OnPropertyChanged(nameof(IsAllSelected));
                    OnPropertyChanged(nameof(IsSongSelected));
                    OnPropertyChanged(nameof(IsArtistSelected));
                    OnPropertyChanged(nameof(IsAlbumSelected));
                }
            }
        }

        public bool IsArtistSelected
        {
            get => CurrentCategory == ARTIST_CATEGORY;
            set
            {
                if (value)
                {
                    CurrentCategory = ARTIST_CATEGORY;
                    OnPropertyChanged(nameof(IsAllSelected));
                    OnPropertyChanged(nameof(IsSongSelected));
                    OnPropertyChanged(nameof(IsArtistSelected));
                    OnPropertyChanged(nameof(IsAlbumSelected));
                }
            }
        }

        public bool IsAlbumSelected
        {
            get => CurrentCategory == ALBUM_CATEGORY;
            set
            {
                if (value)
                {
                    CurrentCategory = ALBUM_CATEGORY;
                    OnPropertyChanged(nameof(IsAllSelected));
                    OnPropertyChanged(nameof(IsSongSelected));
                    OnPropertyChanged(nameof(IsArtistSelected));
                    OnPropertyChanged(nameof(IsAlbumSelected));
                }
            }
        }

        public void OnCategorySelected(object sender, RoutedEventArgs e)
        {
            LogUtils.Debug($"CurrentCategory selected: {((UserControl)sender).Tag}");
            CurrentCategory = ((UserControl)sender).Tag.ToString();
            OnPropertyChanged(nameof(IsAllSelected));
            OnPropertyChanged(nameof(IsSongSelected));
            OnPropertyChanged(nameof(IsArtistSelected));
            OnPropertyChanged(nameof(IsAlbumSelected));
        }
    }
}
