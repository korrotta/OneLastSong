using OneLastSong.DAOs;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneLastSong.BUS
{
    class SongBus
    {
        private readonly SongDAO _songDao;

        public SongBus(SongDAO songDao)
        {
            _songDao = songDao;
        }

        public Song GetSongDetails(int id)
        {
            var song = _songDao.GetSongById(id);
            if (song == null)
            {
                throw new Exception("Song not found.");
            }
            return song;
        }

        public void AddNewSong(Song song)
        {
            if (string.IsNullOrEmpty(song.Title))
            {
                throw new Exception("Song title is required.");
            }
            if (song.Duration.TotalSeconds <= 0)
            {
                throw new Exception("Song duration must be positive.");
            }
            if (song.FilePath == null)
            {
                throw new Exception("Song file path is required.");
            }
            if (song.Thumbnail == null)
            {
                throw new Exception("Song thumbnail is required.");
            }

            _songDao.AddSong(song);
        }

        public void UpdateSong(Song song)
        {
            if (song == null || song.Id == 0)
            {
                throw new Exception("Invalid song data.");
            }

            _songDao.UpdateSong(song);
        }

        public void DeleteSong(int id)
        {
            _songDao.DeleteSong(id);
        }

        public List<Song> GetAllSongs()
        {
            return _songDao.GetAllSongs();
        }

        public async Task AddNewSongFromMetadata(string filepath)
        {
            var song = new Song();
            await song.loadMetadata(filepath);
            AddNewSong(song);
        }
    }

}
