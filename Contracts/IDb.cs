using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Contracts
{
    public interface IDb
    {
        public Task Connect();
        public Task Dispose();
        public Task<string> DoTest();
        public Task<string> UserLogin(string username, string password);
        public Task<User> GetUser(string sessionToken);
        public Task<ResultMessage> UserSignUp(string username, string password);
        public Task<ResultMessage> GetMostLikeAudios(int limit=1000);
        public Task<ResultMessage> GetFirstNAlbums(int limit = 20);
        public Task<ResultMessage> GetAllUserPlaylists(string sessionToken);
        public Task<ResultMessage> AddUserPlaylist(string sessionToken, string playlistName, string coverImageUrl);
        public Task<ResultMessage> GetAllArtists();
    }
}
