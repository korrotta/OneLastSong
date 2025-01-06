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
    public class HomePageViewModel : INotifyPropertyChanged, IAudioStateChanged, IDisposable, INotifyLikeAudioStateChanged
    {
        public ICommand PlayCommand { get; }
        public event PropertyChangedEventHandler PropertyChanged;
        
        private ListeningService listeningService;
        private NavigationService navigationService;
        private SidePanelNavigationService sidePanelNavigationService;
        private AudioService audioService;

        private AudioDAO audioDAO;
        private UserDAO userDAO;

        private AudioItem _selectedAudio;
        

        public HomePageViewModel()
        {
            listeningService = ListeningService.Get();
            navigationService = NavigationService.Get();
            sidePanelNavigationService = SidePanelNavigationService.Get();
            audioService = AudioService.Get();

            audioDAO = AudioDAO.Get();
            userDAO = UserDAO.Get();

            PlayCommand = new RelayCommand<Audio>(PlayAudio);
            listeningService.RegisterAudioStateChangeListeners(this);
            audioService.RegisterAudioLikeStateNotifier(this);
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
            ListAudios = new ObservableCollection<AudioItem>(audios.Select(a => new AudioItem(a)));
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
            audioService.UnregisterAudioLikeStateNotifier(this);
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

        public void OnAnAudioLiked(int audioId)
        {
            for (int i = 0; i < ListAudios.Count; i++)
            {
                if (ListAudios[i].AudioId == audioId)
                {
                    ListAudios[i].AudioLikeState = AudioItem.AudioLikeStateType.Liked;
                    ListAudios[i].Likes++;
                }
            }
        }

        public void OnAnAudioLikeRemoved(int audioId)
        {
            for (int i = 0; i < ListAudios.Count; i++)
            {
                if (ListAudios[i].AudioId == audioId)
                {
                    ListAudios[i].AudioLikeState = AudioItem.AudioLikeStateType.NotLiked;
                    ListAudios[i].Likes--;
                }
            }
        }

        internal async void HandleLikeButtonClick(int audioId)
        {
            AudioItem audioItem = ListAudios.FirstOrDefault(a => a.AudioId == audioId);

            string sessionToken = userDAO.SessionToken;

            if (audioItem == null || String.IsNullOrEmpty(sessionToken))
            {
                return;
            }

            try
            {
                if (audioItem.AudioLikeState == AudioItem.AudioLikeStateType.Liked)
                {
                    await audioDAO.RemoveLikeFromAudio(sessionToken, audioId);
                    SnackbarUtils.ShowSnackbar($"Removed like from audio \"{audioItem.Title}\" successfully!", SnackbarType.Success);
                }
                else
                {
                    await audioDAO.LikeAudio(sessionToken, audioId);
                    SnackbarUtils.ShowSnackbar($"Liked audio \"{audioItem.Title}\" successfully!", SnackbarType.Success);
                }
            }
            catch (Exception e)
            {
                SnackbarUtils.ShowSnackbar(e.Message, SnackbarType.Error);
            }
        }

        internal void NavigateToAds()
        {
            string url = $"https://yostar.store/products/azur-lane-plushie-4th-anniv";
            // Open the URL in the default browser
            BrowserUtils.OpenUrl(url);
            sidePanelNavigationService.Navigate(typeof(AdsPage));
        }
    }
}
