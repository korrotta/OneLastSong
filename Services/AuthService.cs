using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using Windows.Data.Xml.Dom;

namespace OneLastSong.Services
{
    public class AuthService : IDisposable, INotifySubsytemStateChanged
    {
        private List<IAuthChangeNotify> _authChangeNotifies = new List<IAuthChangeNotify>();
        private DispatcherQueue _eventHandler;
        private UserDAO _userDAO;

        public AuthService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
            _userDAO = UserDAO.Get();
            _userDAO.SetAuthService(this);
        }

        public void RegisterAuthChangeNotify(IAuthChangeNotify notify)
        {
            if(_authChangeNotifies.Contains(notify))
            {
                return;
            }

            _authChangeNotifies.Add(notify);
        }

        public void UnregisterAuthChangeNotify(IAuthChangeNotify notify)
        {
            if(!_authChangeNotifies.Contains(notify))
            {
                return;
            }

            _authChangeNotifies.Remove(notify);
        }

        public static AuthService Get()
        {
            return (AuthService)((App)Application.Current).Services.GetService(typeof(AuthService));
        }

        public void NotifyUserChange(User user, string token)
        {
            foreach (var notify in _authChangeNotifies)
            {
                _eventHandler.TryEnqueue(() =>
                {
                    notify.OnUserChange(user, token);
                });
            }
        }

        public async Task<bool> OnSubsystemInitialized()
        {
            await Task.CompletedTask;
            return true;
        }

        public void Dispose()
        {
            
        }

        internal async void OnComponentsLoaded()
        {
            await _userDAO.TryToLoadStoredToken();
        }
    }
}
