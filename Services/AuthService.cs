using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
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

        public User User { private set; get; } = null;

        public AuthService()
        {
        }

        public void SetUser(User user)
        {
            User = user;
            foreach (var notify in _authChangeNotifies)
            {
                notify.OnUserChange(user);
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
    }
}
