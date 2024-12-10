using OneLastSong.Contracts;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.Extensions.DependencyInjection;
using OneLastSong.Services;
using System.Security.Policy;
using System.Text.Json;

namespace OneLastSong.DAOs
{
    public class PlayHistoryDAO
    {
        private List<PlayHistory> _playHistories = new List<PlayHistory>();
        private PlayHistoryService _playHistoryService;
        private IDb _db;

        public PlayHistoryDAO()
        {
        }

        internal void SetPlayHistoryService(PlayHistoryService playHistoryService)
        {
            _playHistoryService = playHistoryService;
        }

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public static PlayHistoryDAO Get()
        {
            return (PlayHistoryDAO)((App)Application.Current).Services.GetService(typeof(PlayHistoryDAO));
        }

        internal async Task<List<PlayHistory>> GetUserPlayHistory(string sessionToken)
        {
            ResultMessage result = await _db.GetUserPlayHistory(sessionToken);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                _playHistories = JsonSerializer.Deserialize<List<PlayHistory>>(result.JsonData);
                _playHistoryService.NotifyPlayHistoriesChanged(_playHistories);
                return _playHistories;
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public void ClearPlayHistories()
        {
            _playHistories.Clear();
            _playHistoryService.NotifyPlayHistoriesChanged(_playHistories);
        }

        public async void AddUserPlayHistory(string sessionToken, int audioId)
        {
            ResultMessage result = await _db.AddUserPlayHistory(sessionToken, audioId);
            if (result.Status != ResultMessage.STATUS_OK)
            {
                throw new Exception(result.ErrorMessage);
            }
            PlayHistory playHistory = JsonSerializer.Deserialize<PlayHistory>(result.JsonData);
            _playHistories.Add(playHistory);
            _playHistoryService.NotifyPlayHistoriesChanged(_playHistories);
        }

        internal void FetchUserHistory(string sessionToken)
        {
            if(sessionToken == null)
            {
                return;
            }

            _ = GetUserPlayHistory(sessionToken);
        }
    }
}
