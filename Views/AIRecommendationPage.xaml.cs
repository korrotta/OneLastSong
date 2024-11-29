using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.DAOs;
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
    public sealed partial class AIRecommendationPage : Page
    {
        OpenAIClient openAiService;

        public AIRecommendationPage()
        {
            this.InitializeComponent();
            openAiService = (OpenAIClient)((App)Application.Current).Services.GetService(typeof(OpenAIClient));
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string userInput = InputTextBox.Text;

                if (!string.IsNullOrEmpty(userInput))
                {
                    AddMessageToConversation($"User: {userInput}");
                    InputTextBox.Text = string.Empty;
                    var chatClient = openAiService.GetChatClient("gpt-4o"); // or another model
                    var chatOptions = new ChatCompletionOptions
                    {
                        MaxOutputTokenCount = 300
                    };

                    // Assemble the chat prompt with a system message and the user's input
                    var completionResult = await chatClient.CompleteChatAsync(
                        [
                            ChatMessage.CreateSystemMessage("You are a helpful assistant."),
                        ChatMessage.CreateUserMessage(userInput)
                        ],
                        chatOptions);

                    if (completionResult != null && completionResult.Value.Content.Count > 0)
                    {
                        AddMessageToConversation($"GPT: {completionResult.Value.Content.First().Text}");
                    }
                    else
                    {
                        AddMessageToConversation($"GPT: Sorry, something bad happened: {completionResult?.Value.Refusal ?? "Unknown error."}");
                    }
                }
            }
            catch (Exception ex)
            {
                AddMessageToConversation($"GPT: Sorry, something bad happened: {ex.Message}");
            }
        }

        private void AddMessageToConversation(string message)
        {
            ConversationList.Items.Add(message);
            ConversationList.ScrollIntoView(ConversationList.Items.Last());
        }
    }
}
