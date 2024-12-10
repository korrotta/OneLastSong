using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Services
{
    public class PlaylistService : IDisposable, INotifySubsytemStateChanged, IAuthChangeNotify
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

        public async Task<bool> OnSubsystemInitialized()
        {
            AuthService.Get().RegisterAuthChangeNotify(this);
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
