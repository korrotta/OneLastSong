using CommunityToolkit.Mvvm.Input;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneLastSong.ViewModels
{
    public class HomePageViewModel : INotifyPropertyChanged
    {
        public ICommand PlayCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        private ListeningService listeningService = null;
        
        public HomePageViewModel()
        {
            PlayCommand = new RelayCommand<Audio>(PlayAudio);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<Audio> _listAudios = new ObservableCollection<Audio>();
        private ObservableCollection<Album> _listAlbums = new ObservableCollection<Album>();
        
        public async Task Load()
        {
            ListAudios = new ObservableCollection<Audio>(await AudioDAO.Get().GetMostLikeAudios());
            ListAlbums = new ObservableCollection<Album>(await AlbumDAO.Get().GetMostLikeAlbums());
            listeningService = ListeningService.Get();
        }

        public ObservableCollection<Audio> ListAudios
        {
            get => _listAudios;
            set
            {
                if (_listAudios != value)
                {
                    _listAudios = value;
                    OnPropertyChanged(nameof(ListAudios));
                }
            }
        }

        public ObservableCollection<Album> ListAlbums
        {
            get => _listAlbums;
            set
            {
                if (_listAlbums != value)
                {
                    _listAlbums = value;
                    OnPropertyChanged(nameof(ListAlbums));
                }
            }
        }

        public void PlayAudio(Audio audio)
        {
            if(listeningService == null)
            {
                LogUtils.Error("Listening service is not initialized");
            }

            listeningService.PlayAudio(audio);
        }
    }
}
