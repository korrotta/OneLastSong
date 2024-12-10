using System;
using System.Text.Json.Serialization;

namespace OneLastSong.Models
{
    public class Comment
    {
        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("UserId")]
        public int UserId { get; set; }

        [JsonPropertyName("AudioId")]
        public int AudioId { get; set; }

        [JsonPropertyName("CommentText")]
        public string CommentText { get; set; }

        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; }
    }
}