using Microsoft.UI.Xaml.Controls;
using OneLastSong.Models;
using OneLastSong.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Dialogs
{
    public sealed partial class EditUserProfileDialog : ContentDialog
    {
        public EditUserProfileDialogViewModel ViewModel { get; private set; } = new EditUserProfileDialogViewModel();

        public EditUserProfileDialog(User user)
        {
            this.InitializeComponent();
            ViewModel.User = user;
        }

        public User GetUser()
        {
            return ViewModel.User;
        }
    }
}
