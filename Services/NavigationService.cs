﻿using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using OneLastSong.Views.Components;
using Microsoft.UI.Xaml.Media.Animation;
using OneLastSong.Contracts;
using Microsoft.UI.Dispatching;
using OneLastSong.Views;

namespace OneLastSong.Services
{
    public class NavigationService : IDisposable, INotifySubsytemStateChanged
    {
        private Frame _frame;
        private Stack<(Type PageType, object Parameter)> _backStack = new();
        private Stack<(Type PageType, object Parameter)> _forwardStack = new();
        private object _currentParameter = null;
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
                _currentParameter = parameter;
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
                _currentParameter = parameter;
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

        public Page Navigate(Type pageType, object parameter = null, bool forcedNew = false)
        {
            // if requested page is the same as current page, do nothing
            if (_frame.CurrentSourcePageType == pageType && !forcedNew)
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
            _currentParameter = parameter;
            _frame.Navigate(pageType, parameter);

            // Load state
            {
                if (_frame.Content is INavigationStateSavable savable)
                {
                    savable.OnStateLoad(parameter);
                }
            }

            NotifyNavChangeNotifiers();

            // Return the instance of the navigated-to page
            return _frame.Content as Page;
        }

        public void NavigateOrReloadOnParameterChanged(Type pageType, object parameter)
        {
            if (_frame.CurrentSourcePageType == pageType)
            {
                if (_frame.Content is INavigationStateSavable savable && !_currentParameter.Equals(parameter))
                {
                    // update current page parameter
                    _currentParameter = parameter;
                    savable.OnStateLoad(parameter);
                }
            }
            else
            {
                Navigate(pageType, parameter);
            }
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

        internal Page GetCurrentPage()
        {
            return _frame.Content as Page;
        }

        public object GetCurrentParameter()
        {
            return _currentParameter;
        }

        public bool IsAsPageType(Type pageType)
        {
            if(_frame == null)
            {
                return false;
            }

            return _frame.CurrentSourcePageType == pageType;
        }
    }
}
