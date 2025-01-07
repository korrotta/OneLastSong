using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Utils
{
    public class ConfigValueUtils
    {
        public static readonly String APP_NAME_KEY = "AppName";
        public static readonly String APP_VERSION_KEY = "AppVersion";
        public static readonly String DEFAULT_PLAYLIST_COVER_IMAGE_URL_KEY = "DefaultPlaylistCover";
        public static readonly String LIKE_PLAYLIST_NAME_KEY = "LikePlaylistName";
        public static readonly String NOT_FOUND_IMAGE_KEY = "NotFoundImage";

        public static String GetConfigValue(String key)
        {
            var currentAppDictionary = GetCurrentAppDictionary();

            if(currentAppDictionary.ContainsKey(key))
            {
                return currentAppDictionary[key].ToString();
            }

            return null;
        }

        public static ResourceDictionary GetCurrentAppDictionary()
        {
            return ((App)Application.Current).Resources;
        }
    }
}
