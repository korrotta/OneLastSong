using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using OneLastSong.Db;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public void SignInUser(string username, string password)
        {

        }
    }
}
