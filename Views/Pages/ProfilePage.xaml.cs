using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.ViewModels;
using OneLastSong.Views.Components;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ProfilePage : Page
    {
        public ProfileViewModel ViewModel { get; private set; } = new ProfileViewModel();

        public ProfilePage()
        {
            this.InitializeComponent();
            this.DataContext = ViewModel;
            Loaded += ProfilePage_Loaded;
        }

        private void ProfilePage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.XamlRoot = this.XamlRoot;
        }

        private void dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            ViewModel.SortAudios(e.Column.Tag.ToString(), e, dgAudios);
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            //if (sender is Button button && button.DataContext is Audio audio)
            //{
            //    await ViewModel.ShowConfirmRemoveAudioFromPlaylist(audio);
            //}
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.FilterPlayHistory(SearchBox.Text);
        }

        private void PaginationControl_PageChanged(object sender, PaginationControlValueChangedEventArgs e)
        {
            ViewModel.PaginationControl_PageChanged(e);
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.EditProfile();
        }
    }
}
