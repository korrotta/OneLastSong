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

namespace OneLastSong.Services
{
    public class AuthService
    {
        private List<IAuthChangeNotify> _authChangeNotifies = new List<IAuthChangeNotify>();
        private String token = null;
        private static readonly String STORED_TOKEN_PATH = "Session token";

        public User User { private set; get; } = null;

        public AuthService()
        {
        }

        public String SessionToken()
        {
            return token;
        }

        public void SetSession(User user, String token, bool willStoredToken = true)
        {
            User = user;
            this.token = token;

            foreach (var notify in _authChangeNotifies)
            {
                notify.OnUserChange(user);
            }

            if (willStoredToken)
            {
                SaveCurrentSessionToken();
            }
        }

        public User GetUser()
        {
            return User;
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

        public void SignOut()
        {
            User = null;
            token = null;
            foreach (var notify in _authChangeNotifies)
            {
                notify.OnUserChange(null);
            }
            ClearStoredToken();
        }

        private void ClearStoredToken()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove(STORED_TOKEN_PATH);
        }

        public void SaveCurrentSessionToken()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[STORED_TOKEN_PATH] = token;
        }
        
        public async Task<bool> TryToLoadStoredToken()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.TryGetValue(STORED_TOKEN_PATH, out var storedToken))
            {
                var loadedToken = storedToken as String;
                var user = await UserDAO.Get().GetUser(loadedToken);
                // If the token is invalid, the user will be null
                // We only set user and token if the user is not null
                if (user != null)
                {
                    User = user;
                    token = loadedToken;
                    foreach (var notify in _authChangeNotifies)
                    {
                        notify.OnUserChange(user);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
