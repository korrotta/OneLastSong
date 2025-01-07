using OneLastSong.DAOs;
using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Cores.DataItems
{
    public class AudioItem : Audio
    {
        private PlaylistDAO _playlistDAO;
        private UserDAO _userDAO;

        private Playlist _likedPlaylist;

        public enum AudioLikeStateType
        {
            Fetching,
            Liked,
            NotLiked
        }

        private AudioLikeStateType _audioLikeState = AudioLikeStateType.Fetching;

        public AudioLikeStateType AudioLikeState
        {
            get => _audioLikeState;
            set
            {
                if (_audioLikeState != value)
                {
                    _audioLikeState = value;
                    OnPropertyChanged(nameof(AudioLikeState));
                }
            }
        }

        public AudioItem(Audio a)
        {
            _playlistDAO = PlaylistDAO.Get();
            _userDAO = UserDAO.Get();

            AudioId = a.AudioId;
            Title = a.Title;
            Artist = a.Artist;
            AlbumId = a.AlbumId;
            AuthorId = a.AuthorId;
            Duration = a.Duration;
            CreatedAt = a.CreatedAt;
            CategoryId = a.CategoryId;
            Description = a.Description;
            CoverImageUrl = a.CoverImageUrl;
            Likes = a.Likes;
            Url = a.Url;

            Audio = this;

            UpdateLikeState();
        }

        public async void UpdateLikeState()
        {
            if(String.IsNullOrEmpty(_userDAO.SessionToken))
            {
                return;
            }

            _likedPlaylist = await _playlistDAO.GetLikePlaylist(_userDAO.SessionToken);
            if (AudioLikeState == AudioLikeStateType.Fetching)
            {
                AudioLikeState = _likedPlaylist?.ContainsAudio(Audio.AudioId) == true
                    ? AudioLikeStateType.Liked
                    : AudioLikeStateType.NotLiked;
            }
        }

        private bool _isPlaying;
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (_isPlaying != value)
                {
                    _isPlaying = value;
                    OnPropertyChanged(nameof(IsPlaying));
                }
            }
        }

        public Audio Audio { get; private set; }
    }
}