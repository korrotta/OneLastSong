using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Dispatching;
using System;
using OneLastSong.Contracts;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using OneLastSong.Views.Pages;

namespace OneLastSong.Services
{
    public class SidePanelNavigationService : IDisposable, INotifySubsytemStateChanged
    {
        private Frame _sidePanelFrame;
        private DispatcherQueue _eventHandler;

        public SidePanelNavigationService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
        }

        public void Initialize(Frame sidePanelFrame)
        {
            _sidePanelFrame = sidePanelFrame;
        }

        public void Navigate(Type pageType, object parameter = null)
        {
            if(_sidePanelFrame == null)
            {
                throw new InvalidOperationException("SidePanelFrame is not initialized");
            }

            if(_sidePanelFrame.Content != null && _sidePanelFrame.Content.GetType() == pageType)
            {
                return;
            }

            if (_sidePanelFrame.Content is IDisposable disposableContent)
            {
                disposableContent.Dispose();
            }

            _sidePanelFrame.Navigate(pageType, parameter);
        }

        public static SidePanelNavigationService Get()
        {
            return (SidePanelNavigationService)((App)Application.Current).Services.GetService(typeof(SidePanelNavigationService));
        }

        public void Dispose()
        {
            if (_sidePanelFrame.Content is IDisposable disposableContent)
            {
                disposableContent.Dispose();
            }

            _sidePanelFrame = null;
        }

        public async Task<bool> OnSubsystemInitialized()
        {
            await Task.CompletedTask;
            return true;
        }
    }
}
