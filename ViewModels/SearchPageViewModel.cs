using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Contracts;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
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

        private string _currentCategory = ALL_CATEGORY;

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
            if(CurrentSearchQuery.Equals(newSearchQuery.Trim()))
            {
                return;
            }

            CurrentSearchQuery = newSearchQuery.Trim();

            LogUtils.Debug($"SearchPageViewModel received {newSearchQuery}");
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
