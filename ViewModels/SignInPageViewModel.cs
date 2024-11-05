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

namespace OneLastSong.ViewModels
{
    public class SignInPageViewModel : INotifyPropertyChanged
    {
        public NavigationService NavigationService { get; set; }

        public SignInPageViewModel()
        {
            NavigationService = NavigationService.Get();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        
        public void signUpButton_Click(Object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(typeof(SignUpPage));
        }
    }

}
