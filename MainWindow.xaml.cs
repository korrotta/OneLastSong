using Microsoft.UI.Xaml;
using System;

namespace OneLastSong
{
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.InitializeComponent();
        }

        public void NavigateMainFrameTo(Type pageType)
        {
            MainFrame.Navigate(pageType);
        }
    }
}
