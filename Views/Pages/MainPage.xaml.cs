using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.ModelViews;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.Views.Components;
using System;
using System.Threading.Tasks;

namespace OneLastSong.Views
{
    public sealed partial class MainPage : Page, IAuthChangeNotify, IDisposable
    {
        public MainPageViewModel MainPageViewModel { get; set; }
        private NavigationService NavigationService { get; set; }
        private AuthService AuthService { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;

            // Extend content into the title bar
            var window = (Application.Current as App).MainWindow;
            window.ExtendsContentIntoTitleBar = true;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            MainPageViewModel = new MainPageViewModel();
            TopFrame.Navigate(typeof(TopFrame));
            BodyFrame.Navigate(typeof(BodyFrame));
            BottomFrame.Navigate(typeof(BottomFrame));

            (TopFrame.Content as TopFrame)?.TopFrameViewModel.SubscribeToSearchEvent(OnSearchEventTrigger);

            NavigationService = NavigationService.Get();
            AuthService = AuthService.Get();
            AuthService.RegisterAuthChangeNotify(this);
            // Update user
            AuthService.OnComponentsLoaded();
        }

        public async void OnUserChange(User user)
        {
            if (user == null)
            {
                SnackbarUtils.ShowSnackbar("User logged out", SnackbarType.Info);
                NavigationService.Navigate(typeof(SignInPage));
                NavigationService.ClearHistory();
                return;
            }

            NavigationService.Navigate(typeof(HomePage));
            NavigationService.ClearHistory();

            await DialogUtils.ShowDialogAsync("Welcome", $"Welcome {user.Username}!", XamlRoot);
        }

        private void OnSearchEventTrigger(object sender, string newSearchQuery)
        {
            SearchPage searchPage = NavigationService.GetCurrentPage() as SearchPage;
           
            if(searchPage != null)
            {
                searchPage.SearchPageViewModel.OnSearchEventTrigger(newSearchQuery);
            }
            else
            {
                NavigationService.Navigate(typeof(SearchPage), newSearchQuery);
            }
        }

        public void Dispose()
        {
            AuthService.Get().UnregisterAuthChangeNotify(this);
        }
    }
}
