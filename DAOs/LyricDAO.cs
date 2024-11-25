using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneLastSong.DAOs
{
    public class LyricDAO
    {
        private IDb _db;
        private readonly Dictionary<int, OrderedDictionary> _lyricsCache = new();

        public LyricDAO()
        {
        }

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public async Task<OrderedDictionary> GetLyric(int audioId, bool willRefresh = false)
        {
            if (willRefresh || !_lyricsCache.ContainsKey(audioId))
            {
                List<Lyric> lyrics = await GetLyricFromDb(audioId);
                OrderedDictionary orderedDictionary = new();
                foreach (Lyric lyric in lyrics)
                {
                    orderedDictionary.Add(lyric.Timestamp, lyric.LyricText);
                }
                _lyricsCache[audioId] = orderedDictionary;
            }
            return _lyricsCache[audioId];
        }

        private async Task<List<Lyric>> GetLyricFromDb(int audioId)
        {
            ResultMessage result = await _db.GetLyrics(audioId);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                return JsonSerializer.Deserialize<List<Lyric>>(result.JsonData);
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public static LyricDAO Get()
        {
            return (LyricDAO)((App)Application.Current).Services.GetService(typeof(LyricDAO));
        }
    }
}
