using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.DAOs
{
    public class UserDAO
    {
        IDb _db = null;

        public UserDAO()
        {
        }

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public async Task SignInUser(string username, string password)
        {
            String token = await _db.SignInUser(username, password);
            AuthService.Get().SetToken(token);
            var user = await _db.GetUser(token);
            AuthService.Get().SetUser(user);
        }

        public static UserDAO Get()
        {
            return (UserDAO)((App)Application.Current).Services.GetService(typeof(UserDAO));
        }
    }
}
