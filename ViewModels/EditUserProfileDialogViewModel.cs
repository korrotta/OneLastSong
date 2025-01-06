using OneLastSong.Models;
using OneLastSong.Utils;
using System.ComponentModel;

namespace OneLastSong.ViewModels
{
    public class EditUserProfileDialogViewModel : INotifyPropertyChanged
    {
        private User _user;
        private string _inputUrl;
        private string _previewUrl;

        public User User
        {
            get => _user;
            set
            {
                if (_user != value)
                {
                    _user = value;
                    InputUrl = User.AvatarUrl;
                    OnPropertyChanged(nameof(User));
                }
            }
        }

        public string InputUrl
        {
            get => _inputUrl;
            set
            {
                if (_inputUrl != value)
                {
                    _inputUrl = value;
                    User.AvatarUrl = value;
                    if (ImageUtils.IsValidImageUrl(value))
                    {
                        PreviewUrl = value;
                    }
                    else
                    {
                        PreviewUrl = ConfigValueUtils.GetConfigValue(ConfigValueUtils.NOT_FOUND_IMAGE_KEY);
                    }
                    OnPropertyChanged(nameof(InputUrl));
                }
            }
        }

        public string PreviewUrl
        {
            get => _previewUrl;
            set
            {
                if (_previewUrl != value)
                {
                    _previewUrl = value;
                    OnPropertyChanged(nameof(PreviewUrl));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}


