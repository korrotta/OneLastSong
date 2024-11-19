﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class Audio : INotifyPropertyChanged
    {
        [JsonPropertyName("Url")]
        public string Url { get; set; }

        [JsonPropertyName("Likes")]
        public int Likes { get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Artist")]
        public string Artist { get; set; }

        [JsonPropertyName("AlbumId")]
        public int? AlbumId { get; set; }

        [JsonPropertyName("AudioId")]
        public int AudioId { get; set; }

        [JsonPropertyName("AuthorId")]
        public int? AuthorId { get; set; }

        [JsonPropertyName("Duration")]
        public int Duration { get; set; }

        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("CategoryId")]
        public int? CategoryId { get; set; }

        [JsonPropertyName("Description")]
        public string Description { get; set; }

        [JsonPropertyName("CoverImageUrl")]
        public string CoverImageUrl { get; set; }

        public static readonly Audio Default = new Audio
        {
            AudioId = -1,
            Title = "Unknown",
            Artist = "Unknown",
            AlbumId = -1,
            AuthorId = -1,
            Duration = 0,
            CreatedAt = DateTime.MinValue,
            CategoryId = -1,
            Description = "Unknown",
            CoverImageUrl = "Unknown",
            Url = "Unknown",
            Likes = 0
        };

        public event PropertyChangedEventHandler PropertyChanged;
    }
}