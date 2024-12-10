using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using OneLastSong.Models;
using OneLastSong.Views.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneLastSong.ViewModels
{
    public class PlaylistMenuFlyoutViewModel
    {
        public XamlRoot XamlRoot { get; set; }
        public ICommand EditPlaylistDetailsCommand { get; }
        public Playlist Playlist { get; set; }

        public PlaylistMenuFlyoutViewModel()
        {
            EditPlaylistDetailsCommand = new RelayCommand(EditPlaylistDetails);
        }

        private async void EditPlaylistDetails()
        {
            if (XamlRoot == null) { return; }

            EditPlaylistDetailsDialog editPlaylistDetailsDialog = new EditPlaylistDetailsDialog
            {
                XamlRoot = this.XamlRoot
            };

            await editPlaylistDetailsDialog.ShowAsync();
        }
    }
}
