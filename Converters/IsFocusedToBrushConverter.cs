using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using OneLastSong.Utils;
using System;
using Windows.UI;

namespace OneLastSong.Converters
{
    public class IsFocusedToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is bool isFocused && isFocused)
            {
                return ThemeUtils.GetBrush(ThemeUtils.SUCCESS_BRUSH);
            }
            return ThemeUtils.GetBrush(ThemeUtils.TEXT_LIGHT); // Default color
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}