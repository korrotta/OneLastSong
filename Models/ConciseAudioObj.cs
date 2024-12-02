using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class ConciseAudioObj
    {
        [JsonPropertyName("AudioId")]
        public int AudioId { get; set; }

        [JsonPropertyName("Title")]
        public string Title { get; set; }

        [JsonPropertyName("Artist")]
        public string Artist { get; set; }

        [JsonPropertyName("Genres")]
        public List<string> Genres { get; set; }

        [JsonPropertyName("CategoryName")]
        public string CategoryName { get; set; }

        [JsonPropertyName("Country")]
        public string Country { get; set; }

        public ConciseAudioObj(Audio audio)
        {
            AudioId = audio.AudioId;
            Title = audio.Title;
            Artist = audio.Artist;
            Genres = audio.Genres;
            CategoryName = audio.CategoryName;
            Country = audio.Country;
        }
    }
}
