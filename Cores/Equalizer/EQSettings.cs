using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Cores.Equalizer
{
    public class EQSettings
    {
        public int AudioId { get; set; }
        public List<EQBand> Bands { get; set; }
    }
}
