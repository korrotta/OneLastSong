using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;

namespace OneLastSong.Models
{
    public class User : INotifyPropertyChanged, ICloneable
    {
        private int _id;
        private string _username;
        private DateTime _createdAt;
        private string _avatarUrl;
        private string _profileQuote;
        private string _description;
        private bool _isArtist;

        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                if (_username != value)
                {
                    _username = value;
                    OnPropertyChanged(nameof(Username));
                }
            }
        }

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

        public string AvatarUrl
        {
            get => _avatarUrl;
            set
            {
                if (_avatarUrl != value)
                {
                    _avatarUrl = value;
                    OnPropertyChanged(nameof(AvatarUrl));
                }
            }
        }

        public string ProfileQuote
        {
            get => _profileQuote;
            set
            {
                if (_profileQuote != value)
                {
                    _profileQuote = value;
                    OnPropertyChanged(nameof(ProfileQuote));
                }
            }
        }

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

        public bool IsArtist
        {
            get => _isArtist;
            set
            {
                if (_isArtist != value)
                {
                    _isArtist = value;
                    OnPropertyChanged(nameof(IsArtist));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public static User FromJson(string json)
        {
            return JsonSerializer.Deserialize<User>(json);
        }

        public object Clone()
        {
            return new User
            {
                Id = Id,
                Username = Username,
                CreatedAt = CreatedAt,
                AvatarUrl = AvatarUrl,
                ProfileQuote = ProfileQuote,
                Description = Description,
                IsArtist = IsArtist
            };
        }
    }
}
