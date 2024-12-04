using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.Models;
using OneLastSong.ViewModels;
using System;
using System.Threading.Tasks;

namespace OneLastSong.Views
{
    public sealed partial class AudioLyricPage : Page, IDisposable
    {
        private AudioLyricPageViewModel ViewModel { get; set; }

        public AudioLyricPage()
        {
            this.InitializeComponent();
            ViewModel = new AudioLyricPageViewModel(LyricsListView);
            this.DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            //if (e.Parameter is Audio audio)
            //{
            //    await LoadLyrics(audio.AudioId);
            //}
        }

        public void Dispose()
        {
            ViewModel.Dispose();
        }
    }
}
