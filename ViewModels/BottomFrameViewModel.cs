using OneLastSong.BUS;
using OneLastSong.Controller;
using OneLastSong.DAOs;
using OneLastSong.Models;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class BottomFrameViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private Song _currentSong;
        private SongController _songController;

        public Song CurrentSong
        {
            get => _currentSong;
            set
            {
                if (_currentSong != value)
                {
                    _currentSong = value;
                    OnPropertyChanged(nameof(CurrentSong));
                }
            }
        }

        public BottomFrameViewModel()
        {
            _songController = new SongController(new SongBus(new SongDAO()));
            CurrentSong = new Song();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task AddNewSongFromFilePath(string filePath)
        {
            await _songController.AddNewSongFromMetadata(filePath);
            CurrentSong = await LoadSongFromFilePath(filePath);
        }

        private async Task<Song> LoadSongFromFilePath(string filePath)
        {
            var song = new Song();
            await song.loadMetadata(filePath);
            return song;
        }

        internal async Task AddNewSongFromFilePath(object filePath)
        {
            throw new NotImplementedException();
        }
    }
}
