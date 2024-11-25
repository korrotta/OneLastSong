using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
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

        public readonly static string TEXT_PRIMARY = "TEXT_PRIMARY";
        public readonly static string TEXT_DISABLED = "TEXT_DISABLED";
        public readonly static string TEXT_CONTRAST1 = "TEXT_CONTRAST1";
        public readonly static string TEXT_LIGHT = "TEXT_LIGHT";

        public readonly static string INFO_BRUSH = "INFO_BRUSH";
        public readonly static string SUCCESS_BRUSH = "SUCCESS_BRUSH";
        public readonly static string WARNING_BRUSH = "WARNING_BRUSH";
        public readonly static string ERROR_BRUSH = "ERROR_BRUSH";

        public readonly static string BG_TERTIARY = "BG_TERTIARY";
        public readonly static string BG_CONTRAST1 = "BG_CONTRAST1";

        private static bool _isInitialized = false;

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

        // Currently we cannot change theme in runtime, so this method is not implemented
        public static void ChangeTheme(String themeKey, bool willStoreSetting)
        {
            throw new NotImplementedException();
            DoChangeTheme(themeKey);

            if (willStoreSetting)
            {
                SetStoredLocalTheme(themeKey);
            }
        }

        private static void DoChangeTheme(String themeKey)
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
            // Remove the current theme and add the new theme
            if (App.Current.Resources.ContainsKey(_currentTheme))
            {
                App.Current.Resources.Remove(_currentTheme);
            }

            if (!App.Current.Resources.MergedDictionaries.Contains(newTheme))
            {
                App.Current.Resources.MergedDictionaries.Add(newTheme);
            }

            // Update the current theme
            _currentTheme = themeKey;
        }

        // We need to restart the app to apply the new theme
        public static void ChangeTheme(String themeKey)
        {
            // Update the current theme
            _currentTheme = themeKey;

            SetStoredLocalTheme(themeKey);

            // Apply the theme when the app is initialized only
            if(_isInitialized)
            {
                return;
            }

            _isInitialized = true;
            DoChangeTheme(themeKey);
        }

        public static string GetCurrentTheme()
        {
            return _currentTheme;
        }

        public static void LoadStoredTheme()
        {
            string storedTheme = GetStoredLocalTheme();
            ChangeTheme(storedTheme);
        }

        public static SolidColorBrush GetBrush(string color)
        {
            if (App.Current.Resources.TryGetValue(color, out object brush))
            {
                return (SolidColorBrush)brush;
            }
            //return white brush if color not found
            return new SolidColorBrush(Microsoft.UI.Colors.White);
        }
    }
}