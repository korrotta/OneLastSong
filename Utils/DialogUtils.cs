using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;

namespace OneLastSong.Utils
{
    public class DialogUtils
    {
        public static async Task ShowDialogAsync(string title, string content, XamlRoot xamlRoot)
        {
            ContentDialog dialog = new ContentDialog
            {
                Title = title,
                Content = content,
                CloseButtonText = "OK",
                XamlRoot = xamlRoot,
            };

            await dialog.ShowAsync();
        }
    }
}
