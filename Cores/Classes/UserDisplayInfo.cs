using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OneLastSong.Cores.Classes
{
    public class UserDisplayInfo
    {
        [JsonPropertyName("UserId")]
        public int UserId { get; set; }
        [JsonPropertyName("DisplayName")]
        public string DisplayName { get; set; }
        [JsonPropertyName("AvatarUrl")]
        public string AvatarUrl { get; set; }
    }
}
