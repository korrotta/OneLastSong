using CommunityToolkit.Mvvm.Input;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OneLastSong.ViewModels
{
    public class AudioDetailsPageViewModel : INotifyPropertyChanged
    {
        public ICommand SubmitCommentCommand;

        public AudioDetailsPageViewModel()
        {
            _audioDAO = AudioDAO.Get();
            SubmitCommentCommand = new RelayCommand(SubmitComment);
        }

        private float _rating = 5f;
        public float Rating
        {
            get => _rating;
            set
            {
                if (_rating != value)
                {
                    _rating = value;
                    OnPropertyChanged(nameof(Rating));
                }
            }
        }

        private void SubmitComment()
        {
            throw new NotImplementedException();
        }

        private string _newCommentContent = string.Empty;
        public string NewCommentContent
        {
            get => _newCommentContent;
            set
            {
                if (_newCommentContent != value)
                {
                    _newCommentContent = value;
                    OnPropertyChanged(nameof(NewCommentContent));
                }
            }
        }

        private ObservableCollection<Comment> _comments = new ObservableCollection<Comment>();
        public ObservableCollection<Comment> Comments
        {
            get => _comments;
            set
            {
                if (_comments != value)
                {
                    _comments = value;
                    OnPropertyChanged(nameof(Comments));
                }
            }
        }

        private AudioDAO _audioDAO;

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
