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

        public async Task<Audio> GetRandom()
        {
            if (_audios.Count == 0)
            {
                await GetMostLikeAudios(true);
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

        internal async Task<Audio> GetAudioById(int audioId, bool forcedRefresh = false)
        {
            if (forcedRefresh)
            {
                ResultMessage result = await _db.GetAudioById(audioId);
                if (result.Status == ResultMessage.STATUS_OK)
                {
                    Audio audio = JsonSerializer.Deserialize<Audio>(result.JsonData);
                    if(_audios.Find(a => a.AudioId == audio.AudioId) == null)
                    {
                        _audios.Add(audio);
                    }
                    else
                    {
                        _audios[_audios.FindIndex(a => a.AudioId == audio.AudioId)] = audio;
                    }

                    return audio;
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            if (_audios.Count == 0)
            {
                await GetMostLikeAudios();
            }

            return _audios.Find(audio => audio.AudioId == audioId);
        }

        public async Task<string> GetAllAudiosInRawJson(bool willRefresh = false)
        {
            if(willRefresh || _audios.Count == 0)
            {
                await GetMostLikeAudios();
            }

            List<ConciseAudioObj> conciseAudioObjs = new List<ConciseAudioObj>();

            foreach (Audio audio in _audios)
            {
                conciseAudioObjs.Add(new ConciseAudioObj(audio));
            }

            return JsonSerializer.Serialize(conciseAudioObjs);
        }
    }
}
