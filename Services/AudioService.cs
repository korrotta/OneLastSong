using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Services
{
    public class AudioService : IDisposable, INotifySubsytemStateChanged
    {
        private List<INotifyLikeAudioStateChanged> _audioLikeStateNotifiers = new List<INotifyLikeAudioStateChanged>();
        private DispatcherQueue _eventHandler;
        private AudioDAO _audioDAO;

        public AudioService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
        }

        public void NotifyAudioLiked(int audioId)
        {
            _eventHandler.TryEnqueue(() =>
            {
                foreach (var notifier in _audioLikeStateNotifiers)
                {
                    notifier.OnAnAudioLiked(audioId);
                }
            });
        }

        public void NotifyAudioLikeRemoved(int audioId)
        {
            _eventHandler.TryEnqueue(() =>
            {
                foreach (var notifier in _audioLikeStateNotifiers)
                {
                    notifier.OnAnAudioLikeRemoved(audioId);
                }
            });
        }

        public void RegisterAudioLikeStateNotifier(INotifyLikeAudioStateChanged notifier)
        {
            if(_audioLikeStateNotifiers.Contains(notifier))
            {
                return;
            }

            _eventHandler.TryEnqueue(() => _audioLikeStateNotifiers.Add(notifier));
        }

        public void UnregisterAudioLikeStateNotifier(INotifyLikeAudioStateChanged notifier)
        {
            if (!_audioLikeStateNotifiers.Contains(notifier))
            {
                return;
            }
            _eventHandler.TryEnqueue(() => _audioLikeStateNotifiers.Remove(notifier));
        }

        public void Dispose()
        {
            
        }

        public Task<bool> OnSubsystemInitialized()
        {
            _audioDAO = AudioDAO.Get();
            _audioDAO.SetAudioService(this);
            return Task.FromResult(true);
        }

        public static AudioService Get()
        {
            return (AudioService)((App)Application.Current).Services.GetService(typeof(AudioService));
        }
    }
}
