using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Models;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.DAOs;
using OneLastSong.Db;
using OneLastSong.ModelViews;
using OneLastSong.Utils;
using OneLastSong.Views.Components;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUI3Localizer;

namespace OneLastSong.Views
{
    public sealed partial class MainPage : Page
    {
        private IDb _db;

        public MainPageViewModel MainPageViewModel { get; set; }

        string filepath = "D:\\HCMUS\\Windows Programming\\Project\\OneLastSong\\Assets\\samples\\Bazelgeuse.mp3";

        public MainPage()
        {
            this.InitializeComponent();
            this.Loaded += MainPage_Loaded;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
            await InitializeDatabase();
            BodyFrame.Navigate(typeof(BodyFrame));
            MainPageViewModel = new MainPageViewModel();
            DataContext = MainPageViewModel;

            _ = LoadSongAsync(filepath);
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

        private async void changeThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeUtils.GetCurrentTheme();
            var newTheme = currentTheme == ThemeUtils.LIGHT_THEME ? ThemeUtils.DARK_THEME : ThemeUtils.LIGHT_THEME;
            ThemeUtils.ChangeTheme(newTheme, true);
            await DialogUtils.ShowDialogAsync("Theme Changed", $"Theme changed to {newTheme}. You will need to restart to see the changes!", XamlRoot);
        }

        private async Task LoadSongAsync(string filepath)
        {
            try
            {
                var song = new Song();
                await song.loadMetadata(filepath);

                // Update the UI
                MainPageViewModel.CurrentSong = song;
            }
            catch (Exception ex)
            {
                await DialogUtils.ShowDialogAsync("Error", "Failed to load song: " + ex.Message, XamlRoot);
            }
        }
        //private void langComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    if (e.AddedItems.Count > 0)
        //    {
        //        string selectedLanguage = e.AddedItems[0].ToString();
        //        LogUtils.Debug($"Selected Language: {selectedLanguage}");
        //        Localizer.Get().SetLanguage(selectedLanguage);
        //    }
        //}

        //private async void changeThemeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var currentTheme = ThemeUtils.GetCurrentTheme();
        //    var newTheme = currentTheme == ThemeUtils.LIGHT_THEME ? ThemeUtils.DARK_THEME : ThemeUtils.LIGHT_THEME;
        //    ThemeUtils.ChangeTheme(newTheme, true);
        //    await DialogUtils.ShowDialogAsync("Theme Changed", $"Theme changed to {newTheme}. You will need to restart to see the changes!", XamlRoot);
        //}
    }
}
