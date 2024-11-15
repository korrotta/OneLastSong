using OneLastSong.Contracts;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class SearchPageViewModel : INotifyPropertyChanged
    {
        private string _currentSearchQuery = "";

        public String CurrentSearchQuery
        {
            get => _currentSearchQuery;
            set
            {
                if (_currentSearchQuery != value)
                {
                    _currentSearchQuery = value;
                    OnPropertyChanged(nameof(CurrentSearchQuery));
                }
            }
        }

        public SearchPageViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnSearchEventTrigger(string newSearchQuery)
        {
            if(CurrentSearchQuery.Equals(newSearchQuery.Trim()))
            {
                return;
            }

            CurrentSearchQuery = newSearchQuery.Trim();

            LogUtils.Debug($"SearchPageViewModel received {newSearchQuery}");
        }
    }
}
