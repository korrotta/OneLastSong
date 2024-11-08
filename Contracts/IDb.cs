﻿using OneLastSong.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OneLastSong.Contracts
{
    public interface IDb
    {
        public Task<AudioData> GetAudioDataByIdAsync(string id);
        public Task Connect();
        public Task Dispose();
        public Task<string> DoTest();
        public Task<string> UserLogin(string username, string password);
        public Task<User> GetUser(string sessionToken);
    }
}
