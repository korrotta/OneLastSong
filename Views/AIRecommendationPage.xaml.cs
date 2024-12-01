using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.Contracts;
using OneLastSong.DAOs;
using OneLastSong.Services;
using OneLastSong.Utils;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AIRecommendationPage : Page, IAIChatMessageChangedNotify, IDisposable
    {
        public class MessageItem
        {
            public string Text { get; set; }
            public SolidColorBrush Color { get; set; }
        }

        AIService aiService;

        public AIRecommendationPage()
        {
            this.InitializeComponent();
            aiService = AIService.Get();
            aiService.RegisterChatMessageChangedNotify(this);
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string userInput = InputTextBox.Text;

                if (!string.IsNullOrEmpty(userInput))
                {
                    AddMessageToConversation($"User: {userInput}");
                    InputTextBox.Text = string.Empty;                    

                    // Assemble the chat prompt with a system message and the user's input
                    aiService.SendUserMessage(userInput);
                }
            }
            catch (Exception ex)
            {
                AddMessageToConversation($"GPT: Sorry, something bad happened: {ex.Message}");
            }
            finally
            {
                ResponseProgressBar.Visibility = Visibility.Collapsed;
            }
        }

        private void AddMessageToConversation(string message)
        {
            var messageItem = new MessageItem
            {
                Text = message,
                Color = message.StartsWith("User:") ? ThemeUtils.GetBrush(ThemeUtils.TEXT_PRIMARY)
                                                    : ThemeUtils.GetBrush(ThemeUtils.TEXT_LIGHT)
            };
            ConversationList.Items.Add(messageItem);

            // handle scrolling
            ConversationScrollViewer.UpdateLayout();
            ConversationScrollViewer.ChangeView(null, ConversationScrollViewer.ScrollableHeight, null);
        }

        private void InputTextBox_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter && !string.IsNullOrWhiteSpace(InputTextBox.Text))
            {
                SendButton_Click(this, new RoutedEventArgs());
            }
        }

        public void OnNewMessageToUser(string message)
        {
            AddMessageToConversation($"CoChiller: {message}");
        }

        public void Dispose()
        {
            aiService.UnregisterChatMessageChangedNotify(this);
        }
    }
}
