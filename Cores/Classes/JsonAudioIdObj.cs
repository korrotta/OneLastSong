using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Cores.Classes
{
    public class JsonAudioIdObj
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }
    }
}
