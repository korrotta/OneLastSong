﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Models
{
    public class ListeningSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AudioId { get; set; }
        public int Progress { get; set; } = 0;
    }
}
