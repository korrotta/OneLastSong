using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.DAOs;
using OneLastSong.Db;
using OneLastSong.ModelViews;
using OneLastSong.Utils;
using OneLastSong.Views.Components;
using System;
using System.Threading.Tasks;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
            TopFrame.Navigate(typeof(TopFrame));
            BodyFrame.Navigate(typeof(BodyFrame));
            BottomFrame.Navigate(typeof(BottomFrame));
            MainPageViewModel = new MainPageViewModel();
        }

        private async Task InitializeDatabase()
        {
            try
            {
                await _db.Connect();
                LogUtils.Debug("Database initialized successfully");
                ((App)Application.Current).Services.GetService<TestDAO>().Init();
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error initializing database: {ex.Message}");
            }
        }


    }
}
