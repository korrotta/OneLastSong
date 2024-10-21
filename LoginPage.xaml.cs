using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Views;
using System;
using System.Security.Cryptography;
using System.Text;
using Windows.Storage;

namespace OneLastSong
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            this.InitializeComponent();
            LoadLoginInfo();
        }

   
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameLogin.Text;
            string password = PasswordLogin.Password;

            if (RememberMeLogin.IsChecked == true)
            {
                SaveLoginInfo(username, password);
            }

            // Add your login logic here
            else {
                if (username == "admin" && password == "123")
                {
                    Frame.Navigate(typeof(MainPage));
                }
                else
                {
                    // Show error message
                    ErrorTextBlock.Text = "Invalid username or password";
                }
            }
        }

        private void NavigateToRegisterPage_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RegisterPage));
        }

        private void SaveLoginInfo(string username, string password)
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            localSettings.Values["Username"] = username;
            localSettings.Values["Password"] = HashPassword(password);
        }

        private void LoadLoginInfo()
        {
            var localSettings = ApplicationData.Current.LocalSettings;
            if (localSettings.Values.ContainsKey("Username") && localSettings.Values.ContainsKey("Password"))
            {
                UsernameLogin.Text = localSettings.Values["Username"].ToString();
                // Note: For security reasons, you should not store plain text passwords.
                // This is just for demonstration purposes.
            }
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
        private TextBlock ErrorTextBlock;

    }
}
