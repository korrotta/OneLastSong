using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OneLastSong.Models;

namespace OneLastSong.Services
{
    class PlayHistoryService : IDisposable, INotifySubsytemStateChanged, IAuthChangeNotify
    {
        private DispatcherQueue _eventHandler;
        private PlayHistoryDAO _playHistoryDAO;

        private List<INotifyPlayHistoriesChanged> _playHistoryNotifiers = new List<INotifyPlayHistoriesChanged>();

        private AuthService _authService;

        public PlayHistoryService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
        }

        public static PlayHistoryService Get()
        {
            return (PlayHistoryService)((App)Application.Current).Services.GetService(typeof(PlayHistoryService));
        }

        public void RegisterPlayHistoryChangedNotify(INotifyPlayHistoriesChanged notify, EventHandler onRegisterCompleted)
        {
            _eventHandler.TryEnqueue(() =>
            {
                if (_playHistoryNotifiers.Contains(notify))
                {
                    onRegisterCompleted?.Invoke(this, EventArgs.Empty);
                    return;
                }
                _playHistoryNotifiers.Add(notify);
                onRegisterCompleted?.Invoke(this, EventArgs.Empty);
            });
        }

        public void UnregisterPlayHistoryChangedNotify(INotifyPlayHistoriesChanged notify)
        {
            _eventHandler.TryEnqueue(() =>
            {
                if (!_playHistoryNotifiers.Contains(notify))
                {
                    return;
                }
                _playHistoryNotifiers.Remove(notify);
            });
        }

        public async Task<bool> OnSubsystemInitialized()
        {
            _playHistoryDAO = PlayHistoryDAO.Get();
            _authService = AuthService.Get();

            _playHistoryDAO.SetPlayHistoryService(this);

            _authService.RegisterAuthChangeNotify(this);

            await Task.CompletedTask;
            return true;
        }

        public void Dispose()
        {

        }

        public async void OnUserChange(User user, string token)
        {
            if(user == null)
            {
                _playHistoryDAO.ClearPlayHistories();
                return;
            }

            string sessionToken = token;
            _ = await _playHistoryDAO.GetUserPlayHistory(sessionToken);
        }

        public void NotifyPlayHistoriesChanged(List<PlayHistory> playHistories)
        {
            _eventHandler.TryEnqueue(() =>
            {
                foreach (var notifier in _playHistoryNotifiers)
                {
                    notifier.OnPlayHistoriesChanged(playHistories);
                }
            });
        }
    }
}
