using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Contracts
{
    public interface INotifyLikeAudioStateChanged
    {
        public void OnAnAudioLiked(int audioId);
        public void OnAnAudioLikeRemoved(int audioId);
    }
}
