using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using OneLastSong.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace OneLastSong.DAOs
{
    public class ListeningSessionDAO
    {
        public ListeningSessionDAO() { }
        private ListeningService _listeningService;
        private IDb _db;

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public void SetListeningService(ListeningService listeningService)
        {
            _listeningService = listeningService;
        }

        public async Task<ListeningSession> GetListeningSession(string sessionToken)
        {
            ResultMessage result = await _db.GetListeningSession(sessionToken);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                if(result.JsonData == null)
                {
                    // User doesn't have any listening sessions.
                    return null;
                }
                return JsonSerializer.Deserialize<ListeningSession>(result.JsonData);
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public async Task SaveListeningSession(string sessionToken, ListeningSession listeningSession)
        {
            ResultMessage result = await _db.SaveListeningSession(sessionToken, listeningSession.AudioId, listeningSession.Progress);
            if (result.Status != ResultMessage.STATUS_OK)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public static ListeningSessionDAO Get()
        {
            return (ListeningSessionDAO)((App)Application.Current).Services.GetService(typeof(ListeningSessionDAO));
        }
    }
}
