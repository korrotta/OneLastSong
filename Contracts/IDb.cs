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
        /* Auth */
        public Task<string> UserLogin(string username, string password);
        public Task<User> GetUser(string sessionToken);
        public Task<ResultMessage> UserSignUp(string username, string password);
        /* Get lists */
        public Task<ResultMessage> GetMostLikeAudios(int limit=1000);
        public Task<ResultMessage> GetFirstNAlbums(int limit = 20);
        public Task<ResultMessage> GetAllUserPlaylists(string sessionToken);
        public Task<ResultMessage> AddUserPlaylist(string sessionToken, string playlistName, string coverImageUrl);
        public Task<ResultMessage> GetAllArtists();
        public Task<ResultMessage> GetAudioById(int audioId);
        /* Playlist */
        public Task<ResultMessage> AddAudioToPlaylist(string sessionToken, int playlistId, int audioId);
        public Task<ResultMessage> RemoveAudioFromPlaylist(string sessionToken, int playlistId, int audioId);
        public Task<ResultMessage> DeletePlaylist(string sessionToken, int playlistId);
        /* Play */
        public Task<ResultMessage> SaveListeningSession(string sessionToken, int audioId, int progress);
        public Task<ResultMessage> GetListeningSession(string sessionToken);
        public Task<ResultMessage> GetLyrics(int audioId);
    }
}
