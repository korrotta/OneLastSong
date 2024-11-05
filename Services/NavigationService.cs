using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using OneLastSong.Views.Components;
using Microsoft.UI.Xaml.Media.Animation;
using OneLastSong.Contracts;

namespace OneLastSong.Services
{
    public class NavigationService
    {
        private Frame _frame;
        private Stack<(Type PageType, object Parameter)> _backStack = new();
        private Stack<(Type PageType, object Parameter)> _forwardStack = new();
        private List<INavChangeNotifier> navChangeNotifiers = new();

        public NavigationService() { }

        public void Initialize(Frame frame)
        {
            _frame = frame;
        }

        public bool CanGoBack => _backStack.Count > 0;
        public bool CanGoForward => _forwardStack.Count > 0;

        public void RegisterNavChangeNotifier(INavChangeNotifier navChangeNotifier)
        {
            navChangeNotifiers.Add(navChangeNotifier);
        }

        public void UnregisterNavChangeNotifier(INavChangeNotifier navChangeNotifier)
        {
            navChangeNotifiers.Remove(navChangeNotifier);
        }

        public void GoBack()
        {
            if (CanGoBack)
            {
                var currentEntry = (_frame.CurrentSourcePageType, _frame.GetNavigationState());
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
                NotifyNavChangeNotifiers();
            }
        }

        public void GoForward()
        {
            if (CanGoForward)
            {
                var currentEntry = (_frame.CurrentSourcePageType, _frame.GetNavigationState());
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
                NotifyNavChangeNotifiers();
            }
        }

        public void Navigate(Type pageType, object parameter = null)
        {
            // if requested page is the same as current page, do nothing
            if (_frame.CurrentSourcePageType == pageType)
            {
                return;
            }

            if (_frame.CurrentSourcePageType != null)
            {
                var currentEntry = (_frame.CurrentSourcePageType, _frame.GetNavigationState());
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
        }

        private void NotifyNavChangeNotifiers()
        {
            foreach (var navChangeNotifier in navChangeNotifiers)
            {
                navChangeNotifier.OnNavHistoryChanged(this);
            }
        }

        public static NavigationService Get()
        {
            return (NavigationService)((App)Application.Current).Services.GetService(typeof(NavigationService));
        }
    }
}
