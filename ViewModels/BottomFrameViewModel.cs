﻿using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Models;
using OneLastSong.Services;
using OneLastSong.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;

namespace OneLastSong.ViewModels
{
    public class BottomFrameViewModel : INotifyPropertyChanged, IAudioStateChanged, IDisposable
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private ListeningService _listeningService;
        private Audio _currentAudio = Audio.Default;
        private int _currentProgress = 0;
        private DispatcherQueue _dispatcherQueue;
        private Slider _slider;
        private bool _isPlaying = false;
        public ICommand ChangePlayStateCommand { get; set; }

        public Audio CurrentAudio
        {
            get => _currentAudio;
            set
            {
                if (_currentAudio != value)
                {
                    _currentAudio = value;
                    OnPropertyChanged(nameof(CurrentAudio));
                }
            }
        }

        public int CurrentProgress
        {
            get => _currentProgress;
            set
            {
                if (_currentProgress != value)
                {
                    _currentProgress = value;
                    OnPropertyChanged(nameof(CurrentProgress));
                }
            }
        }

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

        public BottomFrameViewModel(DispatcherQueue dispatcherQueue, Slider slider)
        {
            _listeningService = ListeningService.Get();
            _listeningService.RegisterAudioStateChangeListeners(this);
            this._dispatcherQueue = dispatcherQueue;
            this._slider = slider;
            ChangePlayStateCommand = new RelayCommand(ChangePlayState);
        }

        private void ChangePlayState()
        {
            _listeningService.ChangePlayState();
        }

        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void OnAudioChanged(Audio audio)
        {
            CurrentAudio = audio;
            OnPropertyChanged(nameof(CurrentAudio));
        }

        public void OnAudioPlayStateChanged(bool isPlaying)
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                IsPlaying = isPlaying;
            });            
        }

        public void OnAudioProgressChanged(int progress)
        {
            if (_dispatcherQueue.HasThreadAccess)
            {
                // If already on the UI thread, update directly
                _slider.Value = progress;
                CurrentProgress = progress;
            }
            else
            {
                // Otherwise, marshal the call to the UI thread
                _dispatcherQueue.TryEnqueue(() =>
                {
                    _slider.Value = progress;
                    CurrentProgress = progress;
                });
            }
        }

        public void Dispose()
        {
            _listeningService.UnregisterAudioStateChangeListeners(this);
        }

        public void OnSliderValueChanged(int newValue)
        {
            // Add any additional logic you need when the slider value changes
            _listeningService.Seek(newValue);
        }
    }
}
