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

        public UserDAO()
        {
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
            AuthService.Get().SetSession(user, token);
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
    }
}
