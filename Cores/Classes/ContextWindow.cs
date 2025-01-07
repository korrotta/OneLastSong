using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Cores.Classes
{
    class ContextWindow
    {
        private List<ChatMessage> _conversationHistory;
        private readonly int MAX_CONTEXT_WINDOW_SIZE = 8;
        private readonly int MAX_CONTEXT_WINDOW_TOKENS = 1024;

        public ContextWindow()
        {
            _conversationHistory = new List<ChatMessage>();
        }

        public void Add(ChatMessage message)
        {
            _conversationHistory.Add(message);
            //if (_conversationHistory.Count > MAX_CONTEXT_WINDOW_SIZE)
            //{
            //    _conversationHistory.RemoveAt(0);
            //}

            //int totalTokens = 0;
            //for (int i = 0; i < _conversationHistory.Count; i++)
            //{
            //    totalTokens += _conversationHistory[i].Content.Count;
            //    if (totalTokens > MAX_CONTEXT_WINDOW_TOKENS)
            //    {
            //        _conversationHistory.RemoveRange(0, i);
            //        break;
            //    }
            //}
        }

        public List<ChatMessage> GetContextWindow()
        {
            return _conversationHistory;
        }
    }
}
