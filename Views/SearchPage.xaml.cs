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
        }

        public object GetCurrentParameterState()
        {
            return SearchPageViewModel.CurrentSearchQuery;
        }

        public void OnStateLoad(object parameter)
        {
            SearchPageViewModel.OnSearchEventTrigger(parameter.ToString());
        }
    }
}
