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

namespace OneLastSong.DAOs
{
    public class CommentDAO
    {
        IDb _db;
        
        public CommentDAO()
        {
        }

        public void Init()
        {
            _db = ((App)Application.Current).Services.GetService<IDb>();
        }

        public async Task CommentAudio(string sessionToken, int audioId, string comment)
        {
            ResultMessage result = await _db.CommentAudio(sessionToken, audioId, comment);
            if (result.Status != ResultMessage.STATUS_OK)
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public async Task<List<Comment>> GetCommentsByAudioId(int audioId)
        {
            ResultMessage result = await _db.GetCommentsByAudioId(audioId);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                return JsonSerializer.Deserialize<List<Comment>>(result.JsonData);
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public static CommentDAO Get()
        {
            return (CommentDAO)((App)Application.Current).Services.GetService(typeof(CommentDAO));
        }
    }
}
