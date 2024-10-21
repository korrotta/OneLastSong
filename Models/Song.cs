using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace OneLastSong.Models
{
    public class Song : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }
        public TimeSpan Duration { get; set; }
        public ImageSource Thumbnail { get; set; }
        public string FilePath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task loadMetadata(string filepath)
        {
            // Load metadata from file
            var File = TagLib.File.Create(filepath);
            Title = File.Tag.Title ?? "Unknown Title";
            Artist = File.Tag.FirstPerformer ?? "Unknown Artist";
            Album = File.Tag.Album ?? "Unknown Album";
            Duration = File.Properties.Duration;

            // Set thumbnail
            if (File.Tag.Pictures.Length > 0)
            {
                var bin = (byte[])(File.Tag.Pictures[0].Data.Data);
                Thumbnail = await ConvertImageFromBytesAsync(bin);
            }

            // Set file path
            FilePath = filepath;
        }

        private async Task<ImageSource> ConvertImageFromBytesAsync(byte[] imageData)
        {
            using (var stream = new MemoryStream(imageData))
            {
                var bitmapImage = new BitmapImage();

                // Create a random access stream from the byte array
                using (var randomAccessStream = new InMemoryRandomAccessStream())
                {
                    await randomAccessStream.WriteAsync(imageData.AsBuffer());
                    await randomAccessStream.FlushAsync();
                    randomAccessStream.Seek(0);

                    // Set the stream source for the bitmap image
                    await bitmapImage.SetSourceAsync(randomAccessStream);
                }

                return bitmapImage;
            }
        }
    }
}
