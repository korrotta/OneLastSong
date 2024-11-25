using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Animation;
using OneLastSong.Contracts;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OneLastSong.Services
{
    public class NavigationService : IDisposable, INotifySubsytemStateChanged
    {
        private Frame _frame;
        private Stack<(Type PageType, object Parameter)> _backStack = new();
        private Stack<(Type PageType, object Parameter)> _forwardStack = new();
        private List<INavChangeNotifier> navChangeNotifiers = new();
        private DispatcherQueue _eventHandler;

        public NavigationService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
        }

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public bool CanGoBack => _backStack.Count > 0;
        public bool CanGoForward => _forwardStack.Count > 0;

        public void RegisterNavChangeNotifier(INavChangeNotifier navChangeNotifier)
        {
            if (navChangeNotifiers.Contains(navChangeNotifier))
            {
                return;
            }

            navChangeNotifiers.Add(navChangeNotifier);
        }

        public void UnregisterNavChangeNotifier(INavChangeNotifier navChangeNotifier)
        {
            if (!navChangeNotifiers.Contains(navChangeNotifier))
            {
                return;
            }

            navChangeNotifiers.Remove(navChangeNotifier);
        }

        public void GoBack()
        {
            if (CanGoBack)
            {
                object currentState = _frame.GetNavigationState();

                {
                    if (_frame.Content is INavigationStateSavable savable)
                    {
                        currentState = savable.GetCurrentParameterState();
                    }

                    if (_frame.Content is IDisposable)
                    {
                        ((IDisposable)_frame.Content).Dispose();
                    }
                }

                var currentEntry = (_frame.CurrentSourcePageType, currentState);
                _forwardStack.Push(currentEntry);

                var (pageType, parameter) = _backStack.Pop();
                _frame.ContentTransitions = new TransitionCollection
                {
                    new NavigationThemeTransition
                    {
                        DefaultNavigationTransitionInfo = new SlideNavigationTransitionInfo
                        {
                            Effect = SlideNavigationTransitionEffect.FromLeft
                        }
                    }
                };
                _frame.Navigate(pageType, parameter);

                // Restore state
                {
                    if (_frame.Content is INavigationStateSavable savable)
                    {
                        savable.OnStateLoad(parameter);
                    }
                }

                NotifyNavChangeNotifiers();
            }
        }

        public void GoForward()
        {
            if (CanGoForward)
            {
                object currentState = _frame.GetNavigationState();

                {
                    if (_frame.Content is INavigationStateSavable savable)
                    {
                        currentState = savable.GetCurrentParameterState();
                    }

                    if (_frame.Content is IDisposable)
                    {
                        ((IDisposable)_frame.Content).Dispose();
                    }
                }

                var currentEntry = (_frame.CurrentSourcePageType, currentState);
                _backStack.Push(currentEntry);

                var (pageType, parameter) = _forwardStack.Pop();
                _frame.ContentTransitions = new TransitionCollection
                {
                    new NavigationThemeTransition
                    {
                        DefaultNavigationTransitionInfo = new SlideNavigationTransitionInfo
                        {
                            Effect = SlideNavigationTransitionEffect.FromRight
                        }
                    }
                };
                _frame.Navigate(pageType, parameter);

                // Restore state
                {
                    if (_frame.Content is INavigationStateSavable savable)
                    {
                        savable.OnStateLoad(parameter);
                    }
                }

                NotifyNavChangeNotifiers();
            }
        }

        public Page Navigate(Type pageType, object parameter = null)
        {
            // if requested page is the same as current page, do nothing
            if (_frame.CurrentSourcePageType == pageType)
            {
                return _frame.Content as Page;
            }

            if (_frame.CurrentSourcePageType != null)
            {
                object currentState = _frame.GetNavigationState();

                {
                    if (_frame.Content is INavigationStateSavable savable)
                    {
                        currentState = savable.GetCurrentParameterState();
                    }

                    if (_frame.Content is IDisposable)
                    {
                        ((IDisposable)_frame.Content).Dispose();
                    }
                }

                var currentEntry = (_frame.CurrentSourcePageType, currentState);
                _backStack.Push(currentEntry);
                _forwardStack.Clear();
            }

            _frame.ContentTransitions = new TransitionCollection
            {
                new NavigationThemeTransition
                {
                    DefaultNavigationTransitionInfo = new SlideNavigationTransitionInfo
                    {
                        Effect = SlideNavigationTransitionEffect.FromRight
                    }
                }
            };
            _frame.Navigate(pageType, parameter);

            NotifyNavChangeNotifiers();

            // Return the instance of the navigated-to page
            return _frame.Content as Page;
        }

        public void ClearHistory()
        {
            _backStack.Clear();
            _forwardStack.Clear();
            NotifyNavChangeNotifiers();
        }

        private void NotifyNavChangeNotifiers()
        {
            foreach (var navChangeNotifier in navChangeNotifiers)
            {
                _eventHandler.TryEnqueue(() =>
                {
                    navChangeNotifier.OnNavHistoryChanged(this);
                });
            }
        }

        public static NavigationService Get()
        {
            return (NavigationService)((App)Application.Current).Services.GetService(typeof(NavigationService));
        }

        public async Task<bool> OnSubsystemInitialized()
        {
            NotifyNavChangeNotifiers();
            await Task.CompletedTask;
            return true;
        }

        public void Dispose()
        {

        }
    }
}
