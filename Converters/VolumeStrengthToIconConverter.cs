using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Converters
{
    public class VolumeStrengthToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            float volumeStrength = (float)value;

            if(volumeStrength < 0.0001f)
            {
                return "\uE74F"; // Mute icon
            }
            else if(volumeStrength < 0.1)
            {
                return "\uE992"; // Volume 0 icon
            }
            else if (volumeStrength < 0.5)
            {
                return "\uE993"; // Volume 1 icon
            }
            else if(volumeStrength < 0.9)
            {
                return "\uE994"; // Volume 2 icon
            }
            return "\uE995";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
