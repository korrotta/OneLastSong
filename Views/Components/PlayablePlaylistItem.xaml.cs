using System;
using System.ComponentModel;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Views.Pages;

namespace OneLastSong.Views.Components
{
    public sealed partial class PlayablePlaylistItem : UserControl, INotifyPropertyChanged
    {
        private ListeningService _listeningService;
        private NavigationService _navigationService;

        public static readonly DependencyProperty PlaylistProperty =
            DependencyProperty.Register(nameof(Playlist), typeof(Playlist), typeof(PlayablePlaylistItem), new PropertyMetadata(null));

        public Playlist Playlist
        {
            get => (Playlist)GetValue(PlaylistProperty);
            set => SetValue(PlaylistProperty, value);
        }

        public PlayablePlaylistItem()
        {
            this.InitializeComponent();
            _listeningService = ListeningService.Get();
            _navigationService = NavigationService.Get();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool IsHovered { get; set; }

        private void UserControl_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            IsHovered = true;
            OnPropertyChanged(nameof(IsHovered));
        }

        private void UserControl_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            IsHovered = false;
            OnPropertyChanged(nameof(IsHovered));
        }

        public void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            _listeningService.PlayPlaylist(Playlist);
        }

        public void PlaylistTitleButton_Click(object sender, RoutedEventArgs e)
        {
            _navigationService.NavigateOrReloadOnParameterChanged(typeof(PlaylistSongsPage), Playlist.PlaylistId);
        }
    }
}

