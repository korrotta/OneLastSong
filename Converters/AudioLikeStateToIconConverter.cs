using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OneLastSong.Cores.DataItems.AudioItem;

namespace OneLastSong.Converters
{
    public class AudioLikeStateToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            AudioLikeStateType audioLikeStateType = (AudioLikeStateType)value;

            if (audioLikeStateType == AudioLikeStateType.Fetching)
            {
                return "\uF143"; // Loading icon
            }
            else if (audioLikeStateType == AudioLikeStateType.Liked)
            {
                return "\uEB52"; // Heart icon filled
            }
            else
            {
                return "\uEB51"; // Heart icon outline
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
