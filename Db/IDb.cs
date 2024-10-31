using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Db
{
    public interface IDb
    {
        public Task<AudioData> GetAudioDataByIdAsync(string id);
        public Task Connect();
        public Task Dispose();
        public Task<string> DoTest();
    }    
}
