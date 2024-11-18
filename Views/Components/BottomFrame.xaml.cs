using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.ViewModels;
using System;

namespace OneLastSong.Views.Components
{
    public sealed partial class BottomFrame : Page
    {
        public BottomFrameViewModel ViewModel { get; set; } = new BottomFrameViewModel();

        public BottomFrame()
        {
            this.InitializeComponent();
            this.DataContext = this.ViewModel;
            this.Loaded += BottomFrame_Loaded;            
        }

        private void BottomFrame_Loaded(object sender, RoutedEventArgs e)
        {
            //ViewModel.LoadSong(new Uri("https://vgmsite.com/soundtracks/minecraft/kvptjmornx/1-18.%20Sweden.mp3"));
        }
    }
}
