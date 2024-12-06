using Microsoft.UI.Xaml.Controls;
using OneLastSong.Models;
using OneLastSong.Utils;
using System.ComponentModel;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Dialogs
{
    public sealed partial class RatingAudioDialog : ContentDialog, INotifyPropertyChanged
    {
        Audio _audio = Audio.Default;

        public Audio Audio
        {
            get => _audio;
            set
            {
                _audio = value;
                OnPropertyChanged(nameof(Audio));
            }
        }

        float _score = 1f;
        public float Score
        {
            get => _score;
            set
            {
                _score = value;
                RatingLevel = RatingUtils.GetSentimentLevel(value);
                OnPropertyChanged(nameof(Score));
            }
        }

        string _ratingLevel = RatingUtils.GetSentimentLevel(1f);
        public string RatingLevel
        {
            get => _ratingLevel;
            set
            {
                _ratingLevel = value;
                OnPropertyChanged(nameof(RatingLevel));
            }
        }

        public RatingAudioDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Handle cancel action if needed
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void RatingControl_ValueChanged(RatingControl sender, object args)
        {
            Score = (float)sender.Value;
        }
    }
}
