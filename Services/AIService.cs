using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using OneLastSong.Contracts;
using OneLastSong.Utils;
using OpenAI;
using OpenAI.Assistants;
using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneLastSong.Services
{
    public class AIService : IDisposable, INotifySubsytemStateChanged
    {
        private DispatcherQueue _eventHandler;
        private OpenAIClient _client;
        ChatClient _chatClient;
        private List<ChatMessage> _conversationHistory;
        ChatCompletionOptions _chatOptions;
        List<IAIChatMessageChangedNotify> _chatMessageChangedNotify = new List<IAIChatMessageChangedNotify>();

        public AIService(DispatcherQueue dispatcherQueue)
        {
            _eventHandler = dispatcherQueue;
            _conversationHistory = new List<ChatMessage>();
            _conversationHistory.Add(ChatMessage.CreateSystemMessage(
                "You are a helpful assistant can help user to control a desktop music application \"OneLastSong\" like Spotify" +
                "You can control the application on behave of the user including change Play/Pause state, Play asking playlist by user if exists" +
                "Play a requested song from user by name when user asks" +
                "Play a random mashup based on user suggestions like genres, artists, categories, ..." +
                "You can response the system with recommended options so that the system can display for the user to choose next action instead of typing" +
                "After handle user requests you should response what you have done politely" +
                "If there is no function you can call to execute the user's request you should say it is out of your scope and politely apologize user and recommend what you can do"));
            _chatOptions = GetChatOptions();
        }

        public static AIService Get()
        {
            return (AIService)((App)Application.Current).Services.GetService(typeof(AIService));
        }

        public void Dispose()
        {
            // Clean up
        }

        public async Task<bool> OnSubsystemInitialized()
        {
            var openAiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            if (openAiKey == null)
            {
                // Contact me to get the key
                throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set");
            }

            // Initialization
            _client = new OpenAIClient(openAiKey);
            _chatClient = GetCurrentChatClient();
            
            await Task.CompletedTask;
            return true;
        }

        public OpenAIClient GetClient()
        {
            return _client;
        }

        public ChatCompletionOptions GetChatOptions()
        {
            var chatOptions = new ChatCompletionOptions
            {
                MaxOutputTokenCount = 400,
                AllowParallelToolCalls = false
            };

            chatOptions.Tools.Add(
                ChatTool.CreateFunctionTool(
                    "GetPlayState",
                    "Get the state of the music app: Play/Pause"));
            chatOptions.Tools.Add(
                ChatTool.CreateFunctionTool(
                    "Play",
                    "Change to play state"));
            chatOptions.Tools.Add(
                ChatTool.CreateFunctionTool(
                    "Pause",
                    "Change to pause state"));
            chatOptions.Tools.Add(
                ChatTool.CreateFunctionTool(
                    "ShowOptionsSuggestion",
                    "Show prompt suggestions for user to choose from instead of typing manually",
                    BinaryData.FromBytes("""
                    {
                        "type": "object",
                        "properties": {
                            "options": {
                                "description": "an array of string contains possible actions user can take",
                                "type": "array",
                                "items": {
                                    "type": "string"
                                }
                            }
                        },
                        "additionalProperties": false,
                        "required": ["options"]
                    }
                    """u8.ToArray()),
                    true
                )
            );

            return chatOptions;
        }

        private ChatClient GetCurrentChatClient()
        {
            return _client.GetChatClient("gpt-4o-mini");
        }

        public async void SendUserMessage(string userInput)
        {
            var chatClient = _chatClient;
            _conversationHistory.Add(ChatMessage.CreateUserMessage(userInput));
            // Assemble the chat prompt with a system message and the user's input
            var completionResult = await chatClient.CompleteChatAsync(
                _conversationHistory,
                _chatOptions);

            var toolCalls = completionResult.Value.ToolCalls;

            if (completionResult.Value.FinishReason ==  ChatFinishReason.ToolCalls)
            {
                _conversationHistory.Add(ChatMessage.CreateAssistantMessage(toolCalls));
                foreach (var toolCall in toolCalls)
                {
                    HandleToolCall(toolCall);
                }
            }

            if (IsValidCompletionResult(completionResult))
            {
                string msg = completionResult.Value.Content.First().Text;
                _conversationHistory.Add(ChatMessage.CreateAssistantMessage(msg));
                NotifyNewMessageToUser(msg);
            }
        }

        private bool IsValidCompletionResult(ClientResult<ChatCompletion> completionResult)
        {
            return completionResult != null && completionResult.Value.Content.Count > 0;
        }

        private void NotifyNewMessageToUser(string userMsg)
        {
            _eventHandler.TryEnqueue(() =>
            {
                foreach (var notify in _chatMessageChangedNotify)
                {
                    notify.OnNewMessageToUser(userMsg);
                }
            });
        }

        private async void HandleToolCall(ChatToolCall toolCall)
        {
            LogUtils.Debug($"Tool call: {toolCall.FunctionName}");
            LogUtils.Debug($"Tool call parameters: {toolCall.FunctionArguments}");

            string functionCallResult = "";

            switch (toolCall.FunctionName)
            {
                case "GetPlayState":
                    functionCallResult = "The music app is currently playing";
                    break;
                case "Play":
                    functionCallResult = "The music app is now playing";
                    break;
                case "Pause":
                    functionCallResult = "The music app is now paused";
                    break;
                case "ShowOptionsSuggestion":
                    functionCallResult = "Here are some options for you to choose from: " + string.Join(", ", toolCall.FunctionArguments);
                    break;
                default:
                    functionCallResult = "I'm sorry, I can't do that";
                    break;
            }
            _conversationHistory.Add(ChatMessage.CreateToolMessage(toolCall.Id, functionCallResult));
            // send function result back to OpenAI
            var response = await _chatClient.CompleteChatAsync(_conversationHistory);

            if(IsValidCompletionResult(response))
            {
                string msg = response.Value.Content.First().Text;
                _conversationHistory.Add(ChatMessage.CreateAssistantMessage(msg));
                NotifyNewMessageToUser(msg);
            }
        }

        public void RegisterChatMessageChangedNotify(IAIChatMessageChangedNotify notify)
        {
            _chatMessageChangedNotify.Add(notify);
        }

        public void UnregisterChatMessageChangedNotify(IAIChatMessageChangedNotify notify)
        {
            _chatMessageChangedNotify.Remove(notify);
        }
    }
}
