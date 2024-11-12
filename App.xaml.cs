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

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            Services = ConfigureServices();
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
            // Services
            services.AddSingleton<NavigationService>();
            services.AddSingleton<AuthService>();

            return services.BuildServiceProvider();
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
