using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace OneLastSong
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
            MainFrame.Navigate(typeof(LoginPage));
        }
    }
}
