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
        ObservableCollection<ChatMessageItem> _conversationList = new ObservableCollection<ChatMessageItem>();
        public ObservableCollection<ChatMessageItem> ConversationList
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
            // only split the first colon
            string[] parts = message.Split(new char[] { ':' }, 2);
            ChatMessageItem newMsg = null;

            if (parts[0].StartsWith("User"))
            {
                newMsg = new ChatMessageItem(ChatMessageItem.Type.Sent, parts[1].Trim());
            }
            else
            {
                newMsg = new ChatMessageItem(ChatMessageItem.Type.Received, parts[1].Trim());
            }
            ConversationList.Add(newMsg);
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
            SendMessage(InputTextBoxText);
            InputTextBoxText = string.Empty;
        }

        internal void SendMessage(string msg)
        {
            try
            {
                ResponseProgressBar = Visibility.Visible;
                if (!string.IsNullOrEmpty(msg))
                {
                    AddMessageToConversation($"User: {msg}");
                    aiService.SendUserMessage(msg);
                }
                SuggestedActions.Clear();
            }
            catch (Exception ex)
            {
                AddMessageToConversation($"CoChiller: Sorry, something bad happened: {ex.Message}");
            }
            finally
            {

            }
        }
    }
}
