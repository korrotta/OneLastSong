using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class UserAudioRating
    {
        [JsonPropertyName("UserId")]
        public int UserId { get; set; }

        [JsonPropertyName("AudioId")]
        public int AudioId { get; set; }

        [JsonPropertyName("Rating")]
        public float Rating { get; set; }

        [JsonPropertyName("RatedAt")]
        public DateTime RatedAt { get; set; }
    }
}
