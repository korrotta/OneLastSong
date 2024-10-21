using OneLastSong.Models;
using System.ComponentModel;
using WinUI3Localizer;

namespace OneLastSong.ModelViews
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Language { get; set; }
        private Song _currentSong;

        public Song CurrentSong
        {
            get => _currentSong;
            set
            {
                if (_currentSong != value)
                {
                    _currentSong = value;
                    OnPropertyChanged(nameof(CurrentSong));
                }
            }
        }

        public MainPageViewModel()
        {
            Language = Localizer.Get().GetCurrentLanguage();

            CurrentSong = new Song();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
