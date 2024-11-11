using System;
using System.Text.Json.Serialization;

namespace OneLastSong.Models
{
    public class Album
    {
        [JsonPropertyName("AlbumId")]
        public int AlbumId { get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Artist")]
        public string Artist { get; set; }

        [JsonPropertyName("CoverImageUrl")]
        public string CoverImageUrl { get; set; }

        [JsonPropertyName("ReleaseDate")]
        public DateTime? ReleaseDate { get; set; }

        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("UserId")]
        public int? UserId { get; set; }

        [JsonPropertyName("ItemCount")]
        public int ItemCount { get; set; }
    }
}
