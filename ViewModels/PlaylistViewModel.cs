using Microsoft.UI.Xaml;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
using System.ComponentModel;

namespace OneLastSong.ViewModels
{
    public class PlaylistViewModel : INotifyPropertyChanged
    {
        public NavigationService NavigationService { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;
        public XamlRoot XamlRoot { get; set; }

        public PlaylistViewModel()
        {
            NavigationService = NavigationService.Get();
        }

        private Playlist _selectedPlaylist;
        public Playlist SelectedPlaylist
        {
            get => _selectedPlaylist;
            set
            {
                _selectedPlaylist = value;
                OnPropertyChanged(nameof(SelectedPlaylist));
            }
        }

        public void LoadPlaylist(Playlist playlist)
        {
            try
            {
                SelectedPlaylist = playlist;
            }
            catch (System.Exception ex)
            {
                if (XamlRoot == null)
                {
                    LogUtils.Error("XamlRoot is null");
                    return;
                }
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}