using CommunityToolkit.Mvvm.Input;
using OneLastSong.Contracts;
using OneLastSong.Cores.AudioSystem;
using OneLastSong.Cores.DataItems;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.Views.Components;
using OneLastSong.Views.Pages;
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
    public class HomePageViewModel : INotifyPropertyChanged, IAudioStateChanged, IDisposable
    {
        public ICommand PlayCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        private ListeningService listeningService;
        private NavigationService navigationService;
        private AudioItem _selectedAudio;

        public HomePageViewModel()
        {
            listeningService = ListeningService.Get();
            navigationService = NavigationService.Get();
            PlayCommand = new RelayCommand<Audio>(PlayAudio);
            listeningService.RegisterAudioStateChangeListeners(this);
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private ObservableCollection<AudioItem> _listAudios = new ObservableCollection<AudioItem>();
        private ObservableCollection<Album> _listAlbums = new ObservableCollection<Album>();
        
        public async Task Load()
        {
            var audios = await AudioDAO.Get().GetMostLikeAudios();
            ListAudios = new ObservableCollection<AudioItem>(audios.Select(a => new AudioItem
            {
                AudioId = a.AudioId,
                Title = a.Title,
                Artist = a.Artist,
                AlbumId = a.AlbumId,
                AuthorId = a.AuthorId,
                Duration = a.Duration,
                CreatedAt = a.CreatedAt,
                CategoryId = a.CategoryId,
                Description = a.Description,
                CoverImageUrl = a.CoverImageUrl,
                Likes = a.Likes,
                Url = a.Url
            }));
            ListAlbums = new ObservableCollection<Album>(await AlbumDAO.Get().GetMostLikeAlbums());
            listeningService = ListeningService.Get();
            if (ListAudios.Count > 0)
            {
                SelectedAudio = ListAudios[0];
            }
        }

        public ObservableCollection<AudioItem> ListAudios
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

        public AudioItem SelectedAudio
        {
            get => _selectedAudio;
            set
            {
                if (_selectedAudio != value)
                {
                    _selectedAudio = value;
                    OnPropertyChanged(nameof(SelectedAudio));
                }
            }
        }

        public async void PlayAudio(Audio audio)
        {
            if(listeningService == null)
            {
                LogUtils.Error("Listening service is not initialized");
            }

            bool isTheCurrentAudio = await listeningService.PlayModeData.IsTheCurrentAudioFromMashup(audio.AudioId);

            // if the audio is the current audio, then change the play state
            if (isTheCurrentAudio)
            {
                listeningService.ChangePlayState();
                return;
            }

            listeningService.PlayAudio(audio);
        }

        public void OnAudioChanged(Audio newAudio)
        {
            if(newAudio == null)
            {
                return;
            }
            // set the corresponding audio item to be selected
            for (int i = 0; i < ListAudios.Count; i++)
            {
                if (ListAudios[i].AudioId == newAudio.AudioId)
                {
                    SelectedAudio = ListAudios[i];
                }
            }
        }

        public async void OnAudioPlayStateChanged(bool isPlaying)
        {
            for (int i = 0; i < ListAudios.Count; i++)
            {
                var isTheCurrentAudio = await listeningService.PlayModeData.IsTheCurrentAudioFromMashup(ListAudios[i].AudioId);
                if (isTheCurrentAudio)
                {
                    ListAudios[i].IsPlaying = isPlaying;
                }
                else
                {
                    ListAudios[i].IsPlaying = false;
                }
            }
        }

        public void OnAudioProgressChanged(int progress)
        {
            
        }

        public void Dispose()
        {
            listeningService.UnregisterAudioStateChangeListeners(this);
        }

        public void UpdateView()
        {
            OnAudioPlayStateChanged(listeningService.IsPlaying);
            OnAudioChanged(listeningService.PlayModeData.CurrentAudio);
        }

        internal void NavigateToAudioDetails(string audioId)
        {
            navigationService.NavigateOrReloadOnParameterChanged(typeof(AudioDetailsPage), audioId);
        }
    }
}
