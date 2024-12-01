using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Cores.Classes
{
    public class JsonSuggestedOptionsObj
    {
        [JsonPropertyName("Options")]
        public List<string> Options { get; set; }
    }
}
