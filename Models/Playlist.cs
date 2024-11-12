using System;
using System.Text.Json.Serialization;

namespace OneLastSong.Models
{
    public class Playlist
    {
        [JsonPropertyName("PlaylistId")]
        public int PlaylistId { get; set; }

        [JsonPropertyName("Name")]
        public string Name { get; set; }

        [JsonPropertyName("CoverImageUrl")]
        public string CoverImageUrl { get; set; }

        [JsonPropertyName("ItemCount")]
        public int ItemCount { get; set; }

        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}

