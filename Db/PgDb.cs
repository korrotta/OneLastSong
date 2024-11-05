using Npgsql;
using OneLastSong.Models;
using OneLastSong.Utils;
using System;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OneLastSong.Db
{
    public class PgDb : IDb
    {
        private NpgsqlConnection _conn = null;
        NpgsqlDataSourceBuilder dataSourceBuilder = null;
        NpgsqlDataSource dataSource = null;
        public String ConnectionString { get; set; } = new DbSpecs().GetQuickConnectionString();

        public Task<AudioData> GetAudioDataByIdAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task Connect()
        {
            try
            {
                dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);

                dataSource = dataSourceBuilder.Build();

                _conn = await dataSource.OpenConnectionAsync();
            }
            catch (Exception e)
            {
                throw new InvalidOperationException("Connection failed", e);
            }

            if (_conn.State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException("Connection not established");
            }
        }

        public async Task<DbDataReader> ExecuteQueryAsync(string query)
        {
            if (_conn == null)
            {
                throw new InvalidOperationException("Connection not established");
            }

            var cmd = _conn.CreateCommand();
            cmd.CommandText = query;

            return await cmd.ExecuteReaderAsync();
        }

        // Close the connection
        public Task Dispose()
        {
            if (_conn != null)
            {
                _conn.Close();
                _conn.Dispose();
            }

            return Task.CompletedTask;
        }

        public async Task<string> UserLogin(string username, string password)
        {
            if (_conn == null)
            {
                throw new InvalidOperationException("Connection not established");
            }

            string res = "";

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_USER_LOGIN))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password);

                    LogUtils.Debug("Executing command: " + QUERY_USER_LOGIN);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        LogUtils.Debug("Command executed, reading results...");

                        while (await reader.ReadAsync())
                        {
                            Debug.WriteLine("Row returned:" + reader.FieldCount.ToString());
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                res += reader.GetString(i) + " ";
                                Debug.WriteLine($"Field {i}: {reader.GetString(i)}");
                            }
                        }

                        if (string.IsNullOrEmpty(res))
                        {
                            LogUtils.Debug("No rows returned.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error executing query: {ex.Message}");
                throw;
            }

            return res;
        }

        public async Task<string> DoTest()
        {
            return await UserLogin("test", "test");

            if (_conn == null)
            {
                throw new InvalidOperationException("Connection not established");
            }

            string res = "";

            try
            {
                await using (var cmd = dataSource.CreateCommand("SELECT name FROM table_name"))
                {
                    cmd.CommandTimeout = 5; // Set a timeout of 5 seconds

                    LogUtils.Debug("Executing command: SELECT name FROM table_name");

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        LogUtils.Debug("Command executed, reading results...");

                        while (await reader.ReadAsync())
                        {
                            Debug.WriteLine("Row returned:" + reader.FieldCount.ToString());
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                res += reader.GetString(i) + " ";
                                Debug.WriteLine($"Field {i}: {reader.GetString(i)}");
                            }
                        }

                        if (string.IsNullOrEmpty(res))
                        {
                            LogUtils.Debug("No rows returned.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error executing query: {ex.Message}");
                throw;
            }

            return res;
        }

        // Our Query strings
        public static readonly string QUERY_USER_LOGIN = "SELECT user_login(@username, @password)";
    }
}
