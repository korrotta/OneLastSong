using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using OneLastSong.Contracts;
using OneLastSong.Cores.DataItems;
using OneLastSong.Services;
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
    public class AIRecommendationPageViewModel : INotifyPropertyChanged, IAIChatMessageChangedNotify, IDisposable
    {
        ObservableCollection<MessageItem> _conversationList = new ObservableCollection<MessageItem>();
        public ObservableCollection<MessageItem> ConversationList
        {
            get => _conversationList;
            set
            {
                if (_conversationList != value)
                {
                    _conversationList = value;
                    OnPropertyChanged(nameof(ConversationList));
                }
            }
        }

        string _inputTextBoxText = string.Empty;
        public string InputTextBoxText
        {
            get => _inputTextBoxText;
            set
            {
                if (_inputTextBoxText != value)
                {
                    _inputTextBoxText = value;
                    OnPropertyChanged(nameof(InputTextBoxText));
                }
            }
        }

        Visibility _responseProgressBarVisibility = Visibility.Collapsed;
        public Visibility ResponseProgressBar
        {
            get => _responseProgressBarVisibility;
            set
            {
                if (_responseProgressBarVisibility != value)
                {
                    _responseProgressBarVisibility = value;
                    OnPropertyChanged(nameof(ResponseProgressBar));
                }
            }
        }

        AIService aiService;

        public AIRecommendationPageViewModel()
        {
            aiService = AIService.Get();
            aiService.RegisterChatMessageChangedNotify(this);
        }

        private ObservableCollection<string> _suggestedActions = new ObservableCollection<string>();

        public ObservableCollection<string> SuggestedActions
        {
            get => _suggestedActions;
            set
            {
                if (_suggestedActions != value)
                {
                    _suggestedActions = value;
                    OnPropertyChanged(nameof(SuggestedActions));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal void UpdateSuggestedActions(List<string> actions)
        {
            SuggestedActions.Clear();
            foreach (var action in actions)
            {
                SuggestedActions.Add(action);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void AddMessageToConversation(string message)
        {
            var messageItem = new MessageItem
            {
                Text = message,
                Color = message.StartsWith("User:") ? ThemeUtils.GetBrush(ThemeUtils.TEXT_PRIMARY)
                                                    : ThemeUtils.GetBrush(ThemeUtils.TEXT_LIGHT)
            };
            ConversationList.Add(messageItem);
        }

        public void OnNewMessageToUser(string message)
        {
            AddMessageToConversation($"CoChiller: {message}");
            ResponseProgressBar = Visibility.Collapsed;
        }

        public void OnSuggestionActionsChanged(List<string> actions)
        {
            UpdateSuggestedActions(actions);
        }

        public void Dispose()
        {
            aiService.UnregisterChatMessageChangedNotify(this);
        }

        internal void HandleUserChat()
        {
            try
            {
                ResponseProgressBar = Visibility.Visible;
                if (!string.IsNullOrEmpty(InputTextBoxText))
                {
                    AddMessageToConversation($"User: {InputTextBoxText}");
                    InputTextBoxText = string.Empty;

                    // Assemble the chat prompt with a system message and the user's input
                    aiService.SendUserMessage(InputTextBoxText);
                }
            }
            catch (Exception ex)
            {
                AddMessageToConversation($"GPT: Sorry, something bad happened: {ex.Message}");
            }
            finally
            {
                
            }
        }
    }
}
