using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class PlayingSession
    {
        public int AudioId { get; set; }
        public TimeSpan Progress { get; set; }
        public bool IsPlaying { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
