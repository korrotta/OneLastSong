using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Db
{
    public interface IAudioDb
    {
        Task<AudioData> GetAudioDataByIdAsync(string id);
    }
}
