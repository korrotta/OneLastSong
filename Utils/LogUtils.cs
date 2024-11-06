using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Utils
{
    public class LogUtils
    {
        public static void Debug(string message)
        {
            System.Diagnostics.Debug.WriteLine(message, "DEBUG");
        }
    }
}
