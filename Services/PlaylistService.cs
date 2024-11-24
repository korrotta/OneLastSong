using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Services
{
    public class PlaylistService : IDisposable, INotifySubsytemStateChanged
    {
        private List<INotifyPlaylistChanged> playlistNotifiers = new List<INotifyPlaylistChanged>();
        private DispatcherQueue _eventHandler;

        public PlaylistService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
        }

        public static PlaylistService Get()
        {
            return (PlaylistService)((App)Application.Current).Services.GetService(typeof(PlaylistService));
        }

        public void Dispose()
        {

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
            await Task.CompletedTask;
            return true;
        }

        public void RegisterPlaylistNotifier(INotifyPlaylistChanged notifier)
        {
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
