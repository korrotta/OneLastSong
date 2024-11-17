using Npgsql;
using OneLastSong.Contracts;
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

        public async Task Connect()
        {
            try
            {
                dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
                dataSourceBuilder.EnableRecordsAsTuples();

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

        ~PgDb()
        {
            Dispose();
        }

        private void CheckConnection()
        {
            if (_conn == null)
            {
                throw new InvalidOperationException("Connection not established");
            }
        }

        public async Task<string> UserLogin(string username, string password)
        {
            CheckConnection();

            string res = "";

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_USER_LOGIN))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password);

                    LogUtils.Debug("Executing command: " + QUERY_USER_LOGIN);

                    // Execute and log returned data
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {

                        if (await reader.ReadAsync())
                        {
                            res = reader.GetString(0);
                        }
                        else
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

        public async Task<User> GetUser(string sessionToken)
        {
            CheckConnection();
            User res = new User();
            String userJson = "";

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_USER))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    LogUtils.Debug("Executing command: " + QUERY_GET_USER);

                    // Execute and log returned data
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        LogUtils.Debug("Command executed, reading results...");

                        if (await reader.ReadAsync())
                        {
                            // if number of rows is 0, then the user is not found
                            if(reader.FieldCount == 0)
                            {
                                return null;
                            }

                            userJson = reader.GetString(0);
                            LogUtils.Debug("Raw JSON data: " + userJson);

                            res = User.FromJson(userJson);
                        }
                        else
                        {
                            LogUtils.Debug("No rows returned.");
                        }
                    }
                }

                return res;
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error executing query: {ex.Message}");
                // Error occurred or invalid session token, simply return null
                return null;
            }
        }

        public async Task<ResultMessage> UserSignUp(string username, string password)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_USER_SIGNUP))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password);

                    LogUtils.Debug("Executing command: " + QUERY_USER_SIGNUP);

                    // Execute and log returned data
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        LogUtils.Debug("Command executed, reading results...");

                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            LogUtils.Debug("Raw JSON data: " + json);

                            return ResultMessage.FromJson(json);
                        }
                        else
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

            return null;
        }

        public async Task<ResultMessage> GetMostLikeAudios(int limit = 1000)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_MOST_LIKE_AUDIOS))
                {
                    cmd.Parameters.AddWithValue("limit", limit);

                    LogUtils.Debug("Executing command: " + QUERY_GET_MOST_LIKE_AUDIOS);

                    // Execute and log returned data
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        LogUtils.Debug("Command executed, reading results...");

                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            LogUtils.Debug("Raw JSON data: " + json);

                            return ResultMessage.FromJson(json);
                        }
                        else
                        {
                            throw new Exception("No rows returned. While executing " + QUERY_GET_MOST_LIKE_AUDIOS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error executing query: {ex.Message}");
                throw;
            }
        }

        public async Task<ResultMessage> GetFirstNAlbums(int limit = 20)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_FIRST_N_ALBUMS))
                {
                    cmd.Parameters.AddWithValue("limit", limit);

                    LogUtils.Debug("Executing command: " + QUERY_GET_FIRST_N_ALBUMS);

                    // Execute and log returned data
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        LogUtils.Debug("Command executed, reading results...");

                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            LogUtils.Debug("Raw JSON data: " + json);

                            return ResultMessage.FromJson(json);
                        }
                        else
                        {
                            throw new Exception("No rows returned. While executing " + QUERY_GET_FIRST_N_ALBUMS);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error executing query: {ex.Message}");
                throw;
            }
        }

        public async Task<ResultMessage> GetAllUserPlaylists(string sessionToken)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_ALL_USER_PLAYLISTS))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);

                    LogUtils.Debug("Executing command: " + QUERY_GET_ALL_USER_PLAYLISTS);

                    // Execute and log returned data
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        LogUtils.Debug("Command executed, reading results...");

                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            LogUtils.Debug("Raw JSON data: " + json);

                            return ResultMessage.FromJson(json);
                        }
                        else
                        {
                            throw new Exception("No rows returned. While executing " + QUERY_GET_ALL_USER_PLAYLISTS);
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                LogUtils.Debug($"Error executing query: {ex.Message}");
            };

            return null;
        }

        public async Task<ResultMessage> AddUserPlaylist(string sessionToken, string playlistName, string coverImageUrl)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_ADD_USER_PLAYLIST))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("playlist_name", playlistName);
                    cmd.Parameters.AddWithValue("cover_image_url", coverImageUrl);

                    LogUtils.Debug("Executing command: " + QUERY_ADD_USER_PLAYLIST);

                    // Execute and log returned data
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        LogUtils.Debug("Command executed, reading results...");

                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            LogUtils.Debug("Raw JSON data: " + json);

                            return ResultMessage.FromJson(json);
                        }
                        else
                        {
                            throw new Exception("No rows returned. While executing " + QUERY_ADD_USER_PLAYLIST);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error executing query: {ex.Message}");
                throw;
            }
        }

        public Task<ResultMessage> GetAllArtists()
        {
            CheckConnection();

            try
            {
                return Task.Run(() =>
                {
                    using (var cmd = dataSource.CreateCommand(QUERY_GET_ALL_ARTISTS))
                    {
                        LogUtils.Debug("Executing command: " + QUERY_GET_ALL_ARTISTS);

                        // Execute and log returned data
                        using (var reader = cmd.ExecuteReader())
                        {
                            LogUtils.Debug("Command executed, reading results...");

                            if (reader.Read())
                            {
                                string json = reader.GetString(0);
                                LogUtils.Debug("Raw JSON data: " + json);

                                return ResultMessage.FromJson(json);
                            }
                            else
                            {
                                throw new Exception("No rows returned. While executing " + QUERY_GET_ALL_ARTISTS);
                            }
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                LogUtils.Debug($"Error executing query: {ex.Message}");
                throw;
            }
        }

        // Our Query strings
        public static readonly string QUERY_USER_LOGIN = "SELECT user_login(@username, @password)";
        public static readonly string QUERY_GET_USER = "SELECT get_user_data(@session_token)";
        public static readonly string QUERY_USER_SIGNUP = "SELECT user_signup(@username, @password)";
        public static readonly string QUERY_GET_MOST_LIKE_AUDIOS = "SELECT get_most_like_audios(@limit)";
        public static readonly string QUERY_GET_FIRST_N_ALBUMS = "SELECT get_first_n_albums(@limit)";
        public static readonly string QUERY_GET_ALL_USER_PLAYLISTS = "SELECT get_all_user_playlists(@session_token)";
        public static readonly string QUERY_ADD_USER_PLAYLIST = "SELECT add_user_playlist(@session_token, @playlist_name, @cover_image_url)";
        public static readonly string QUERY_GET_ALL_ARTISTS = "SELECT get_all_artists()";
    }
}
