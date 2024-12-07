using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class PlayHistoryPageViewModel : IDisposable, INotifyPropertyChanged, INotifyPlayHistoriesChanged
    {
        private AudioDAO audioDAO;

        private PlayHistoryDAO playHistoryDAO;
        private PlayHistoryService playHistoryService;
        private UserDAO userDAO;

        public PlayHistoryPageViewModel()
        {
            audioDAO = AudioDAO.Get();
            playHistoryDAO = PlayHistoryDAO.Get();
            userDAO = UserDAO.Get();
            playHistoryService = PlayHistoryService.Get();
            playHistoryService.RegisterPlayHistoryChangedNotify(this, FetchUserHistory);
            playHistories = new ObservableCollection<Audio>();

        }

        private void FetchUserHistory(object sender, EventArgs e)
        {
            playHistoryDAO.FetchUserHistory(userDAO.SessionToken);
        }

        private ObservableCollection<Audio> playHistories;
        public ObservableCollection<Audio> PlayHistories
        {
            get { return playHistories; }
            set
            {
                playHistories = value;
                OnPropertyChanged(nameof(PlayHistories));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void Dispose()
        {
            playHistoryService.UnregisterPlayHistoryChangedNotify(this);
        }

        public async void OnPlayHistoriesChanged(List<PlayHistory> playHistories)
        {
            PlayHistories.Clear();
            for(int i = 0; i < playHistories.Count; i++)
            {
                Audio audio = await audioDAO.GetAudioById(playHistories[i].AudioId);
                PlayHistories.Add(audio);
            }
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
