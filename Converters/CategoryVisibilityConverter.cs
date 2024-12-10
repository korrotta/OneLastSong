using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using OneLastSong.Utils;
using OneLastSong.ViewModels;
using System;

namespace OneLastSong.Converters
{
    public class CategoryVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            LogUtils.Debug($"CategoryVisibilityConverter: {value} and {parameter}");
            
            if (value == null || parameter == null)
                return Visibility.Collapsed;

            string currentCategory = value.ToString();
            string targetCategory = parameter.ToString();


            if (currentCategory == SearchPageViewModel.ALL_CATEGORY)
            {
                return Visibility.Visible;
            }

            return currentCategory.Equals(targetCategory, StringComparison.OrdinalIgnoreCase) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}