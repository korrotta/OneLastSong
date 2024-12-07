using OneLastSong.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class AudioItemInQueueMenuFlyoutViewModel
    {
        ListeningService _listeningService;

        private int _index;

        public AudioItemInQueueMenuFlyoutViewModel(int index)
        {
            _index = index;

            _listeningService = ListeningService.Get();
        }

        internal void PlayAudio()
        {
            _listeningService.PlayAudioInQueue(_index);
        }

        internal void RemoveAudioFromQueue()
        {
            _listeningService.RemoveAudioFromQueue(_index);
        }
    }
}
