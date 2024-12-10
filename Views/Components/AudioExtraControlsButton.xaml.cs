using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.ComponentModel;
using OneLastSong.Models;
using OneLastSong.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    public sealed partial class AudioExtraControlsButton : UserControl, INotifyPropertyChanged
    {
        public AudioExtraControlsButton()
        {
            this.InitializeComponent();
            Audio = Audio.Default;
        }

        public Audio Audio
        {
            get { return (Audio)GetValue(AudioProperty); }
            set { SetValue(AudioProperty, value); }
        }

        public static readonly DependencyProperty AudioProperty =
            DependencyProperty.Register("Audio", typeof(Audio), typeof(AudioExtraControlsButton), new PropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //private void MoreControls_ButtonClicked(object sender, RoutedEventArgs e)
        //{
        //    // Might do something here
        //}
    }
}
