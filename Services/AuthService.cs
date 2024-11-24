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
        }

        public void RegisterAuthChangeNotify(IAuthChangeNotify notify)
        {
            _authChangeNotifies.Add(notify);
        }

        public void UnregisterAuthChangeNotify(IAuthChangeNotify notify)
        {
            _authChangeNotifies.Remove(notify);
        }

        public static AuthService Get()
        {
            return (AuthService)((App)Application.Current).Services.GetService(typeof(AuthService));
        }

        public void NotifyUserChange(User user)
        {
            foreach (var notify in _authChangeNotifies)
            {
                _eventHandler.TryEnqueue(() =>
                {
                    notify.OnUserChange(user);
                });
            }
        }

        public async Task<bool> OnSubsystemInitialized()
        {
            _userDAO = UserDAO.Get();
            _userDAO.SetAuthService(this);
            await _userDAO.TryToLoadStoredToken();
            return true;
        }

        public void Dispose()
        {
            
        }
    }
}
