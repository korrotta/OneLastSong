using Microsoft.UI.Xaml;
using OneLastSong.Services;
using OneLastSong.Views.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class SignUpPageViewModel : INotifyPropertyChanged
    {
        public NavigationService NavigationService { get; set; }

        public SignUpPageViewModel()
        {
            NavigationService = NavigationService.Get();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void logInButton_Click(Object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(SignInPage));
        }
    }

}
