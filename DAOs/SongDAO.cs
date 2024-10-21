using OneLastSong.Models;
using System.Collections.Generic;
using System.Linq;

namespace OneLastSong.DAOs
{
    class SongDAO
    {
        private readonly List<Song> _songDatabase;

        public SongDAO()
        {
            _songDatabase = new List<Song>();
        }

        public Song GetSongById(int id)
        {
            return _songDatabase.FirstOrDefault(s => s.Id == id);
        }

        public void AddSong(Song song)
        {
            _songDatabase.Add(song);
        }

        public void UpdateSong(Song song)
        {
            var existingSong = GetSongById(song.Id);
            if (existingSong != null)
            {
                existingSong.Title = song.Title;
                existingSong.Artist = song.Artist;
                existingSong.Album = song.Album;
                existingSong.Duration = song.Duration;
                existingSong.Thumbnail = song.Thumbnail;
                existingSong.FilePath = song.FilePath;
            }
        }

        public void DeleteSong(int id)
        {
            var songToRemove = GetSongById(id);
            if (songToRemove != null)
            {
                _songDatabase.Remove(songToRemove);
            }
        }

        public List<Song> GetAllSongs()
        {
            return _songDatabase;
        }
    }
}
