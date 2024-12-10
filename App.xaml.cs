using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Extensions.DependencyInjection;
using OneLastSong.Db;
using OneLastSong.DAOs;
using OneLastSong.Views;
using OneLastSong.Services;
using OneLastSong.Contracts;
using System.Threading;
using Microsoft.UI.Dispatching;
using System.Threading.Tasks;
using OpenAI;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public IServiceProvider Services { get; }

        private IDb _db;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.UnhandledException += OnUnhandledException;
            Services = ConfigureServices();
        }

        private void OnUnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            // Log the exception and ignore it
            LogUtils.Debug($"Unhandled exception: {e.Exception.Message}");
            e.Handled = true;
        }

        private static IServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Db
            services.AddSingleton<IDb, PgDb>();
            // DAOs
            services.AddSingleton<TestDAO>();
            services.AddSingleton<UserDAO>();
            services.AddSingleton<AudioDAO>();
            services.AddSingleton<AlbumDAO>();
            services.AddSingleton<PlaylistDAO>();
            services.AddSingleton<ListeningSessionDAO>();
            services.AddSingleton<LyricDAO>();
            services.AddSingleton<RatingDAO>();
            services.AddSingleton<CommentDAO>();
            services.AddSingleton<PlayHistoryDAO>();
            // Services
            var dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            services.AddSingleton<NavigationService>(provider => new NavigationService(dispatcherQueue));
            services.AddSingleton<SidePanelNavigationService>(provider => new SidePanelNavigationService(dispatcherQueue));
            services.AddSingleton<AuthService>(provider => new AuthService(dispatcherQueue));
            services.AddSingleton<ListeningService>(provider => new ListeningService(dispatcherQueue));
            services.AddSingleton<PlaylistService>(provider => new PlaylistService(dispatcherQueue));            
            services.AddSingleton<AIService>(provider => new AIService(dispatcherQueue)); // OpenAI
            services.AddSingleton<PlayHistoryService>(provider => new PlayHistoryService(dispatcherQueue));

            return services.BuildServiceProvider();
        }

        private async Task OnServiceInitialized()
        {
            //this line is for testing purposes only
            //await DoDbTest();
            await AuthService.Get().OnSubsystemInitialized();
            await ListeningService.Get().OnSubsystemInitialized();
            await PlaylistService.Get().OnSubsystemInitialized();
            await NavigationService.Get().OnSubsystemInitialized();
            await SidePanelNavigationService.Get().OnSubsystemInitialized();
            await AIService.Get().OnSubsystemInitialized(); // OpenAI
            await PlayHistoryService.Get().OnSubsystemInitialized();
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
                ((App)Application.Current).Services.GetService<ListeningSessionDAO>().Init();
                ((App)Application.Current).Services.GetService<LyricDAO>().Init();
                ((App)Application.Current).Services.GetService<RatingDAO>().Init();
                ((App)Application.Current).Services.GetService<CommentDAO>().Init();
                ((App)Application.Current).Services.GetService<PlayHistoryDAO>().Init();
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error initializing database: {ex.Message}");
                SnackbarUtils.ShowSnackbar("Cannot connect to the database :(", SnackbarType.Error, 3);
                // Close the app after 3 seconds
                await Task.Delay(3000);
                // Close the app
                Application.Current.Exit();
                // throw new InvalidOperationException("Connection failed", e);
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected async override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            await LocalizationUtils.InitializeLocalizer();
            //this line is for testing purposes only
            //ThemeUtils.ChangeTheme(ThemeUtils.GetStoredLocalTheme());
            //ThemeUtils.ChangeTheme(ThemeUtils.DARK_THEME, true); //uncomment this line to set the default theme (dark theme
            ThemeUtils.LoadStoredTheme(); //uncomment this line to load the stored theme

            var mainWindow = new MainWindow();
            _window = mainWindow;
            _window.Activate();

            _db = ((App)Application.Current).Services.GetService<IDb>();
            await InitializeDatabase();
            await OnServiceInitialized();
            mainWindow.NavigateMainFrameTo(typeof(MainPage));
        }

        private MainWindow _window;

        public MainWindow MainWindow
        {
            get
            {
                return _window;
            }
        }
    }
}
