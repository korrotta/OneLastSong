using OneLastSong.Models;
using OneLastSong.Services;
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

        private ListeningService listeningService;
        private NavigationService navigationService;

        public AudioExtraControlsFlyoutViewModel()
        {
            this.audio = null;
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
