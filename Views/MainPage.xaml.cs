using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.ModelViews;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPageViewModel MainPageViewModel { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            MainPageViewModel = new MainPageViewModel();
        }

        private void langComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                string selectedLanguage = e.AddedItems[0].ToString();
                LogUtils.Debug($"Selected Language: {selectedLanguage}");
                Localizer.Get().SetLanguage(selectedLanguage);
            }
        }

        private async void changeThemeButton_Click(object sender, RoutedEventArgs e)
        {
            var currentTheme = ThemeUtils.GetCurrentTheme();
            var newTheme = currentTheme == ThemeUtils.LIGHT_THEME ? ThemeUtils.DARK_THEME : ThemeUtils.LIGHT_THEME;
            ThemeUtils.ChangeTheme(newTheme, true);
            await DialogUtils.ShowDialogAsync("Theme Changed", $"Theme changed to {newTheme}. You will need to restart to see the changes!", XamlRoot);
        }
    }
}
