using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.Views.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.UserDataAccounts;
using WinUI3Localizer;

namespace OneLastSong.ViewModels
{
    public class SignUpPageViewModel : INotifyPropertyChanged
    {
        public NavigationService NavigationService { get; set; }
        private string _usernameValidationMessage = "";
        private string _passwordValidationMessage = "";
        private string _confirmPasswordValidationMessage = "";
        private string _username = "";
        private string _password = "";
        private string _confirmPassword = "";

        public XamlRoot XamlRoot { get; set; }
        public ICommand SignUpCommand { get; private set; }

        public SignUpPageViewModel()
        {
            NavigationService = NavigationService.Get();
            SignUpCommand = new RelayCommand(SignUp);
        }

        private bool ValidateAll()
        {
            ValidateUsername(Username);
            ValidatePassword(Password);
            ValidateConfirmPassword(ConfirmPassword);

            return string.IsNullOrEmpty(UsernameValidationMessage) &&
                string.IsNullOrEmpty(PasswordValidationMessage) &&
                string.IsNullOrEmpty(ConfirmPasswordValidationMessage);
        }

        private async void SignUp()
        {
            try
            {
                if(!ValidateAll())
                {
                    return;
                }

                await UserDAO.Get().SignUpUser(Username, Password);
                await DialogUtils.ShowDialogAsync(Localizer.Get().GetLocalizedString(LocalizationUtils.INFO_STRING),
                    Localizer.Get().GetLocalizedString(LocalizationUtils.SIGN_UP_SUCCESS_STRING),
                    XamlRoot);
            }
            catch (Exception ex)
            {
                LogUtils.Error($"Error signing up user: {ex.Message}");
                await DialogUtils.ShowDialogAsync(Localizer.Get().GetLocalizedString(LocalizationUtils.ERROR_STRING),
                    ex.Message,
                    XamlRoot);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void logInButton_Click(Object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(SignInPage));
        }

        public void Username_TextChanged(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string text = textBox.Text;
                ValidateUsername(text);
                LogUtils.Info($"Username: {text}");
                LogUtils.Info($"UsernameValidationMessage: {UsernameValidationMessage}");
            }
        }

        private void ValidateUsername(string text)
        {
            UsernameValidationMessage = AuthUtils.ValidateUsername(text);
        }

        public void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                string password = passwordBox.Password;
                ValidatePassword(password);
            }
        }

        private void ValidatePassword(string password)
        {
            PasswordValidationMessage = AuthUtils.ValidatePassword(password);
            ConfirmPasswordValidationMessage = AuthUtils.ValidateConfirmPassword(password, ConfirmPassword);
        }

        public void ConfirmPassword_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                string confirmPassword = passwordBox.Password;
                ValidateConfirmPassword(confirmPassword);
            }
        }

        private void ValidateConfirmPassword(string confirmPassword)
        {
            ConfirmPasswordValidationMessage = AuthUtils.ValidateConfirmPassword(Password, confirmPassword);
        }

        public string UsernameValidationMessage
        {
            get => _usernameValidationMessage;
            set
            {
                _usernameValidationMessage = value;
                OnPropertyChanged(nameof(UsernameValidationMessage));
            }
        }

        public string PasswordValidationMessage
        {
            get => _passwordValidationMessage;
            set
            {
                _passwordValidationMessage = value;
                OnPropertyChanged(nameof(PasswordValidationMessage));
            }
        }

        public string ConfirmPasswordValidationMessage
        {
            get => _confirmPasswordValidationMessage;
            set
            {
                _confirmPasswordValidationMessage = value;
                OnPropertyChanged(nameof(ConfirmPasswordValidationMessage));
            }
        }

        public string Username
        {
            get => _username;
            set
            {
                _username = value;
                OnPropertyChanged(nameof(Username));
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public string ConfirmPassword
        {
            get => _confirmPassword;
            set
            {
                _confirmPassword = value;
                OnPropertyChanged(nameof(ConfirmPassword));
            }
        }
    }

}
