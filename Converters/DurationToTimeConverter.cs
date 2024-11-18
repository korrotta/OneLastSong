using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Converters
{
    public class DurationToTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int duration = (int)value;
            int minutes = duration / 60;
            int seconds = duration % 60;

            // Format the result as "mm:ss"
            string result = string.Format("{0:D2}:{1:D2}", minutes, seconds);

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string time = value.ToString();
            string[] parts = time.Split(':');
            if (parts.Length == 1)
            {
                return int.Parse(parts[0]) * 60;
            }
            else
            {
                return int.Parse(parts[0]) * 60 + int.Parse(parts[1]);
            }
        }
    }
}
