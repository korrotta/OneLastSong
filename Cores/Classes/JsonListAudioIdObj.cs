using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Cores.Classes
{
    public class JsonListAudioIdObj
    {
        [JsonPropertyName("Ids")]
        public List<int> Ids { get; set; }
    }
}
