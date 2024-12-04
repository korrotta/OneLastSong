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
using OneLastSong.Contracts;
using OneLastSong.ViewModels;
using OneLastSong.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AudioDetailsPage : Page, INavigationStateSavable
    {
        public AudioDetailsPageViewModel ViewModel { get; private set; } = new AudioDetailsPageViewModel();

        public AudioDetailsPage()
        {
            this.InitializeComponent();
        }

        public object GetCurrentParameterState()
        {
            return ViewModel.Audio.AudioId.ToString();
        }

        public void OnStateLoad(object parameter)
        {
            try
            {
                string stringId = parameter as string;
                int audioId = int.Parse(stringId);
                ViewModel.LoadAudio(audioId);
            }
            catch (Exception e)
            {
                SnackbarUtils.ShowSnackbar("There was an error while loading audio's details", SnackbarType.Error);
            }
        }
    }
}
