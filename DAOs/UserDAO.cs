using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using OneLastSong.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneLastSong.DAOs
{
    public class UserDAO
    {
        IDb _db = null;
        List<User> _artists = new List<User>();
        private static readonly String STORED_TOKEN_PATH = "Session token";
        public User User { private set; get; } = null;
        public String SessionToken { get; private set; } = null;
        private AuthService _authService;

        public UserDAO()
        {
        }

        public void SetAuthService(AuthService authService)
        {
            _authService = authService;
        }

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public async Task SignInUser(string username, string password)
        {
            String token = await _db.UserLogin(username, password);

            if(token == "")
            {
                throw new Exception("Invalid credentials");
            }

            var user = await _db.GetUser(token);
            User = user;
            SessionToken = token;
            _authService.NotifyUserChange(user);
        }

        public async Task<User> GetUser(string sessionToken)
        {
            return await _db.GetUser(sessionToken);
        }

        public async Task SignUpUser(string username, string password)
        {
            ResultMessage result = await _db.UserSignUp(username, password);

            if (result.Status != ResultMessage.STATUS_OK)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public async Task<List<User>> GetAllArtists(bool willReload = false)
        {
            if(_artists.Count == 0 || willReload)
            {
                ResultMessage result = await _db.GetAllArtists();
                if (result.Status == ResultMessage.STATUS_OK)
                {
                    _artists = JsonSerializer.Deserialize<List<User>>(result.JsonData);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            return _artists;
        }

        public static UserDAO Get()
        {
            return (UserDAO)((App)Application.Current).Services.GetService(typeof(UserDAO));
        }

        private void ClearStoredToken()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values.Remove(STORED_TOKEN_PATH);
        }

        public void SaveCurrentSessionToken()
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            localSettings.Values[STORED_TOKEN_PATH] = SessionToken;
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
                    SessionToken = loadedToken;
                    _authService.NotifyUserChange(user);
                    return true;
                }
            }
            return false;
        }

        public void SignOut()
        {
            User = null;
            SessionToken = null;
            ClearStoredToken();
            _authService.NotifyUserChange(null);
        }
    }
}
