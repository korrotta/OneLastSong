using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class Audio : INotifyPropertyChanged, IComparable, IEquatable
    {
        private string _url;
        private int _likes;
        private string _title;
        private string _artist;
        private int? _albumId;
        private int _audioId;
        private int? _authorId;
        private int _duration;
        private DateTime _createdAt;
        private int? _categoryId;
        private string _description;
        private string _country;
        private List<string> _genres = new List<string>();
        private string _categoryName;
        private string _coverImageUrl;

        [JsonPropertyName("Url")]
        public string Url
        {
            get => _url;
            set
            {
                if (_url != value)
                {
                    _url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        [JsonPropertyName("Likes")]
        public int Likes
        {
            get => _likes;
            set
            {
                if (_likes != value)
                {
                    _likes = value;
                    OnPropertyChanged(nameof(Likes));
                }
            }
        }

        [JsonPropertyName("Title")]
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        [JsonPropertyName("Artist")]
        public string Artist
        {
            get => _artist;
            set
            {
                if (_artist != value)
                {
                    _artist = value;
                    OnPropertyChanged(nameof(Artist));
                }
            }
        }

        [JsonPropertyName("AlbumId")]
        public int? AlbumId
        {
            get => _albumId;
            set
            {
                if (_albumId != value)
                {
                    _albumId = value;
                    OnPropertyChanged(nameof(AlbumId));
                }
            }
        }

        [JsonPropertyName("AudioId")]
        public int AudioId
        {
            get => _audioId;
            set
            {
                if (_audioId != value)
                {
                    _audioId = value;
                    OnPropertyChanged(nameof(AudioId));
                }
            }
        }

        [JsonPropertyName("AuthorId")]
        public int? AuthorId
        {
            get => _authorId;
            set
            {
                if (_authorId != value)
                {
                    _authorId = value;
                    OnPropertyChanged(nameof(AuthorId));
                }
            }
        }

        [JsonPropertyName("Duration")]
        public int Duration
        {
            get => _duration;
            set
            {
                if (_duration != value)
                {
                    _duration = value;
                    OnPropertyChanged(nameof(Duration));
                }
            }
        }

        [JsonPropertyName("CreatedAt")]
        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                if (_createdAt != value)
                {
                    _createdAt = value;
                    OnPropertyChanged(nameof(CreatedAt));
                }
            }
        }

        [JsonPropertyName("CategoryId")]
        public int? CategoryId
        {
            get => _categoryId;
            set
            {
                if (_categoryId != value)
                {
                    _categoryId = value;
                    OnPropertyChanged(nameof(CategoryId));
                }
            }
        }

        [JsonPropertyName("Description")]
        public string Description
        {
            get => _description;
            set
            {
                if (_description != value)
                {
                    _description = value;
                    OnPropertyChanged(nameof(Description));
                }
            }
        }

        [JsonPropertyName("Country")]
        public string Country
        {
            get => _country;
            set
            {
                if (_country != value)
                {
                    _country = value;
                    OnPropertyChanged(nameof(Country));
                }
            }
        }

        [JsonPropertyName("Genres")]
        public List<string> Genres
        {
            get => _genres;
            set
            {
                if (_genres != value)
                {
                    _genres = value;
                    OnPropertyChanged(nameof(Genres));
                }
            }
        }

        [JsonPropertyName("CategoryName")]
        public string CategoryName
        {
            get => _categoryName;
            set
            {
                if (_categoryName != value)
                {
                    _categoryName = value;
                    OnPropertyChanged(nameof(CategoryName));
                }
            }
        }

        [JsonPropertyName("CoverImageUrl")]
        public string CoverImageUrl
        {
            get => _coverImageUrl;
            set
            {
                if (_coverImageUrl != value)
                {
                    _coverImageUrl = value;
                    OnPropertyChanged(nameof(CoverImageUrl));
                }
            }
        }

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
            Country = "Unknown",
            Genres = new List<string>(),
            CategoryName = "Unknown",
            Likes = 0
        };

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public int CompareTo(object obj)
        {
            return obj is Audio audio ? AudioId.CompareTo(audio.AudioId) : 1;
        }
    }
}
