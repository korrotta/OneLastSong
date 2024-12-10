using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Cores.Classes
{
    public class AudioRatingSummary
    {
        [JsonPropertyName("AverageRating")]
        public float AverageRating { get; set; }

        [JsonPropertyName("RatingCount")]
        public int RatingCount { get; set; }
    }
}
