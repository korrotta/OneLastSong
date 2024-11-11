using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using OneLastSong.Models;
using OneLastSong.DAOs;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page
    {
        public HomePage()
        {
            this.InitializeComponent();
            Loaded += HomePage_Loaded;
        }

        private async void HomePage_Loaded(object sender, RoutedEventArgs e)
        {
            List<Audio> listAudios = await AudioDAO.Get().GetMostLikeAudios();
            DiscoveryList.ItemsSource = listAudios;

            List<Album> listAlbums = await AlbumDAO.Get().GetMostLikeAlbums();
            AlbumList.ItemsSource = listAlbums;
        }

        private List<CustomDataObject> GenerateRandomData(int count)
        {
            var random = new Random();
            var dataList = new List<CustomDataObject>();

            for (int i = 0; i < count; i++)
            {
                dataList.Add(new CustomDataObject
                {
                    Title = $"Title {i + 1}",
                    ImageLocation = "/Assets/LikedPlaylist.png",
                    Views = random.Next(0, 10000).ToString(),
                    Likes = random.Next(0, 5000).ToString(),
                    Description = $"Description for item {i + 1}"
                });
            }

            return dataList;
        }
    }
}
