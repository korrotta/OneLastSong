using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class User
    {
        public User()
        {
        }

        public int Id { get; set; }
        public String Username { get; set; }
        public DateTime CreatedAt { get; set; }
        public String AvatarUrl { get; set; }
        public String ProfileQuote { get; set; }
        public String Description { get; set; }

        public static User FromJson(String json)
        {
            return JsonSerializer.Deserialize<User>(json);
        }
    }
}
