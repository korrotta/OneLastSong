using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using OneLastSong.Services;
using OneLastSong.ViewModels;
using OneLastSong.Views.Pages;
using System;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TopFrame : Page
    {
        public TopFrameViewModel TopFrameViewModel { get; set; } = new TopFrameViewModel();
        private NavigationService NavigationService { get; set; }
        public String AvatarUrl { get; set; }

        public TopFrame()
        {
            this.InitializeComponent();
            // Set the DataContext of the page to the ViewModel to allow for user login state data binding
            this.DataContext = TopFrameViewModel;
            NavigationService = NavigationService.Get();
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TopFrameViewModel.XamlRoot = this.XamlRoot;
        }

        private void SearchTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                TopFrameViewModel?.SearchCommand?.Execute(null);
            }
        }

        private void ProfileButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(ProfilePage));
        }
    }
}
