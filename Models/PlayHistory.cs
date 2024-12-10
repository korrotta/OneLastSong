using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class PlayHistory
    {
        [JsonPropertyName("AudioId")]
        public int AudioId { get; set; }

        [JsonPropertyName("PlayedAt")]
        public DateTime PlayedAt { get; set; }
    }
}
