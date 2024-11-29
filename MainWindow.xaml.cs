using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using OneLastSong.UI;
using OneLastSong.Utils;
using System;
using System.Collections.ObjectModel;
using WinUI3Localizer;
using OpenAI;
using OpenAI.Chat;

namespace OneLastSong
{
    public enum SnackbarType
    {
        Info,
        Success,
        Warning,
        Error
    }


    public sealed partial class MainWindow : Window
    {
        public ObservableCollection<SnackbarMessage> SnackbarMessages { get; set; }

        public MainWindow()
        {
            this.InitializeComponent();
            AppWindow.SetIcon("Assets/AppIcon.ico");
            SnackbarMessages = new ObservableCollection<SnackbarMessage>();
        }

        public void NavigateMainFrameTo(Type pageType)
        {
            MainFrame.Navigate(pageType);
        }

        public void ShowSnackbar(string message, SnackbarType type = SnackbarType.Info, int duration = 3)
        {
            var color = type switch
            {
                SnackbarType.Warning => ThemeUtils.GetBrush(ThemeUtils.WARNING_BRUSH),
                SnackbarType.Error => ThemeUtils.GetBrush(ThemeUtils.ERROR_BRUSH),
                SnackbarType.Success => ThemeUtils.GetBrush(ThemeUtils.SUCCESS_BRUSH),
                _ => ThemeUtils.GetBrush(ThemeUtils.INFO_BRUSH)
            };

            var title = type switch
            {
                SnackbarType.Warning => "Warning",
                SnackbarType.Error => Localizer.Get().GetLocalizedString(LocalizationUtils.ERROR_STRING),
                SnackbarType.Success => Localizer.Get().GetLocalizedString(LocalizationUtils.SUCCESS_STRING),
                _ => Localizer.Get().GetLocalizedString(LocalizationUtils.INFO_STRING)
            };

            var icon = type switch
            {
                SnackbarType.Warning => SnackbarUtils.WARNING_ICON,
                SnackbarType.Error => SnackbarUtils.ERROR_ICON,
                SnackbarType.Success => SnackbarUtils.SUCCESS_ICON,
                _ => SnackbarUtils.INFO_ICON
            };

            var snackbarMessage = new SnackbarMessage
            {
                Icon = icon,
                Title = title,
                Message = message,
                Background = color
            };

            SnackbarMessages.Add(snackbarMessage);

            // Automatically remove the snackbar after a few seconds
            var timer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(duration) };
            timer.Tick += (sender, args) =>
            {
                snackbarMessage.Visibility = Visibility.Collapsed;
                SnackbarMessages.Remove(snackbarMessage);
                timer.Stop();
            };
            timer.Start();
        }

        private void CloseSnackbar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is SnackbarMessage snackbarMessage)
            {
                snackbarMessage.Visibility = Visibility.Collapsed;
                SnackbarMessages.Remove(snackbarMessage);
            }
        }
    }
}
