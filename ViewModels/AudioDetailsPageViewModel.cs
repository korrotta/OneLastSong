using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using OneLastSong.Cores.Classes;
using OneLastSong.Cores.DataItems;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
using OneLastSong.Views.Dialogs;
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
        public ICommand RateAudioCommand;
        CommentDAO _commentDAO;
        RatingDAO _ratingDAO;
        UserDAO _userDAO;
        public XamlRoot XamlRoot { get; set; }
        private float _userRating = 5f;

        public AudioDetailsPageViewModel()
        {
            _audioDAO = AudioDAO.Get();
            _commentDAO = CommentDAO.Get();
            _ratingDAO = RatingDAO.Get();
            _userDAO = UserDAO.Get();
            SubmitCommentCommand = new RelayCommand(SubmitComment);
            RateAudioCommand = new RelayCommand(ShowRatingDialog);
        }

        public async void ShowRatingDialog()
        {
            // if player is not signed in show error message
            if (UserDAO.Get().User == null)
            {
                await DialogUtils.ShowDialogAsync(
                    LocalizationUtils.GetString(LocalizationUtils.ERROR_STRING),
                    LocalizationUtils.GetString(LocalizationUtils.NOT_SIGNED_IN_ERROR_STRING),
                    XamlRoot
                );
                return;
            }
            // Show the create new playlist dialog
            await ShowCreateNewPlaylistDialogAsync();
        }

        public async Task ShowCreateNewPlaylistDialogAsync()
        {
            if(XamlRoot == null)
            {
                return;
            }

            if(_userDAO.User == null)
            {
                await DialogUtils.ShowDialogAsync(
                    LocalizationUtils.GetString(LocalizationUtils.ERROR_STRING),
                    LocalizationUtils.GetString(LocalizationUtils.NOT_SIGNED_IN_ERROR_STRING),
                    XamlRoot
                );
                return;
            }

            var dialog = new RatingAudioDialog
            {
                XamlRoot = this.XamlRoot,
                Audio = Audio,
                Score = _userRating
            };
            var result = await dialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                var sessionToken = _userDAO.SessionToken;

                try
                {
                    await _ratingDAO.RateAudio(sessionToken, Audio.AudioId, dialog.Score);
                    LoadAudioRating(Audio.AudioId);
                    LoadUserRating(Audio.AudioId);
                    SnackbarUtils.ShowSnackbar("Your rating has been added", SnackbarType.Success);
                }
                catch (Exception e)
                {
                    await DialogUtils.ShowDialogAsync(
                        LocalizationUtils.GetString(LocalizationUtils.ERROR_STRING),
                        e.Message,
                        XamlRoot
                    );
                }
            }
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

        private string _yourRating = "NA";
        public string YourRating
        {
            get => _yourRating;
            set
            {
                if(_yourRating != value)
                {
                    _yourRating = value;
                    OnPropertyChanged(nameof(YourRating));
                }
            }
        }

        private int _ratingCount = 0;
        public int RatingCount
        {
            get => _ratingCount;
            set
            {
                if (_ratingCount != value)
                {
                    _ratingCount = value;
                    OnPropertyChanged(nameof(RatingCount));
                }
            }
        }

        private async void SubmitComment()
        {
            try
            {
                string sessionToken = _userDAO.SessionToken;
                if(sessionToken == null)
                {
                    SnackbarUtils.ShowSnackbar("Please sign in to comment", SnackbarType.Error);
                    return;
                }

                await _commentDAO.CommentAudio(sessionToken, Audio.AudioId, NewCommentContent);
                NewCommentContent = "";
                SnackbarUtils.ShowSnackbar("Your comment has been added", SnackbarType.Success);
                LoadAudioComments(Audio.AudioId);
            }
            catch (Exception e)
            {
                SnackbarUtils.ShowSnackbar(e.Message, SnackbarType.Error);
            }
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

        private ObservableCollection<CommentDataItem> _comments = new ObservableCollection<CommentDataItem>();
        public ObservableCollection<CommentDataItem> Comments
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

        private async void LoadAudioRating(int audioId)
        {
            AudioRatingSummary audioRating = await _ratingDAO.GetRatingScoreByAudioId(audioId);
            Rating = audioRating.AverageRating;
            RatingCount = audioRating.RatingCount;
        }


        public async void LoadAudio(int audioId)
        {
            Audio = Audio.Default;
            LoadAudioRating(audioId);
            LoadUserRating(audioId);
            Audio = await _audioDAO.GetAudioById(audioId, true);
            LoadAudioComments(audioId);
        }

        private async void LoadUserRating(int audioId)
        {
            YourRating = "NA";
            User currentUser = _userDAO.User;

            if (currentUser != null)
            {
                UserAudioRating userAudioRating = await _ratingDAO.GetUserAudioRating(currentUser.Id, audioId);
                if (userAudioRating != null)
                {
                    YourRating = userAudioRating.Rating.ToString() + "/5";
                    _userRating = userAudioRating.Rating;
                }
            }
        }

        private async void LoadAudioComments(int audioId)
        {
            Comments.Clear();

            try
            {
                List<Comment> comments = await _commentDAO.GetCommentsByAudioId(audioId);

                foreach (Comment comment in comments)
                {
                    Comments.Add(new CommentDataItem(comment));
                }
            }
            catch (Exception e)
            {
                SnackbarUtils.ShowSnackbar(e.Message, SnackbarType.Error);
            }
        }
    }
}
