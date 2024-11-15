using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.ModelViews;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.Views.Components;
using System;
using System.Threading.Tasks;

namespace OneLastSong.Views
{
    public sealed partial class MainPage : Page
    {
        private IDb _db;

        public MainPageViewModel MainPageViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;

            // Extend content into the title bar
            var window = (Application.Current as App).MainWindow;
            window.ExtendsContentIntoTitleBar = true;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
            await InitializeDatabase();
            MainPageViewModel = new MainPageViewModel();
            TopFrame.Navigate(typeof(TopFrame));
            BodyFrame.Navigate(typeof(BodyFrame));
            BottomFrame.Navigate(typeof(BottomFrame));

            (TopFrame.Content as TopFrame)?.TopFrameViewModel.SubscribeToSearchEvent(OnSearchEventTrigger);

            await OnServiceInitialized();
        }

        private void OnSearchEventTrigger(object sender, string newSearchQuery)
        {
            SearchPage searchPage = NavigationService.Get().Navigate(typeof(SearchPage), newSearchQuery) as SearchPage;
           
            if(searchPage != null)
            {
                searchPage.SearchPageViewModel.OnSearchEventTrigger(newSearchQuery);
            }
        }

        private async Task OnServiceInitialized()
        {
            //this line is for testing purposes only
            //await DoDbTest();
            await AuthService.Get().TryToLoadStoredToken();
        }

        private async Task DoDbTest()
        {
            var testDAO = ((App)Application.Current).Services.GetService<TestDAO>();
            var res = await testDAO.Test();
            await DialogUtils.ShowDialogAsync("Testing db", res, XamlRoot);
        }

        private async Task InitializeDatabase()
        {
            try
            {
                await _db.Connect();
                LogUtils.Debug("Database initialized successfully");
                ((App)Application.Current).Services.GetService<TestDAO>().Init();
                ((App)Application.Current).Services.GetService<UserDAO>().Init();
                ((App)Application.Current).Services.GetService<AudioDAO>().Init();
                ((App)Application.Current).Services.GetService<AlbumDAO>().Init();
                ((App)Application.Current).Services.GetService<PlaylistDAO>().Init();
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error initializing database: {ex.Message}");
            }
        }


    }
}
