using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Converters
{
    public class IsRepeatingToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool isRepeating = (bool)value;

            if (isRepeating)
            {
                return "\uE1CD"; // Repeating icon
            }
            else
            {
                return "\uF5E7"; // Non-repeating icon
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
