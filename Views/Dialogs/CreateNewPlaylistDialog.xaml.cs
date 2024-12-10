using Microsoft.UI.Xaml.Controls;
using OneLastSong.Utils;
using WinUI3Localizer;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Dialogs
{
    public sealed partial class CreateNewPlaylistDialog : ContentDialog
    {
        public string PlaylistName { get; private set; }

        public CreateNewPlaylistDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            PlaylistName = PlaylistNameTextBox.Text;
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            // Handle cancel action if needed
        }
    }
}
