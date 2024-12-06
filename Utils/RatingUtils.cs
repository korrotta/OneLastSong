using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Utils
{
    public class RatingUtils
    {
        public static string GetSentimentLevel(float score)
        {
            if (score < 1.5)
            {
                return "Very Bad";
            }
            else if (score < 2.5)
            {
                return "Bad";
            }
            else if (score < 3.5)
            {
                return "Normal";
            }
            else if (score < 4.5)
            {
                return "Good";
            }
            else
            {
                return "Very Good";
            }
        }
    }
}
