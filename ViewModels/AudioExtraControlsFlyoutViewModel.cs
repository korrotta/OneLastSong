using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.Views.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class AudioExtraControlsFlyoutViewModel
    {
        private Audio audio;

        private AudioDAO _audioDAO;
        private PlaylistDAO _playlistDAO;
        private UserDAO _userDAO;

        private ListeningService listeningService;
        private NavigationService navigationService;

        public AudioExtraControlsFlyoutViewModel()
        {
            this.audio = null;

            _audioDAO = AudioDAO.Get();
            _playlistDAO = PlaylistDAO.Get();
            _userDAO = UserDAO.Get();

            listeningService = ListeningService.Get();
            navigationService = NavigationService.Get();
        }

        internal void AddToQueue()
        {
            if (audio == null)
            {
                return;
            }
            listeningService.AddToQueue(audio);
        }

        internal void GoToDetailsPage()
        {
            if(audio == null)
            {
                return;
            }

            navigationService.NavigateOrReloadOnParameterChanged(typeof(AudioDetailsPage), audio.AudioId.ToString());
        }

        internal async Task LikeOrDislikeCurrentAudio()
        {
            if (audio == null)
            {
                return;
            }

            string token = _userDAO.SessionToken;

            if(String.IsNullOrEmpty(token))
            {
                SnackbarUtils.ShowSnackbar("You need to login to like or dislike audio", SnackbarType.Error);
                return;
            }

            Playlist likePlaylist = await _playlistDAO.GetLikePlaylist(token);

            if (!likePlaylist.ContainsAudio(audio))
            {
                await _audioDAO.LikeAudio(token, audio.AudioId);
            }
            else
            {
                await _audioDAO.RemoveLikeFromAudio(token, audio.AudioId);
            }
        }

        internal void PlayAudio()
        {
            if(audio == null)
            {
                return;
            }

            listeningService.PlayAudio(audio);
        }

        internal void PlayNextAudio()
        {
            if(audio == null)
            {
                return;
            }

            listeningService.PlayNextAudio(audio);
        }

        internal void SetAudio(Audio value)
        {
            this.audio = value;
        }
    }
}
