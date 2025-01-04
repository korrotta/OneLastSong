using Microsoft.UI.Xaml;
using Npgsql;
using OneLastSong.Contracts;
using OneLastSong.Models;
using OneLastSong.Utils;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace OneLastSong.Db
{
    public class PgDb : IDb
    {
        private NpgsqlConnection _conn = null;
        private NpgsqlDataSource dataSource = null;
        public string ConnectionString { get; set; } = new DbSpecs().GetQuickConnectionString();

        public async Task Connect()
        {
            try
            {
                var dataSourceBuilder = new NpgsqlDataSourceBuilder(ConnectionString);
                dataSourceBuilder.EnableRecordsAsTuples();
                dataSource = dataSourceBuilder.Build();
                _conn = await dataSource.OpenConnectionAsync();

                if (_conn.State != System.Data.ConnectionState.Open)
                {
                    throw new InvalidOperationException("Connection not established");
                }
            }
            catch (Exception e)
            {
                LogUtils.Error(e.Message);
                throw new Exception("Connection to database failed!");
            }
        }

        public async Task<DbDataReader> ExecuteQueryAsync(string query)
        {
            CheckConnection();
            var cmd = _conn.CreateCommand();
            cmd.CommandText = query;
            return await cmd.ExecuteReaderAsync();
        }

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

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            res = reader.GetString(0);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return res;
        }

        public async Task<string> DoTest()
        {
            CheckConnection();
            string res = "";

            try
            {
                await using (var cmd = dataSource.CreateCommand("SELECT name FROM table_name"))
                {
                    cmd.CommandTimeout = 5; // Set a timeout of 5 seconds

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                res += reader.GetString(i) + " ";
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return res;
        }

        public async Task<User> GetUser(string sessionToken)
        {
            CheckConnection();
            User res = null;

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_USER))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string userJson = reader.GetString(0);
                            res = User.FromJson(userJson);
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return res;
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

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
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

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetFirstNAlbums(int limit = 20)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_FIRST_N_ALBUMS))
                {
                    cmd.Parameters.AddWithValue("limit", limit);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetAllUserPlaylists(string sessionToken)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_ALL_USER_PLAYLISTS))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

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

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetAllArtists()
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_ALL_ARTISTS))
                {
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> AddAudioToPlaylist(string sessionToken, int playlistId, int audioId)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_ADD_AUDIO_TO_PLAYLIST))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("playlist_id", playlistId);
                    cmd.Parameters.AddWithValue("audio_id", audioId);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> RemoveAudioFromPlaylist(string sessionToken, int playlistId, int audioId)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_REMOVE_AUDIO_FROM_PLAYLIST))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("playlist_id", playlistId);
                    cmd.Parameters.AddWithValue("audio_id", audioId);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> DeletePlaylist(string sessionToken, int playlistId)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_DELETE_PLAYLIST))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("playlist_id", playlistId);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> SaveListeningSession(string sessionToken, int audioId, int progress)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_SAVE_LISTENING_SESSION))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("audio_id", audioId);
                    cmd.Parameters.AddWithValue("progress", progress);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetListeningSession(string sessionToken)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_LISTENING_SESSION))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);

                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetLyrics(int audioId)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_LYRICS))
                {
                    cmd.Parameters.AddWithValue("audio_id", audioId);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetAudioById(int audioId)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_AUDIO_BY_ID))
                {
                    cmd.Parameters.AddWithValue("audio_id", audioId);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> CommentAudio(string sessionToken, int audioId, string comment)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_COMMENT_AUDIO))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("audio_id", audioId);
                    cmd.Parameters.AddWithValue("comment", comment);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetCommentsByAudioId(int audioId)
        {
            CheckConnection();
            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_COMMENTS_BY_AUDIO_ID))
                {
                    cmd.Parameters.AddWithValue("audio_id", audioId);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<ResultMessage> RateAudio(string sessionToken, int audioId, float rating)
        {
            CheckConnection();
            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_RATE_AUDIO))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("audio_id", audioId);
                    cmd.Parameters.AddWithValue("rating", rating);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<ResultMessage> GetRatingScoreByAudioId(int audioId)
        {
            CheckConnection();
            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_RATING_SCORE_BY_AUDIO_ID))
                {
                    cmd.Parameters.AddWithValue("audio_id", audioId);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return null;
        }

        public async Task<ResultMessage> GetUserDisplayInfo(int userId)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_USER_DISPLAY_INFO))
                {
                    cmd.Parameters.AddWithValue("user_id", userId);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetUserAudioRating(int userId, int audioId)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_USER_AUDIO_RATING))
                {
                    cmd.Parameters.AddWithValue("user_id", userId);
                    cmd.Parameters.AddWithValue("audio_id", audioId);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> AddUserPlayHistory(string sessionToken, int audioId)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_ADD_USER_PLAY_HISTORY))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("audio_id", audioId);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetUserPlayHistory(string sessionToken)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_USER_PLAY_HISTORY))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> GetAudiosInPlaylist(string sessionToken, int playlistId)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_GET_AUDIOS_IN_PLAYLIST))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("playlist_id", playlistId);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
        }

        public async Task<ResultMessage> UpdatePlaylist(string sessionToken, int playlistId, string name, string coverImageUrl)
        {
            CheckConnection();

            try
            {
                await using (var cmd = dataSource.CreateCommand(QUERY_UPDATE_USER_PLAYLIST))
                {
                    cmd.Parameters.AddWithValue("session_token", sessionToken);
                    cmd.Parameters.AddWithValue("playlist_id", playlistId);
                    cmd.Parameters.AddWithValue("name", name);
                    cmd.Parameters.AddWithValue("cover_image_url", coverImageUrl);
                    await using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            string json = reader.GetString(0);
                            return ResultMessage.FromJson(json);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return null;
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
        public static readonly string QUERY_ADD_AUDIO_TO_PLAYLIST = "SELECT add_audio_to_playlist(@session_token, @playlist_id, @audio_id)";
        public static readonly string QUERY_REMOVE_AUDIO_FROM_PLAYLIST = "SELECT remove_audio_from_playlist(@session_token, @playlist_id, @audio_id)";
        public static readonly string QUERY_DELETE_PLAYLIST = "SELECT delete_playlist(@session_token, @playlist_id)";
        public static readonly string QUERY_SAVE_LISTENING_SESSION = "SELECT save_listening_session(@session_token, @audio_id, @progress)";
        public static readonly string QUERY_GET_LISTENING_SESSION = "SELECT get_listening_session(@session_token)";
        public static readonly string QUERY_GET_LYRICS = "SELECT get_lyrics(@audio_id)";
        public static readonly string QUERY_GET_AUDIO_BY_ID = "SELECT get_audio_by_id(@audio_id)";
        public static readonly string QUERY_COMMENT_AUDIO = "SELECT comment_audio(@session_token, @audio_id, @comment)";
        public static readonly string QUERY_GET_COMMENTS_BY_AUDIO_ID = "SELECT get_comments_by_audio_id(@audio_id)";
        public static readonly string QUERY_RATE_AUDIO = "SELECT rate_audio(@session_token, @audio_id, @rating)";
        public static readonly string QUERY_GET_RATING_SCORE_BY_AUDIO_ID = "SELECT get_rating_score_by_audio_id(@audio_id)";
        public static readonly string QUERY_GET_USER_DISPLAY_INFO = "SELECT get_user_display_info(@user_id)";
        public static readonly string QUERY_GET_USER_AUDIO_RATING = "SELECT get_user_audio_rating(@user_id, @audio_id)";
        public static readonly string QUERY_ADD_USER_PLAY_HISTORY = "SELECT add_user_play_history(@session_token, @audio_id)";
        public static readonly string QUERY_GET_USER_PLAY_HISTORY = "SELECT get_user_play_history(@session_token)";
        public static readonly string QUERY_GET_AUDIOS_IN_PLAYLIST = "SELECT get_audios_in_playlist(@session_token, @playlist_id)";
        public static readonly string QUERY_UPDATE_USER_PLAYLIST = "SELECT update_user_playlist(@session_token, @playlist_id, @name, @cover_image_url)";
    }
}
