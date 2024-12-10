using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.UI
{
    public class SnackbarMessage
    {
        public string Icon { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
        public SolidColorBrush Background { get; set; }
        public Visibility Visibility { get; set; } = Visibility.Visible;
    }
}
