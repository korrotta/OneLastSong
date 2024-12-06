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
using OneLastSong.Contracts;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page, INavigationStateSavable
    {
        public SearchPageViewModel SearchPageViewModel { get; set; } = new SearchPageViewModel();

        public SearchPage()
        {
            this.InitializeComponent();
            DataContext = SearchPageViewModel;
            Loaded += SearchPage_Loaded;
        }

        private void SearchPage_Loaded(object sender, RoutedEventArgs e)
        {
            // SearchPageViewModel.Init();
        }

        public object GetCurrentParameterState()
        {
            return SearchPageViewModel.CurrentSearchQuery;
        }

        public void OnStateLoad(object parameter)
        {
            SearchPageViewModel.OnSearchEventTrigger(parameter.ToString());
        }

        public void AudioTitleHyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is HyperlinkButton hyperlinkButton)
            {
                try
                {
                    SearchPageViewModel.NavigateToAudioDetails(((int)hyperlinkButton.Tag).ToString());
                }
                catch (Exception ex)
                {
                    // Log exception
                    SnackbarUtils.ShowSnackbar("There was an error while navigating to audio details", SnackbarType.Error);
                }
            }
        }
    }
}
