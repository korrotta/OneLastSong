using OneLastSong.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OneLastSong.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _username;
        private string _profilePictureUrl;
        private int _followerCount;
        private ObservableCollection<Playlist> _playlists;

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string ProfilePictureUrl
        {
            get => _profilePictureUrl;
            set
            {
                _profilePictureUrl = value;
                OnPropertyChanged(nameof(ProfilePictureUrl));
            }
        }

        public int FollowerCount
        {
            get => _followerCount;
            set
            {
                _followerCount = value;
                OnPropertyChanged(nameof(FollowerCount));
            }
        }

        public ProfileViewModel()
        {
            // Simulate loading user profile data

            Username = "Tent";
            ProfilePictureUrl = "https://via.placeholder.com/120";
            FollowerCount = 1200;

        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
