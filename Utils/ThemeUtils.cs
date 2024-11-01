using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace OneLastSong.Utils
{
    public class ThemeUtils
    {
        public readonly static string THEME_KEY = "AppTheme";
        public static readonly string DARK_THEME = "Dark";
        public static readonly string LIGHT_THEME = "Light";
        private static string _currentTheme = DARK_THEME;

        public static string GetStoredLocalTheme()
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                if (localSettings.Values.ContainsKey(THEME_KEY))
                {
                    return localSettings.Values[THEME_KEY].ToString();
                }
                SetStoredLocalTheme(_currentTheme);
                return _currentTheme; // Default theme
            }
            catch (Exception ex)
            {
                SetStoredLocalTheme(_currentTheme);
                // Handle exceptions (e.g., file not found)
                return _currentTheme; // Default theme
            }
        }

        private static void SetStoredLocalTheme(string theme)
        {
            try
            {
                var localSettings = ApplicationData.Current.LocalSettings;
                localSettings.Values[THEME_KEY] = theme;
            }
            catch (Exception ex)
            {
                // Handle exceptions (e.g., file not found)
            }
        }

        public static void ChangeTheme(String themeKey, bool willStoreSetting = false)
        {
            // Retrieve the theme dictionary from the application's resources
            ResourceDictionary originalTheme = App.Current.Resources.ThemeDictionaries[themeKey] as ResourceDictionary;

            if (originalTheme == null)
            {
                // Log or handle the case where the theme is not found
                LogUtils.Debug("Theme not found");
                return;
            }

            // Create a new ResourceDictionary and copy the resources from the original theme
            ResourceDictionary newTheme = new ResourceDictionary();
            foreach (var key in originalTheme.Keys)
            {
                newTheme[key] = originalTheme[key];
            }

            // Clear the current resources and add the new theme
            App.Current.Resources.Clear();
            App.Current.Resources.MergedDictionaries.Add(newTheme);
            _currentTheme = themeKey;

            if (willStoreSetting)
            {
                SetStoredLocalTheme(themeKey);
            }
        }

        public static string GetCurrentTheme()
        {
            return _currentTheme;
        }

        public static void LoadStoredTheme()
        {
            string storedTheme = GetStoredLocalTheme();
            ChangeTheme(storedTheme, true);
        }
    }
}