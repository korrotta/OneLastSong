using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using OneLastSong.Views.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Selectors
{
    public class UserStateTemplateSelector : DataTemplateSelector
    {
        public DataTemplate LoggedInTemplate { get; set; }
        public DataTemplate LoggedOutTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            //return LoggedOutTemplate;

            if (item is bool isUserSignedIn)
            {
                // It is OK if it is null for the first time but not null after that
                return isUserSignedIn ? LoggedInTemplate : LoggedOutTemplate;
            }

            // This is a fallback in case the DataContext is not set correctly
            // The next time it will not be 
            return null;
        }
    }
}
