using Microsoft.UI.Xaml;
using OneLastSong.Services;
using OneLastSong.Views.Components;
using OneLastSong.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.ApplicationModel.UserDataAccounts;
using OneLastSong.DAOs;
using OneLastSong.Utils;
using Microsoft.Graphics.DirectX;
using WinUI3Localizer;

namespace OneLastSong.ViewModels
{
    public class SignInPageViewModel : INotifyPropertyChanged
    {
        public NavigationService NavigationService { get; set; }
        public XamlRoot XamlRoot { get; set;  }

        public SignInPageViewModel()
        {
            NavigationService = NavigationService.Get();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void signUpButton_Click(Object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(SignUpPage));
        }

        public async void SignInUser(string text, string password)
        {
            try
            {
                await UserDAO.Get().SignInUser(text, password);
                SnackbarUtils.ShowSnackbar(Localizer.Get().GetLocalizedString(LocalizationUtils.SIGN_IN_SUCCESS_STRING), SnackbarType.Success);
            }
            catch (Exception ex)
            {
                if(XamlRoot == null)
                {
                    LogUtils.Error("XamlRoot is null");
                    return;
                }
                LogUtils.Error($"Error signing in user: {ex.Message}");
                await DialogUtils.ShowDialogAsync(Localizer.Get().GetLocalizedString(LocalizationUtils.LOGIN_STRING), 
                    Localizer.Get().GetLocalizedString(LocalizationUtils.SIGN_IN_FAIL_STRING), 
                    XamlRoot);
            }
        }
    }

}
