using OneLastSong.Contracts;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using OneLastSong.Utils;

namespace OneLastSong.DAOs
{
    public class AudioDAO
    {
        private List<Audio> _audios = new List<Audio>();
        private IDb _db;
        private Random _random = new Random();

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public async Task<List<Audio>> GetMostLikeAudios(bool willRefresh = false, int limit = 1000)
        {
            if (willRefresh || _audios.Count == 0)
            {
                ResultMessage result = await _db.GetMostLikeAudios(limit);
                if (result.Status == ResultMessage.STATUS_OK)
                {
                    _audios = JsonSerializer.Deserialize<List<Audio>>(result.JsonData);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            return _audios;
        }

        public Audio GetRandom()
        {
            if (_audios.Count == 0)
            {
                throw new Exception("No audios available");
            }

            return _audios[_random.Next(0, _audios.Count)];
        }

        public static AudioDAO Get()
        {
            return (AudioDAO)((App)Application.Current).Services.GetService(typeof(AudioDAO));
        }

        internal IEnumerable<Audio> GetPlaylistAudios(int playlistId)
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<Audio> GetAlbumAudios(int albumId)
        {
            throw new NotImplementedException();
        }

        internal IEnumerable<Audio> GetArtistAudios(int artistId)
        {
            throw new NotImplementedException();
        }

        internal async Task<Audio> GetAudioById(int audioId)
        {
            if(_audios.Count == 0)
            {
                await GetMostLikeAudios();
            }

            return _audios.Find(audio => audio.AudioId == audioId);
        }
    }
}
