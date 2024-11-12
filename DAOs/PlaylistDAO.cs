using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneLastSong.DAOs
{
    public class PlaylistDAO
    {
        private List<Playlist> _playlistList = new List<Playlist>();
        private IDb _db;

        public PlaylistDAO()
        {
        }

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public async Task<List<Playlist>> GetUserPlaylists(string sessionToken, bool willRefresh = false)
        {
            if (willRefresh || _playlistList.Count == 0)
            {
                ResultMessage result = await _db.GetAllUserPlaylists(sessionToken);
                if (result.Status == ResultMessage.STATUS_OK)
                {
                    _playlistList = JsonSerializer.Deserialize<List<Playlist>>(result.JsonData);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            return _playlistList;
        }

        public static PlaylistDAO Get()
        {
            return (PlaylistDAO)((App)Application.Current).Services.GetService(typeof(PlaylistDAO));
        }
    }
}
