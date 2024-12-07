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
using OneLastSong.Models;
using OneLastSong.ViewModels;
using OneLastSong.DAOs;
using OneLastSong.Utils;
using System.Threading.Tasks;
using OneLastSong.Views.Dialogs;
using OneLastSong.Services;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    public sealed partial class AudioItemInQueueMenuFlyout : MenuFlyout
    {
        public Audio Audio
        {
            get { return (Audio)GetValue(AudioProperty); }
            set { SetValue(AudioProperty, value); }
        }

        public static readonly DependencyProperty AudioProperty =
            DependencyProperty.Register("Audio", typeof(Audio), typeof(AudioExtraControlsFlyout), new PropertyMetadata(null));

        public AudioItemInQueueMenuFlyout(XamlRoot xamlRoot, Audio currentAudio)
        {
            this.InitializeComponent();
            XamlRoot = xamlRoot;
            Audio = currentAudio;
        }
    }
}