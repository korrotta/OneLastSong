using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Models;
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
using System.Windows.Input;
using WinUI3Localizer;

namespace OneLastSong.ViewModels
{
    [Serializable]
    public class TopFrameViewModel : INotifyPropertyChanged, INavChangeNotifier, IAuthChangeNotify
    {
        private bool _isLanguageComboBoxInitialized = false;
        private bool _isThemeComboBoxInitialized = false;
        public String Language { get; set; }
        public String Theme { get; set; }
        private string _searchQuery = "";
        private bool _isUserLoggedIn;
        public NavigationService NavigationService { get; set; }
        public User User { get; private set; } = new User();
        public ICommand NavigateToSignUpPageCommand { get; }
        public ICommand NavigateToSignInPageCommand { get; }
        public ICommand LogoutCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand GoToAIPageCommand { get; }

        public TopFrameViewModel()
        {
            Language = Localizer.Get().GetCurrentLanguage();
            Theme = ThemeUtils.GetStoredLocalTheme();
            IsUserLoggedIn = false;
            NavigationService = NavigationService.Get();

            GoBackButtonColor = ThemeUtils.GetBrush(ThemeUtils.TEXT_DISABLED);
            GoForwardButtonColor = ThemeUtils.GetBrush(ThemeUtils.TEXT_DISABLED);

            NavigationService.RegisterNavChangeNotifier(this);
            AuthService.Get().RegisterAuthChangeNotify(this);

            NavigateToSignUpPageCommand = new RelayCommand(()=>Navigate(typeof(SignUpPage)));
            NavigateToSignInPageCommand = new RelayCommand(() => Navigate(typeof(SignInPage)));
            LogoutCommand = new RelayCommand(LogOut);
            SearchCommand = new RelayCommand(Search);
            GoToAIPageCommand = new RelayCommand(() => Navigate(typeof(AIRecommendationPage)));
        }

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

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<string> OnSearchEventHandler;

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
                    ThemeUtils.ChangeTheme(selectedTheme);


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
                GoBackButtonColor = ThemeUtils.GetBrush(ThemeUtils.TEXT_PRIMARY);
            }
            else
            {
                GoBackButtonColor = ThemeUtils.GetBrush(ThemeUtils.TEXT_DISABLED);
            }

            if (CanGoForward)
            {
                GoForwardButtonColor = ThemeUtils.GetBrush(ThemeUtils.TEXT_PRIMARY);
            }
            else
            {
                GoForwardButtonColor = ThemeUtils.GetBrush(ThemeUtils.TEXT_DISABLED);
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
            AuthService.Get().UnregisterAuthChangeNotify(this);
        }

        ~TopFrameViewModel()
        {
            Dispose();
        }

        public void OnUserChange(User user, string token)
        {
            if(user == null)
            {
                IsUserLoggedIn = false;
                return;
            }

            User = user;
            IsUserLoggedIn = true;
        }

        public void LogOut()
        {
            UserDAO.Get().SignOut();
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                if (_searchQuery != value)
                {
                    _searchQuery = value;
                    OnPropertyChanged(nameof(SearchQuery));
                }
            }
        }

        public void Search()
        {
            OnSearchEventHandler?.Invoke(this, SearchQuery);
        }

        public void SubscribeToSearchEvent(EventHandler<string> handler)
        {
            OnSearchEventHandler += handler;
        }

        public void UnsubscribeFromSearchEvent(EventHandler<string> handler)
        {
            OnSearchEventHandler -= handler;
        }

        public void OnSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchQuery = (sender as TextBox).Text;
            SearchQuery = SearchQuery.Trim();
        }
    }
}
