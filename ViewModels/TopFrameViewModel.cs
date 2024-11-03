using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WinUI3Localizer;

namespace OneLastSong.ViewModels
{

    public class TopFrameViewModel : INotifyPropertyChanged
    {
        private bool _isLanguageComboBoxInitialized = false;
        private bool _isThemeComboBoxInitialized = false;
        public String Language { get; set; }
        public String Theme { get; set; }
        public bool IsUserLoggedIn { get; set; }

        public XamlRoot XamlRoot { get; set; }

        public TopFrameViewModel()
        {
            Language = Localizer.Get().GetCurrentLanguage();
            Theme = ThemeUtils.GetStoredLocalTheme();
            IsUserLoggedIn = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void langComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isLanguageComboBoxInitialized)
            {
                _isLanguageComboBoxInitialized = true;
                return;
            }

            if (e.AddedItems.Count > 0)
            {
                var comboBoxItem = e.AddedItems[0] as ComboBoxItem;
                if (comboBoxItem != null)
                {
                    string selectedLanguage = comboBoxItem.Tag.ToString();
                    LogUtils.Debug($"Selected Language: {selectedLanguage}");
                    Localizer.Get().SetLanguage(selectedLanguage);
                }
            }
        }

        public async void themeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_isThemeComboBoxInitialized)
            {
                _isThemeComboBoxInitialized = true;
                return;
            }

            if (e.AddedItems.Count > 0)
            {
                var comboBoxItem = e.AddedItems[0] as ComboBoxItem;
                if (comboBoxItem != null && comboBoxItem.Tag.ToString() != ThemeUtils.GetCurrentTheme())
                {
                    string selectedTheme = comboBoxItem.Tag.ToString();
                    LogUtils.Debug($"Selected Theme: {selectedTheme}");
                    ThemeUtils.ChangeTheme(selectedTheme, true);


                    if (XamlRoot != null)
                    {
                        await DialogUtils.ShowDialogAsync("Theme Changed", $"Theme changed to {selectedTheme}. You will need to restart to see the changes!", XamlRoot);
                    }
                }
            }
        }

        //private async void changeThemeButton_Click(object sender, RoutedEventArgs e)
        //{
        //    var currentTheme = ThemeUtils.GetCurrentTheme();
        //    var newTheme = currentTheme == ThemeUtils.LIGHT_THEME ? ThemeUtils.DARK_THEME : ThemeUtils.LIGHT_THEME;
        //    ThemeUtils.ChangeTheme(newTheme, true);
        //    await DialogUtils.ShowDialogAsync("Theme Changed", $"Theme changed to {newTheme}. You will need to restart to see the changes!", XamlRoot);
        //}
    }
}
