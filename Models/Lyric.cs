using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class Lyric
    {
        public int Id { get; set; }
        public int AudioId { get; set; }
        public float Timestamp { get; set; }
        public string LyricText { get; set; }
    }
}
