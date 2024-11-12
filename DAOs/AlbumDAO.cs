using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneLastSong.DAOs
{
    public class AlbumDAO
    {
        private List<Album> _audios = new List<Album>();
        private IDb _db;

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public async Task<List<Album>> GetMostLikeAlbums(bool willRefresh = false, int limit = 20)
        {
            if (willRefresh || _audios.Count == 0)
            {
                ResultMessage result = await _db.GetFirstNAlbums(limit);
                if (result.Status == ResultMessage.STATUS_OK)
                {
                    _audios = JsonSerializer.Deserialize<List<Album>>(result.JsonData);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            return _audios;
        }

        public static AlbumDAO Get()
        {
            return (AlbumDAO)((App)Application.Current).Services.GetService(typeof(AlbumDAO));
        }
    }
}
