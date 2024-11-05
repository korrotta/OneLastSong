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

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TopFrame : Page
    {
        public TopFrameViewModel TopFrameViewModel { get; set; } = new TopFrameViewModel();

        public TopFrame()
        {
            this.InitializeComponent();
            // Set the DataContext of the page to the ViewModel to allow for user login state data binding
            this.DataContext = TopFrameViewModel;
            this.Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            TopFrameViewModel.XamlRoot = this.XamlRoot;
        }

        public void signInButton_Click(object sender, RoutedEventArgs e)
        {
            TopFrameViewModel.Navigate(typeof(SignInPage));
        }

        public void signUpButton_Click(object sender, RoutedEventArgs e)
        {
            TopFrameViewModel.Navigate(typeof(SignUpPage));
        }
    }
}
