using Microsoft.UI.Xaml.Controls;
using OneLastSong.ViewModels;
using System.Threading.Tasks;

namespace OneLastSong.Views.Components
{
    public sealed partial class BottomFrame : Page
    {
        private string filePath = "/Assets/samples/Sweden_C418.mp3";

        public BottomFrameViewModel ViewModel { get; set; }

        public BottomFrame()
        {
            this.InitializeComponent();
            this.ViewModel = new BottomFrameViewModel();
            this.DataContext = this.ViewModel;
            _ = AddSongFromFilePath(filePath);
        }

        public async Task AddSongFromFilePath(string filePath)
        {
            this.filePath = filePath;
            await ViewModel.AddNewSongFromFilePath(filePath);
        }
    }
}
