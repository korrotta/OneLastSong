using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class AudioDetailsPageViewModel : INotifyPropertyChanged
    {
        private AudioDAO _audioDAO;

        public AudioDetailsPageViewModel()
        {
            _audioDAO = AudioDAO.Get();
        }

        private Audio _audio = Audio.Default;
        public Audio Audio
        {
            get => _audio;
            set
            {
                if (_audio != value)
                {
                    _audio = value;
                    OnPropertyChanged(nameof(Audio));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void LoadAudio(int audioId)
        {
            try
            {
                Audio = await _audioDAO.GetAudioById(audioId, true);
            }
            catch (Exception e)
            {
                SnackbarUtils.ShowSnackbar(e.Message, SnackbarType.Error);
            }
        }
    }
}
