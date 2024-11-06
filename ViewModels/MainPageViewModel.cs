using System.ComponentModel;
using WinUI3Localizer;

namespace OneLastSong.ModelViews
{
    public class MainPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public string Language { get; set; }

        public MainPageViewModel()
        {
            Language = Localizer.Get().GetCurrentLanguage();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
