using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
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
        private PlaylistService _playlistService;

        public PlaylistDAO()
        {
        }

        public void SetPlaylistService(PlaylistService playlistService)
        {
            _playlistService = playlistService;
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
                    _playlistService.NotifyPlaylistChanged(_playlistList);
                }
                else
                {
                    throw new Exception(result.ErrorMessage);
                }
            }

            return _playlistList;
        }

        public async Task<List<Audio>> GetAudiosInPlaylist(string sessionToken, int playlistId)
        {
            ResultMessage result = await _db.GetAudiosInPlaylist(sessionToken, playlistId);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                return JsonSerializer.Deserialize<List<Audio>>(result.JsonData);
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public async Task<Playlist> AddUserPlaylist(string sessionToken, string playlistName, string coverImageUrl = null)
        {
            if(coverImageUrl == null)
            {
                coverImageUrl = ConfigValueUtils.GetConfigValue(ConfigValueUtils.DEFAULT_PLAYLIST_COVER_IMAGE_URL_KEY);
            }

            ResultMessage result = await _db.AddUserPlaylist(sessionToken, playlistName, coverImageUrl);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                Playlist playlist = JsonSerializer.Deserialize<Playlist>(result.JsonData);
                _playlistList.Add(playlist);
                _playlistService.NotifyPlaylistChanged(_playlistList);
                return playlist;
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public static PlaylistDAO Get()
        {
            return (PlaylistDAO)((App)Application.Current).Services.GetService(typeof(PlaylistDAO));
        }

        public async Task AddAudioToPlaylist(string sessionToken, int playlistId, int audioId)
        {
            ResultMessage result = await _db.AddAudioToPlaylist(sessionToken, playlistId, audioId);
            if((result.Status == ResultMessage.STATUS_OK))
            {
                // fetch new playlist data
                _playlistList = await GetUserPlaylists(sessionToken, true);
                _playlistService.NotifyPlaylistChanged(_playlistList);
            }
            else 
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public async Task RemoveAudioFromPlaylist(string sessionToken, int playlistId, int audioId)
        {
            ResultMessage result = await _db.RemoveAudioFromPlaylist(sessionToken, playlistId, audioId);
            if (result.Status != ResultMessage.STATUS_OK)
            {
                throw new Exception(result.ErrorMessage);
            }
            // remove from cache
            Playlist playlist = _playlistList.Find(playlist => playlist.PlaylistId == playlistId);
            if (playlist != null)
            {
                var audioList = playlist.Audios.ToList(); // Convert array to List<Audio>
                audioList.RemoveAll(audio => audio.AudioId == audioId); // Perform removal
                playlist.Audios = audioList.ToArray(); // Convert back to array
                _playlistService.NotifyPlaylistChanged(_playlistList);
                playlist.ItemCount = playlist.Audios.Length;
            }
        }

        public async Task DeletePlaylist(string sessionToken, int playlistId)
        {
            ResultMessage result = await _db.DeletePlaylist(sessionToken, playlistId);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                // remove playlist from cache
                _playlistList.RemoveAll(playlist => playlist.PlaylistId == playlistId);
                _playlistService.NotifyPlaylistChanged(_playlistList);
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }

        public void ClearPlaylistCache()
        {
            _playlistList.Clear();
            _playlistService.NotifyPlaylistChanged(_playlistList);
        }

        public List<Playlist> GetCachedPlaylists()
        {
            return _playlistList;
        }

        internal async Task UpdatePlaylist(string sessionToken, Playlist playlist)
        {
            int playlistId = playlist.PlaylistId;
            ResultMessage result = await _db.UpdatePlaylist(sessionToken, playlistId, playlist.Name, playlist.CoverImageUrl);
            if (result.Status == ResultMessage.STATUS_OK)
            {
                // fetch new playlist data
                _playlistList = await GetUserPlaylists(sessionToken, true);
                _playlistService.NotifyPlaylistChanged(_playlistList);
            }
            else
            {
                throw new Exception(result.ErrorMessage);
            }
        }
    }
}
