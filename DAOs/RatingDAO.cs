using OneLastSong.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OneLastSong.Models;
using System.Text.Json;
using OneLastSong.Cores.Classes;

namespace OneLastSong.DAOs
{
    public class RatingDAO
    {
        IDb _db;

        public RatingDAO()
        {
        }

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public async Task RateAudio(string sessionToken, int audioId, float rating)
        {
            ResultMessage result = await _db.RateAudio(sessionToken, audioId, rating);
            if (result.Status != ResultMessage.STATUS_OK)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public async Task<AudioRatingSummary> GetRatingScoreByAudioId(int audioId)
        {
            ResultMessage result = await _db.GetRatingScoreByAudioId(audioId);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                return JsonSerializer.Deserialize<AudioRatingSummary>(result.JsonData);
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public async Task<UserAudioRating> GetUserAudioRating(int userId, int AudioId)
        {
            ResultMessage result = await _db.GetUserAudioRating(userId, AudioId);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                return JsonSerializer.Deserialize<UserAudioRating>(result.JsonData);
            }
            else
            {
                return null;
            }
        }

        public static RatingDAO Get()
        {
            return (RatingDAO)((App)Application.Current).Services.GetService(typeof(RatingDAO));
        }
    }
}
