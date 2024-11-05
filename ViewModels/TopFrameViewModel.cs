using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using OneLastSong.Contracts;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.Views;
using OneLastSong.Views.Components;
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

    public class TopFrameViewModel : INotifyPropertyChanged, INavChangeNotifier
    {
        private bool _isLanguageComboBoxInitialized = false;
        private bool _isThemeComboBoxInitialized = false;
        public String Language { get; set; }
        public String Theme { get; set; }
        private bool _isUserLoggedIn;
        public NavigationService NavigationService { get; set; }
        public bool IsUserLoggedIn
        {
            get => _isUserLoggedIn;
            set
            {
                if (_isUserLoggedIn != value)
                {
                    _isUserLoggedIn = value;
                    OnPropertyChanged(nameof(IsUserLoggedIn));
                }
            }
        }
        private bool _canGoBack;
        public bool CanGoBack
        {
            get => _canGoBack;
            set
            {
                if (_canGoBack != value)
                {
                    _canGoBack = value;
                    OnPropertyChanged(nameof(CanGoBack));
                }
            }
        }

        private bool _canGoForward;
        public bool CanGoForward
        {
            get => _canGoForward;
            set
            {
                if (_canGoForward != value)
                {
                    _canGoForward = value;
                    OnPropertyChanged(nameof(CanGoForward));
                }
            }
        }

        private SolidColorBrush _goBackButtonColor;
        public SolidColorBrush GoBackButtonColor
        {
            get => _goBackButtonColor;
            set
            {
                if (_goBackButtonColor != value)
                {
                    _goBackButtonColor = value;
                    OnPropertyChanged(nameof(GoBackButtonColor));
                }
            }
        }

        private SolidColorBrush _goForwardButtonColor;
        public SolidColorBrush GoForwardButtonColor
        {
            get => _goForwardButtonColor;
            set
            {
                if (_goForwardButtonColor != value)
                {
                    _goForwardButtonColor = value;
                    OnPropertyChanged(nameof(GoForwardButtonColor));
                }
            }
        }

        private ResourceDictionary appRes = Application.Current.Resources;

        public XamlRoot XamlRoot { get; set; }

        public TopFrameViewModel()
        {
            Language = Localizer.Get().GetCurrentLanguage();
            Theme = ThemeUtils.GetStoredLocalTheme();
            IsUserLoggedIn = false;
            NavigationService = NavigationService.Get();

            GoBackButtonColor = GetBrush("TEXT_DISABLED");
            GoForwardButtonColor = GetBrush("TEXT_DISABLED");

            NavigationService.RegisterNavChangeNotifier(this);
        }

        private SolidColorBrush GetBrush(string color)
        {
            if (appRes.TryGetValue(color, out object brush))
            {
                return (SolidColorBrush)brush;
            }
            return null;
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

        public void homeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(HomePage));
        }

        public void Navigate(Type type)
        {
            NavigationService.Navigate(type);
        }

        public void prevPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        public void nextPageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoForward();
        }

        public void OnNavHistoryChanged(NavigationService navService)
        {
            CanGoBack = navService.CanGoBack;
            CanGoForward = navService.CanGoForward;

            if (CanGoBack)
            {
                GoBackButtonColor = GetBrush("TEXT_PRIMARY");
            }
            else
            {
                GoBackButtonColor = GetBrush("TEXT_DISABLED");
            }

            if (CanGoForward)
            {
                GoForwardButtonColor = GetBrush("TEXT_PRIMARY");
            }
            else
            {
                GoForwardButtonColor = GetBrush("TEXT_DISABLED");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Unregister on dispose
        public void Dispose()
        {
            NavigationService.UnregisterNavChangeNotifier(this);
        }
    }
}
