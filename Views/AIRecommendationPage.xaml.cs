using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.ViewModels;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AIRecommendationPage : Page, IDisposable
    {
        public AIRecommendationPageViewModel ViewModel { get; } = new AIRecommendationPageViewModel();

        public AIRecommendationPage()
        {
            this.InitializeComponent();
        }

        public void SendButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.HandleUserChat();
        }

        public void InputTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                SendButton_Click(this, new RoutedEventArgs());
            }
        }

        public void Dispose()
        {
            ViewModel.Dispose();
        }

        public void QuickAction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button)
            {
                LogUtils.Debug(button.Content.ToString());
            }
        }
    }
}
