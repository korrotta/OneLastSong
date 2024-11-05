using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace OneLastSong.Db
{
    public struct DbSpecs
    {
        public DbSpecs()
        {

        }

        String Database = "postgres";
        String Username = "restricted_user";
        String Server { get; set; } = "127.0.0.1";
        String Host { get; set; } = "127.0.0.1";
        int Port { get; set; } = 5432;
        String Password { get; set; } = "12345678";

        public String GetQuickConnectionString()
        {
            return $"Host={Host};Username={Username};Password={Password};Database={Database}";
        }
    }
}
