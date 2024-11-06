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
using Microsoft.Extensions.DependencyInjection;
using OneLastSong.Utils;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace OneLastSong.Views.Components
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LeftRegion : Page
    {
        public LeftRegion()
        {
            this.InitializeComponent();
            ExampleList.ItemsSource = GenerateRandomPlaylists(5);
        }

        private List<Playlist> GenerateRandomPlaylists(int count)
        {
            var random = new Random();
            var playlists = new List<Playlist>();

            for (int i = 0; i < count; i++)
            {
                playlists.Add(new Playlist
                {
                    Name = $"Playlist {i + 1}",
                    Count = random.Next(1, 100),
                    Type = random.Next(0, 2) == 0 ? "Music" : "Video"
                });
            }

            return playlists;
        }
    }

}
