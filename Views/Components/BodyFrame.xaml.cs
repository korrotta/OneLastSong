using Microsoft.UI.Xaml.Controls;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BodyFrame : Page
    {
        public BodyFrame()
        {
            this.InitializeComponent();
            LefRegion.Navigate(typeof(LeftRegion));
            RightRegion.Navigate(typeof(RightRegion));
            ContentFrame.Navigate(typeof(ContentFrame));
        }
    }
}
