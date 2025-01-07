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
    public class PlayHistoryItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int _audioId;
        private AudioItem _audioItem;
        private DateTime _playedAt;

        public int AudioId
        {
            get => _audioId;
            set
            {
                _audioId = value;
                LoadAudioItem(value);
            }
        }

        private AudioDAO _audioDAO;
        
        public AudioItem AudioItem
        {
            get => _audioItem;
            set 
            {
                _audioItem = value;
                OnPropertyChanged(nameof(AudioItem));

            }
        }

        public DateTime PlayedAt
        {
            get => _playedAt;
            set
            {
                _playedAt = value;
                OnPropertyChanged(nameof(PlayedAt));
            }
        }

        public PlayHistoryItem(PlayHistory playHistory)
        {
            _audioDAO = AudioDAO.Get();
            AudioId = playHistory.AudioId;
            _playedAt = playHistory.PlayedAt;
        }

        public PlayHistoryItem(int audioId, DateTime playedAt)
        {
            _audioDAO = AudioDAO.Get();
            AudioId = audioId;
            _playedAt = playedAt;
        }

        private async void LoadAudioItem(int audioId)
        {
            _audioItem = new AudioItem(await _audioDAO.GetAudioById(audioId));
            OnPropertyChanged(nameof(AudioItem));
        }

        public String RelativePlayedAtString
        {
            get
            {
                return DateUtils.GetRelativeTime(PlayedAt);
            }
        }

        public String PlayedAtString
        {
            get
            {
                return DateUtils.GetFormattedDate(PlayedAt);
            }
        }
    }
}
