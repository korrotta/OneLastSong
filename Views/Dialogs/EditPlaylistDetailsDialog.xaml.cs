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
using OneLastSong.ViewModels;
using OneLastSong.Models;
using OneLastSong.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Dialogs
{
    public sealed partial class EditPlaylistDetailsDialog : ContentDialog
    {
        private EditPlaylistDetailsDialogViewModel ViewModel { get; }

        public EditPlaylistDetailsDialog(Playlist playlist)
        {
            this.InitializeComponent();
            ViewModel = new EditPlaylistDetailsDialogViewModel(playlist);
        }

        private async void SaveButton_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                await ViewModel.UpdateCurrentPlaylist();
                SnackbarUtils.ShowSnackbar("Playlist details updated", SnackbarType.Success);
            }
            catch (Exception ex)
            {
                await DialogUtils.ShowDialogAsync("Error", "Failed to update playlist details", XamlRoot);
            }
        }
    }
}
