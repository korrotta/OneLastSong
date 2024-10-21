using OneLastSong.BUS;
using OneLastSong.Models;
using System;
using System.Threading.Tasks;

namespace OneLastSong.Controller
{
    class SongController
    {
        private readonly SongBus _songBus;

        public SongController(SongBus songBus)
        {
            _songBus = songBus;
        }

        public void DisplaySongDetails(int id)
        {
            var song = _songBus.GetSongDetails(id);
            Console.WriteLine($"Title: {song.Title}");
            Console.WriteLine($"Artist: {song.Artist}");
            Console.WriteLine($"Album: {song.Album}");
            Console.WriteLine($"Duration: {song.Duration}");
            Console.WriteLine($"Thumbnail: {song.Thumbnail.ToString}");
            Console.WriteLine($"File Path: {song.FilePath}");
        }

        public async Task AddNewSongFromMetadata(string filepath)
        {
            await _songBus.AddNewSongFromMetadata(filepath);
            Console.WriteLine("Song added successfully.");
        }

        public void AddNewSong(Song song)
        {
            _songBus.AddNewSong(song);
            Console.WriteLine("Song added successfully.");
        }

        public void UpdateSong(Song song)
        {
            _songBus.UpdateSong(song);
            Console.WriteLine("Song updated successfully.");
        }

        public void DeleteSong(int id)
        {
            _songBus.DeleteSong(id);
            Console.WriteLine("Song deleted successfully.");
        }

        public void DisplayAllSongs()
        {
            var songs = _songBus.GetAllSongs();
            foreach (var song in songs)
            {
                Console.WriteLine($"Title: {song.Title}");
                Console.WriteLine($"Artist: {song.Artist}");
                Console.WriteLine($"Album: {song.Album}");
                Console.WriteLine($"Duration: {song.Duration}");
                Console.WriteLine($"Thumbnail: {song.Thumbnail.ToString}");
                Console.WriteLine($"File Path: {song.FilePath}");
                Console.WriteLine();
            }
        }
    }
}
