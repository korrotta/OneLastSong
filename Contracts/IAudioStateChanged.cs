using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Contracts
{
    public interface IAudioStateChanged
    {
        public void OnAudioChanged(Audio audio);
        public void OnAudioPlayStateChanged(bool isPlaying);
        public void OnAudioProgressChanged(int progress);
    }
}
