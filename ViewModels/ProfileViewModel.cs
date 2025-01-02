using OneLastSong.DAOs;
using OneLastSong.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace OneLastSong.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        UserDAO userDAO = UserDAO.Get();
        private string _username;
        private string _profilePictureUrl;
        private string _description;
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

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged(nameof(Description));
            }
        }

        public ProfileViewModel()
        {
            Username = userDAO.User.Username;
            ProfilePictureUrl = userDAO.User.AvatarUrl;
            Description = userDAO.User.Description;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
