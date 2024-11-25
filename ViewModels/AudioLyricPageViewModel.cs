using Microsoft.UI.Xaml.Controls;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Models;
using OneLastSong.Services;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace OneLastSong.ViewModels
{
    public class AudioLyricPageViewModel : IDisposable, IAudioStateChanged
    {
        private readonly LyricDAO _lyricDAO;
        private ListView _lyricListView;
        private ListeningService _listeningService;
        public ObservableCollection<Lyric> Lyrics { get; set; } = new ObservableCollection<Lyric>();

        public AudioLyricPageViewModel(ListView lyricListView)
        {
            _lyricListView = lyricListView;

            _lyricDAO = LyricDAO.Get();
            _listeningService = ListeningService.Get();

            _listeningService.RegisterAudioStateChangeListeners(this);
        }

        public async Task LoadLyrics(int audioId)
        {
            var lyricsDictionary = await _lyricDAO.GetLyric(audioId);
            Lyrics.Clear();
            foreach (DictionaryEntry entry in lyricsDictionary)
            {
                Lyrics.Add(new Lyric
                {
                    Timestamp = (float)entry.Key,
                    LyricText = (string)entry.Value,
                    IsFocused = false
                });
            }
        }

        public void Dispose()
        {
            _listeningService.UnregisterAudioStateChangeListeners(this);
        }

        public void FocusLyric(int timestamp)
        {
            foreach (var lyric in Lyrics)
            {
                lyric.IsFocused = false;
            }

            var closestLyric = Lyrics.OrderBy(lyric => Math.Abs((int)lyric.Timestamp - timestamp)).FirstOrDefault();
            if (closestLyric != null && _lyricListView != null)
            {
                closestLyric.IsFocused = true;
                var index = Lyrics.IndexOf(closestLyric);
                var scrollToIndex = Lyrics.Count>10? (index < Lyrics.Count - 5 ? index + 4 : Lyrics.Count - 1) : index;
                _lyricListView.ScrollIntoView(Lyrics[scrollToIndex]);
            }

            // set the corresponding item to be selected
            _lyricListView.SelectedItem = closestLyric;
        }

        public async void OnAudioChanged(Audio newAudio)
        {
            if(newAudio == null)
            {
                return;
            }

            await LoadLyrics(newAudio.AudioId);
        }

        public void OnAudioPlayStateChanged(bool isPlaying)
        {
        }

        public void OnAudioProgressChanged(int progress)
        {
            FocusLyric(progress);
        }
    }
}
