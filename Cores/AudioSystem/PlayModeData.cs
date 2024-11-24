using Microsoft.UI.Xaml.Hosting;
using OneLastSong.DAOs;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OneLastSong.Cores.AudioSystem
{
    public class PlayModeData
    {
        public PlayMode CurrentPlayMode { get; set; } = PlayMode.NotPlaying;
        public List<Audio> PlayQueue { get; set; } = new List<Audio>();

        public int? PlaylistId { get; set; }
        public int? AlbumId { get; set; }
        public int? ArtistId { get; set; }

        public SemaphoreSlim Semaphore { get; set; } = new SemaphoreSlim(1, 1);

        public Audio CurrentAudio { get; set; }

        private ListeningSession _listeningSession;

        public async Task PlayMashUpAsync(Audio audio)
        {
            await Semaphore.WaitAsync();
            try
            {
                CurrentPlayMode = PlayMode.MashUp;
                PlayQueue.Clear();
                PlayQueue.Add(audio);
                CurrentAudio = audio;
                PlayQueue.Add(AudioDAO.Get().GetRandom());
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task PlayPlaylistAsync(int playlistId)
        {
            await Semaphore.WaitAsync();
            try
            {
                CurrentPlayMode = PlayMode.Playlist;
                PlaylistId = playlistId;
                PlayQueue.Clear();
                PlayQueue.AddRange(AudioDAO.Get().GetPlaylistAudios(playlistId));
                CurrentAudio = PlayQueue[0];
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task PlayAlbumAsync(int albumId)
        {
            await Semaphore.WaitAsync();
            try
            {
                CurrentPlayMode = PlayMode.Album;
                AlbumId = albumId;
                PlayQueue.Clear();
                PlayQueue.AddRange(AudioDAO.Get().GetAlbumAudios(albumId));
                CurrentAudio = PlayQueue[0];
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task PlayArtistAsync(int artistId)
        {
            await Semaphore.WaitAsync();
            try
            {
                CurrentPlayMode = PlayMode.Artist;
                ArtistId = artistId;
                PlayQueue.Clear();
                PlayQueue.AddRange(AudioDAO.Get().GetArtistAudios(artistId));
                CurrentAudio = PlayQueue[0];
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task<Audio> NextAudioAsync()
        {
            await Semaphore.WaitAsync();
            try
            {
                if (CurrentPlayMode == PlayMode.NotPlaying)
                {
                    return null;
                }

                if (PlayQueue.Count == 0)
                {
                    if (CurrentPlayMode == PlayMode.MashUp)
                    {
                        PlayQueue.Add(AudioDAO.Get().GetRandom());
                    }
                    else
                    {
                        CurrentPlayMode = PlayMode.MashUp;
                        PlayQueue.Add(AudioDAO.Get().GetRandom());
                    }
                }

                Audio nextAudio = PlayQueue[0];
                PlayQueue.RemoveAt(0);

                CurrentAudio = nextAudio;
                return nextAudio;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task<bool> IsTheCurrentAudioFromMashup(int id)
        {
            await Semaphore.WaitAsync();
            try
            {
                return CurrentPlayMode == PlayMode.MashUp && CurrentAudio.AudioId == id;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        public async Task<PlayMode> GetCurrentPlayModeSafe()
        {
            await Semaphore.WaitAsync();
            try
            {
                return CurrentPlayMode;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        internal async void AddPlaylistToQueue(Playlist playlist)
        {
            await Semaphore.WaitAsync();
            try
            {
                PlayQueue.AddRange(playlist.Audios);
            }
            finally
            {
                Semaphore.Release();
            }
        }

        internal async void PlayPlaylist(Playlist playlist)
        {
            await Semaphore.WaitAsync();
            try
            {
                CurrentPlayMode = PlayMode.Playlist;
                PlaylistId = playlist.PlaylistId;
                PlayQueue.Clear();
                PlayQueue.AddRange(playlist.Audios);
                CurrentAudio = PlayQueue[0];
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
