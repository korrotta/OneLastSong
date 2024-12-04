using Microsoft.UI.Xaml.Media;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Cores.DataItems
{
    public class ChatMessageItem
    {
        public enum Type
        {
            Sent,
            Received
        }

        private Type _type;
        private string _message;
        private DateTime _time = DateTime.Now;
        private SolidColorBrush _color;
        private SolidColorBrush _bgColor;

        public ChatMessageItem(Type type, string message)
        {
            _type = type;
            _message = message;
            _bgColor = type == Type.Sent ? ThemeUtils.GetBrush(ThemeUtils.BG_TERTIARY)
                                                    : ThemeUtils.GetBrush(ThemeUtils.BG_LIGHT_TERTIARY);
            _color = ThemeUtils.GetBrush(ThemeUtils.TEXT_PRIMARY);
        }

        public string MsgText => _message;
        public string MsgDateTime => _time.ToString("MM/dd/yyyy hh:mm tt");
        public string MsgAlignment => _type == Type.Sent ? "Right" : "Stretch";
        public SolidColorBrush Color => _color;
        public SolidColorBrush BgColor => _bgColor;
    }
}
