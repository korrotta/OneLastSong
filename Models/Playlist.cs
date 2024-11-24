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

        [JsonPropertyName("Audios")]
        public Audio[] Audios { get; set; }

        [JsonPropertyName("Deletable")]
        bool Deletable { get; set; }

        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        public bool ContainsAudio(int audioId)
        {
            if(Audios == null)
            {
                return false;
            }

            foreach (var audio in Audios)
            {
                if (audio.AudioId == audioId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}

