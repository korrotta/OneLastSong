using OneLastSong.Cores.Classes;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Cores.DataItems
{
    public class CommentDataItem : INotifyPropertyChanged
    {
        string _content;
        public string Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged(nameof(Content));
                }
            }
        }

        string _authorName;
        public string AuthorName
        {
            get => _authorName;
            set
            {
                if (_authorName != value)
                {
                    _authorName = value;
                    OnPropertyChanged(nameof(AuthorName));
                }
            }
        }

        string _authorAvatarUrl;
        public string AuthorAvatarUrl
        {
            get => _authorAvatarUrl;
            set
            {
                if (_authorAvatarUrl != value)
                {
                    _authorAvatarUrl = value;
                    OnPropertyChanged(nameof(AuthorAvatarUrl));
                }
            }
        }

        string _relativeTime;
        public string RelativeTime
        {
            get => _relativeTime;
            set
            {
                if (_relativeTime != value)
                {
                    _relativeTime = value;
                    OnPropertyChanged(nameof(RelativeTime));
                }
            }
        }

        public CommentDataItem(Comment comment)
        {
            Content = comment.CommentText;
            AuthorName = "Anonymous";
            AuthorAvatarUrl = "https://firebasestorage.googleapis.com/v0/b/onelastsong-5d5a8.appspot.com/o/images%2FUser.png?alt=media&token=ebf3514f-17e3-4360-a2fe-fd9a60cb1802";
            RelativeTime = DateUtils.GetRelativeTime(comment.CreatedAt);
            Load(comment);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void Load(Comment comment)
        {
            UserDAO userDAO = UserDAO.Get();
            UserDisplayInfo userDisplayInfo = await userDAO.GetUserDisplayInfo(comment.UserId);
            AuthorName = userDisplayInfo.DisplayName;
            AuthorAvatarUrl = userDisplayInfo.AvatarUrl;
        }
    }
}
