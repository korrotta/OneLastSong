using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Utils
{
    public class SnackbarUtils
    {
        public static readonly string INFO_ICON = "\uE946";
        public static readonly string SUCCESS_ICON = "\uE8FB";
        public static readonly string WARNING_ICON = "\uE7BA";
        public static readonly string ERROR_ICON = "\uEA39";

        public static void ShowSnackbar(string message, SnackbarType type = SnackbarType.Info, int duration = 3)
        {
            MainWindow mainWindow = (Application.Current as App).MainWindow;
            mainWindow.ShowSnackbar(message, type, duration);
        }
    }
}
