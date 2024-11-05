// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

using Microsoft.UI.Xaml.Controls;
using OneLastSong.ViewModels;

namespace OneLastSong.Views.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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
