using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Services
{
    public class PlaylistService : IDisposable, INotifySubsytemStateChanged, IAuthChangeNotify, INotifyLikeAudioStateChanged
    {
        private List<INotifyPlaylistChanged> playlistNotifiers = new List<INotifyPlaylistChanged>();
        private DispatcherQueue _eventHandler;
        private PlaylistDAO playlistDAO;

        public PlaylistService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
            playlistDAO = PlaylistDAO.Get();
            playlistDAO.SetPlaylistService(this);
        }

        public static PlaylistService Get()
        {
            return (PlaylistService)((App)Application.Current).Services.GetService(typeof(PlaylistService));
        }

        public void Dispose()
        {
            AuthService.Get().UnregisterAuthChangeNotify(this);
            AudioService.Get().UnregisterAudioLikeStateNotifier(this);
        }

        public void NotifyPlaylistChanged(List<Playlist> playlists)
        {
            foreach (var notifier in playlistNotifiers)
            {
                _eventHandler.TryEnqueue(() =>
                {
                    notifier.OnPlaylistUpdated(playlists);
                });
            }
        }

        public async void OnAnAudioLiked(int audioId)
        {
            string token = UserDAO.Get().SessionToken;

            if(String.IsNullOrEmpty(token))
            {
                return;
            }

            Playlist likedPlaylist = await playlistDAO.GetLikePlaylist(token);
            if (likedPlaylist == null)
            {
                return;
            }

            Audio likedAudio = await AudioDAO.Get().GetAudioById(audioId);
            var newAudioList = likedPlaylist.Audios.ToList();
            newAudioList.Add(likedAudio);
            likedPlaylist.Audios = newAudioList.ToArray();
            playlistDAO.UpdatePlaylistInCache(likedPlaylist);
        }

        public async void OnAnAudioLikeRemoved(int audioId)
        {
            string token = UserDAO.Get().SessionToken;
            if (String.IsNullOrEmpty(token))
            {
                return;
            }
            Playlist likedPlaylist = await playlistDAO.GetLikePlaylist(token);
            if (likedPlaylist == null)
            {
                return;
            }
            var newAudioList = likedPlaylist.Audios.ToList();
            newAudioList.RemoveAll(a => a.AudioId == audioId);
            likedPlaylist.Audios = newAudioList.ToArray();
            playlistDAO.UpdatePlaylistInCache(likedPlaylist);
        }

        public async Task<bool> OnSubsystemInitialized()
        {
            AuthService.Get().RegisterAuthChangeNotify(this);
            AudioService.Get().RegisterAudioLikeStateNotifier(this);
            NotifyPlaylistChanged(playlistDAO.GetCachedPlaylists());
            await Task.CompletedTask;
            return true;
        }

        public async void OnUserChange(User user, string token)
        {
            if(user == null)
            {
                playlistDAO.ClearPlaylistCache();
            }
            else
            {
                var sessionToken = token;
                await playlistDAO.GetUserPlaylists(sessionToken, true);
            }
        }

        public void RegisterPlaylistNotifier(INotifyPlaylistChanged notifier)
        {
            if(playlistNotifiers.Contains(notifier))
            {
                return;
            }

            playlistNotifiers.Add(notifier);
        }

        public void UnregisterPlaylistNotifier(INotifyPlaylistChanged notifier)
        {
            if(!playlistNotifiers.Contains(notifier))
            {
                return;
            }

            playlistNotifiers.Remove(notifier);
        }
    }
}
