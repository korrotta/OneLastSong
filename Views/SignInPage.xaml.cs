using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.ViewModels;
using OneLastSong.Utils;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SignInPage : Page
    {
        public SignInPageViewModel SignInPageViewModel { get; set; } = new SignInPageViewModel();

        public SignInPage()
        {
            this.InitializeComponent();
            Loaded += SignInPage_Loaded;
        }

        private void SignInPage_Loaded(object sender, RoutedEventArgs e)
        {
            SignInPageViewModel.XamlRoot = this.XamlRoot;
        }

        private async void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            await DialogUtils.ShowDialogAsync(
                Localizer.Get().GetLocalizedString(LocalizationUtils.INFO_STRING),
                Localizer.Get().GetLocalizedString(LocalizationUtils.FEATURE_NOT_IMPLEMENTED_STRING),
                this.XamlRoot
                );
        }

        private void SignInButton_Click(object sender, RoutedEventArgs e)
        {
            SignInPageViewModel.SignInUser(tbUsername.Text, pbPassword.Password);
        }
    }
}
