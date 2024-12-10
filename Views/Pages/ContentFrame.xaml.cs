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
using OneLastSong.Services;
using Microsoft.Extensions.DependencyInjection;
using OneLastSong.DAOs;
using OneLastSong.Views.Components;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ContentFrame : Page
    {
        private NavigationService _navService = null;

        public ContentFrame()
        {
            this.InitializeComponent();
            _navService = ((App)Application.Current).Services.GetService<NavigationService>();
            _navService.Initialize(MainFrame);
            _navService.Navigate(typeof(HomePage));
        }
    }
}
