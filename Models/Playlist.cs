using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace OneLastSong.Models
{
    public class Playlist : INotifyPropertyChanged, IComparable
    {
        private int _playlistId;
        private string _name;
        private string _coverImageUrl;
        private int _itemCount;
        private Audio[] _audios;
        private bool _deletable;
        private DateTime _createdAt;

        [JsonPropertyName("PlaylistId")]
        public int PlaylistId
        {
            get => _playlistId;
            set
            {
                if (_playlistId != value)
                {
                    _playlistId = value;
                    OnPropertyChanged(nameof(PlaylistId));
                }
            }
        }

        [JsonPropertyName("Name")]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(nameof(Name));
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

        [JsonPropertyName("ItemCount")]
        public int ItemCount
        {
            get => _itemCount;
            set
            {
                if (_itemCount != value)
                {
                    _itemCount = value;
                    OnPropertyChanged(nameof(ItemCount));
                }
            }
        }

        [JsonPropertyName("Audios")]
        public Audio[] Audios
        {
            get => _audios;
            set
            {
                if (_audios != value)
                {
                    _audios = value;
                    OnPropertyChanged(nameof(Audios));
                }
            }
        }

        [JsonPropertyName("Deletable")]
        public bool Deletable
        {
            get => _deletable;
            set
            {
                if (_deletable != value)
                {
                    _deletable = value;
                    OnPropertyChanged(nameof(Deletable));
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool ContainsAudio(int audioId)
        {
            if (Audios == null)
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

        public int CompareTo(object obj)
        {
            return obj is Playlist playlist ? PlaylistId.CompareTo(playlist.PlaylistId) : 1;
        }
    }
}
