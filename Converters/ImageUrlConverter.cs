using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using OneLastSong.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Converters
{
    public class ImageUrlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            string imageUrl = value as string;
            string res = imageUrl;
            if(!ImageUtils.IsValidImageUrl(imageUrl))
            {
                res = $"https://firebasestorage.googleapis.com/v0/b/onelastsong-5d5a8.appspot.com/o/images%2Fnot_found.jpg?alt=media&token=021931c5-b8c9-4b01-9ea2-8bb456b969d6";
            }

            // return image source
            return new BitmapImage(new Uri(res));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
