using CommunityToolkit.WinUI.UI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.Contracts;
using OneLastSong.Models;
using OneLastSong.Utils;
using OneLastSong.ViewModels;
using System;

namespace OneLastSong.Views.Pages
{
    public sealed partial class PlaylistSongsPage : Page, INavigationStateSavable
    {
        private int _currentPlaylistId;
        public PlaylistSongsPageViewModel ViewModel { get; } = new PlaylistSongsPageViewModel();

        public PlaylistSongsPage()
        {
            this.InitializeComponent();
            Loaded += PlaylistSongsPage_Loaded;
            this.DataContext = ViewModel;
        }

        private void PlaylistSongsPage_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.XamlRoot = XamlRoot;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ViewModel.FilterAudios(SearchBox.Text);
        }

        public object GetCurrentParameterState()
        {
            return _currentPlaylistId.ToString();
        }

        public async void OnStateLoad(object parameter)
        {
            try
            {
                _currentPlaylistId = int.Parse(parameter.ToString());
                await ViewModel.Load(_currentPlaylistId);
            }
            catch (Exception ex)
            {
                SnackbarUtils.ShowSnackbar("There was an error while loading the playlist audios", SnackbarType.Error);
            }
        }

        private void dg_Sorting(object sender, DataGridColumnEventArgs e)
        {
            ViewModel.SortAudios(e.Column.Tag.ToString(), e, dgAudios);
        }

        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is Audio audio)
            {
                await ViewModel.ShowConfirmRemoveAudioFromPlaylist(audio);
            }
        }
    }
}
