using Microsoft.UI.Xaml;

namespace OneLastSong
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            MainFrame.Navigate(typeof(Views.MainPage));
        }
    }
}
