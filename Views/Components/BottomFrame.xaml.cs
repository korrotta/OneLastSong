using Microsoft.UI.Xaml.Controls;
using OneLastSong.ViewModels;

namespace OneLastSong.Views.Components
{
    public sealed partial class BottomFrame : Page
    {
        public BottomFrameViewModel ViewModel { get; set; }

        public BottomFrame()
        {
            this.InitializeComponent();
            this.ViewModel = new BottomFrameViewModel();
            this.DataContext = this.ViewModel;
        }
    }
}
