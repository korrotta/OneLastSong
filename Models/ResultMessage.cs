using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class ResultMessage
    {
        public static readonly int STATUS_OK = 0;
        public static readonly int STATUS_ERROR = 1;

        public int Status { get; set; }
        public string ErrorMessage { get; set; }
        public string JsonData { get; set; }
        public static ResultMessage FromJson(string json)
        {
            return System.Text.Json.JsonSerializer.Deserialize<ResultMessage>(json);
        }
    }
}
