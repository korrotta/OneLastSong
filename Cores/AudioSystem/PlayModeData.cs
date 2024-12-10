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

        public ListeningSession ListeningSession { get; set; } = null;

        public async Task PlayMashUpAsync(Audio audio)
        {
            await ExecuteWithSemaphoreAsync(async () =>
            {
                CurrentPlayMode = PlayMode.MashUp;
                PlayQueue.Clear();
                PlayQueue.Add(audio);
                CurrentAudio = audio;
                Audio newAudio = await AudioDAO.Get().GetRandom();
                PlayQueue.Add(newAudio);
            });
        }

        public async Task PlayPlaylistAsync(int playlistId)
        {
            await ExecuteWithSemaphoreAsync(async () =>
            {
                CurrentPlayMode = PlayMode.Playlist;
                PlaylistId = playlistId;
                PlayQueue.Clear();
                PlayQueue.AddRange(AudioDAO.Get().GetPlaylistAudios(playlistId));
                CurrentAudio = PlayQueue[0];
            });
        }

        public async Task PlayAlbumAsync(int albumId)
        {
            await ExecuteWithSemaphoreAsync(async () =>
            {
                CurrentPlayMode = PlayMode.Album;
                AlbumId = albumId;
                PlayQueue.Clear();
                PlayQueue.AddRange(AudioDAO.Get().GetAlbumAudios(albumId));
                CurrentAudio = PlayQueue[0];
            });
        }

        public async Task PlayArtistAsync(int artistId)
        {
            await ExecuteWithSemaphoreAsync(async () =>
            {
                CurrentPlayMode = PlayMode.Artist;
                ArtistId = artistId;
                PlayQueue.Clear();
                PlayQueue.AddRange(AudioDAO.Get().GetArtistAudios(artistId));
                CurrentAudio = PlayQueue[0];
            });
        }

        public async Task<Audio> NextAudioAsync()
        {
            return await ExecuteWithSemaphoreAsync(async () =>
            {
                if (CurrentPlayMode == PlayMode.NotPlaying)
                {
                    return null;
                }

                await CheckIfQueueIsEmpty();

                Audio nextAudio = PlayQueue[0];
                PlayQueue.RemoveAt(0);

                CurrentAudio = nextAudio;
                return nextAudio;
            });
        }

        public async Task<bool> IsTheCurrentAudioFromMashup(int id)
        {
            return await ExecuteWithSemaphoreAsync(async () =>
            {
                return CurrentPlayMode == PlayMode.MashUp && CurrentAudio.AudioId == id;
            });
        }

        public async Task<PlayMode> GetCurrentPlayModeSafe()
        {
            return await ExecuteWithSemaphoreAsync(async () =>
            {
                return CurrentPlayMode;
            });
        }

        internal async Task AddPlaylistToQueueAsync(Playlist playlist)
        {
            await ExecuteWithSemaphoreAsync(async () =>
            {
                PlayQueue.AddRange(playlist.Audios);
            });
        }

        internal async Task PlayPlaylistAsync(Playlist playlist)
        {
            await ExecuteWithSemaphoreAsync(async () =>
            {
                CurrentPlayMode = PlayMode.Playlist;
                PlaylistId = playlist.PlaylistId;
                PlayQueue.Clear();
                PlayQueue.AddRange(playlist.Audios);
                CurrentAudio = PlayQueue[0];
            });
        }

        internal void RetrievePlayingSession(Audio audio, int progress)
        {
            CurrentAudio = audio;
            ListeningSession = new ListeningSession
            {
                AudioId = audio.AudioId,
                Progress = progress
            };
            CurrentPlayMode = PlayMode.MashUp;
        }

        internal void Clear()
        {
            CurrentPlayMode = PlayMode.NotPlaying;
            CurrentAudio = null;
            ListeningSession = null;
            PlayQueue.Clear();
        }

        private async Task ExecuteWithSemaphoreAsync(Func<Task> action)
        {
            await Semaphore.WaitAsync();
            try
            {
                await action();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private async Task<T> ExecuteWithSemaphoreAsync<T>(Func<Task<T>> action)
        {
            await Semaphore.WaitAsync();
            try
            {
                return await action();
            }
            finally
            {
                Semaphore.Release();
            }
        }

        internal async void PlayAudioList(List<Audio> audioList)
        {
            await Semaphore.WaitAsync();
            try
            {
                PlayQueue.Clear();
                PlayQueue.AddRange(audioList);
                CurrentAudio = audioList[0];
                CurrentPlayMode = PlayMode.MashUp;
            }
            finally
            {
                Semaphore.Release();
            }            
        }

        internal async Task AddNewAudiosToQueue(List<Audio> audios)
        {
            await Semaphore.WaitAsync();
            try
            {
                PlayQueue.AddRange(audios);
                CurrentPlayMode = PlayMode.MashUp;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        internal async Task RemoveAudioFromQueue(int index)
        {
            await Semaphore.WaitAsync();
            try
            {
                if (index < PlayQueue.Count)
                {
                    PlayQueue.RemoveAt(index);
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        internal Task<List<Audio>> GetCurrentQueueSafe()
        {
            return ExecuteWithSemaphoreAsync(async () =>
            {
                return PlayQueue;
            });
        }

        /**
         * Remove all preceding audios in the queue
         * Make the audio at the given index the first audio in the queue
         * This method has no effect if the index is invalid
         */
        internal async Task PlayAudioInQueue(int index)
        {
            // If the index is invalid, do nothing
            if (index < 1 || index >= PlayQueue.Count)
            {
                return;
            }

            await Semaphore.WaitAsync();
            try
            {
                if (index < PlayQueue.Count)
                {
                    List<Audio> newQueue = new List<Audio>();
                    for (int i = index; i < PlayQueue.Count; i++)
                    {
                        newQueue.Add(PlayQueue[i]);
                    }
                    PlayQueue = newQueue;
                }
            }
            finally
            {
                Semaphore.Release();
            }
        }

        private async Task CheckIfQueueIsEmpty()
        {
            if (PlayQueue.Count == 0)
            {
                if (CurrentPlayMode == PlayMode.MashUp)
                {
                    Audio newAudio = await AudioDAO.Get().GetRandom();
                    PlayQueue.Add(newAudio);
                }
                else
                {
                    CurrentPlayMode = PlayMode.MashUp;
                    Audio newAudio = await AudioDAO.Get().GetRandom();
                    PlayQueue.Add(newAudio);
                }
            }
        }

        internal async Task<Audio> GetCurrentAudioSafe()
        {
            return await ExecuteWithSemaphoreAsync(async () =>
            {
                await CheckIfQueueIsEmpty();
                return CurrentAudio;
            });
        }

        internal async Task SetFirstInQueue(Audio audio)
        {
            await Semaphore.WaitAsync();
            try
            {
                List<Audio> newQueue = new List<Audio>();
                newQueue.Add(audio);
                for (int i = 0; i < PlayQueue.Count; i++)
                {
                    newQueue.Add(PlayQueue[i]);
                }
                PlayQueue = newQueue;
            }
            finally
            {
                Semaphore.Release();
            }
        }

        internal async Task AddToQueue(Audio audio)
        {
            await Semaphore.WaitAsync();
            try
            {
                PlayQueue.Add(audio);
            }
            finally
            {
                Semaphore.Release();
            }
        }
    }
}
