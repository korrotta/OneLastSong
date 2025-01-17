-- Active: 1730359393972@@127.0.0.1@5432@postgres@public
SELECT now();

-- Drop existing tables if they exist
DROP TABLE IF EXISTS users CASCADE;

DROP TABLE IF EXISTS playlists CASCADE;

DROP TABLE IF EXISTS audios CASCADE;

DROP TABLE IF EXISTS playlist_audios CASCADE;

DROP TABLE IF EXISTS likes CASCADE;

DROP TABLE IF EXISTS listening_sessions CASCADE;

DROP TABLE IF EXISTS user_settings CASCADE;

DROP TABLE IF EXISTS albums CASCADE;

DROP TABLE IF EXISTS categories CASCADE;

DROP TABLE IF EXISTS friends CASCADE;

DROP TABLE IF EXISTS followers CASCADE;

-- Extensions
CREATE EXTENSION IF NOT EXISTS pgcrypto;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Users table
CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    avatar_url VARCHAR(255),
    profile_quote TEXT,
    is_artist BOOLEAN DEFAULT FALSE,
    description TEXT
);

-- Sessions table
CREATE TABLE sessions (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    session_token VARCHAR(255) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP NOT NULL
);

-- Albums table
CREATE TABLE albums (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    artist VARCHAR(255) NOT NULL,
    cover_image_url VARCHAR(255),
    release_date DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    user_id INTEGER REFERENCES users (id) ON DELETE SET NULL
);

-- Categories table
CREATE TABLE categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Audios table
CREATE TABLE audios (
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    artist VARCHAR(255) NOT NULL,
    album_id INTEGER REFERENCES albums (id) ON DELETE SET NULL,
    category_id INTEGER REFERENCES categories (id) ON DELETE SET NULL,
    duration INTEGER NOT NULL, -- Duration in seconds
    url VARCHAR(255) NOT NULL,
    cover_image_url VARCHAR(255),
    author_id INTEGER REFERENCES users (id) ON DELETE SET NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    country_id VARCHAR(2) REFERENCES countries (id) DEFAULT '__',
    description TEXT
);

-- Genres table
CREATE TABLE genres (
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL
);

-- Audios_Genres table to establish many-to-many relationship
CREATE TABLE audios_genres (
    audio_id INTEGER REFERENCES audios (id) ON DELETE CASCADE,
    genre_id INTEGER REFERENCES genres (id) ON DELETE CASCADE,
    PRIMARY KEY (song_id, genre_id)
);

-- Countries table
CREATE TABLE countries (
    id VARCHAR(2) PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL
);

-- Playlists table
CREATE TABLE playlists (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    cover_image_url VARCHAR(255),
    deletable BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
);

-- Playlist audios table (many-to-many relationship between playlists and audios)
CREATE TABLE playlist_audios (
    playlist_id INTEGER REFERENCES playlists (id) ON DELETE CASCADE,
    audio_id INTEGER REFERENCES audios (id) ON DELETE CASCADE,
    added_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (playlist_id, audio_id)
);

-- Likes table (tracks which audios users have liked)
CREATE TABLE likes (
    user_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    audio_id INTEGER REFERENCES audios (id) ON DELETE CASCADE,
    liked_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, audio_id)
);

-- Listening sessions table
CREATE TABLE listening_sessions (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    audio_id INTEGER REFERENCES audios (id) ON DELETE CASCADE,
    progress INTEGER DEFAULT 0
);

-- User settings table
CREATE TABLE user_settings (
    user_id INTEGER PRIMARY KEY REFERENCES users (id) ON DELETE CASCADE,
    theme VARCHAR(50),
    notifications_enabled BOOLEAN DEFAULT TRUE,
    language VARCHAR(50) DEFAULT 'en'
);

-- Friends table (self-referencing many-to-many relationship)
CREATE TABLE friends (
    user_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    friend_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, friend_id)
);

-- Followers table (self-referencing many-to-many relationship)
CREATE TABLE followers (
    user_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    follower_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, follower_id)
);

-- Playlist --
CREATE TABLE lyrics (
    id SERIAL PRIMARY KEY,
    audio_id INTEGER REFERENCES audios (id) ON DELETE CASCADE,
    timestamp FLOAT,
    lyric TEXT
);

-- Comments --
CREATE TABLE audio_comments (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    audio_id INTEGER REFERENCES audios (id) ON DELETE CASCADE,
    comment_text TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Ratings --
CREATE TABLE audio_ratings (
    user_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    audio_id INTEGER REFERENCES audios (id) ON DELETE CASCADE,
    rating FLOAT CHECK (
        rating >= 0
        AND rating <= 5
    ),
    rated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, audio_id)
);

-- Play history table
CREATE TABLE play_history (
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users (id) ON DELETE CASCADE,
    audio_id INTEGER REFERENCES audios (id) ON DELETE CASCADE,
    played_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

DROP FUNCTION IF EXISTS create_liked_playlist;

-- ### Procedures region ###
-- Function to create a "Liked playlist" for each new user
CREATE OR REPLACE FUNCTION create_liked_playlist()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO playlists (user_id, name, created_at, cover_image_url, deletable)
    VALUES (NEW.id, 'Liked Playlist', CURRENT_TIMESTAMP, 'https://firebasestorage.googleapis.com/v0/b/onelastsong-5d5a8.appspot.com/o/images%2FLikedPlaylist.png?alt=media&token=f72e241d-a82d-40a5-86b3-df6f81f6dd40', false);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE OR REPLACE FUNCTION generate_token()
RETURNS UUID AS $$
BEGIN
    RETURN uuid_generate_v4();
END;
$$ LANGUAGE plpgsql;

DROP FUNCTION IF EXISTS public.get_encrypted_password;

-- Create the get_encrypted_password function
CREATE OR REPLACE FUNCTION get_encrypted_password(ip_password VARCHAR)
RETURNS VARCHAR
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
    RETURN crypt(ip_password, gen_salt('bf'));
END;

-- Create a function to generate a UUID token
CREATE OR REPLACE FUNCTION generate_token()
RETURNS UUID AS $$
BEGIN
    RETURN uuid_generate_v4();

END;

$$ LANGUAGE plpgsql;

DROP FUNCTION IF EXISTS public.user_login;

-- Create the login function
CREATE OR REPLACE FUNCTION user_login(ip_username VARCHAR, ip_password VARCHAR)
RETURNS VARCHAR
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_session_token UUID;
    v_expires_at TIMESTAMP;
BEGIN
    -- Verify the user's credentials
    SELECT id INTO v_user_id
    FROM users
    WHERE users.username = ip_username AND users.password_hash = crypt(ip_password, users.password_hash);

    -- If the user is found, create or update the session
    IF v_user_id IS NOT NULL THEN
        v_expires_at := CURRENT_TIMESTAMP + INTERVAL '30 days';

        -- Check if a session already exists
        SELECT session_token INTO v_session_token
        FROM sessions
        WHERE user_id = v_user_id;

        IF v_session_token IS NOT NULL THEN
            -- Update the existing session
            UPDATE sessions
            SET created_at = CURRENT_TIMESTAMP, expires_at = v_expires_at
            WHERE user_id = v_user_id;
        ELSE
            -- Insert a new session
            v_session_token := uuid_generate_v4();
            INSERT INTO sessions (user_id, session_token, created_at, expires_at)
            VALUES (v_user_id, v_session_token, CURRENT_TIMESTAMP, v_expires_at);
        END IF;

        -- Return the session token
        RETURN v_session_token::VARCHAR;
    ELSE
        -- Return empty string if user not found --
        RETURN '';
    END IF;
END;
$$ LANGUAGE plpgsql;

DROP FUNCTION IF EXISTS public.validate_session;

-- Create the session validation function
CREATE OR REPLACE FUNCTION validate_session(session_token VARCHAR)
RETURNS INTEGER 
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
BEGIN
    -- Check if the session token is valid and not expired
    SELECT user_id INTO v_user_id
    FROM sessions
    WHERE sessions.session_token = validate_session.session_token AND expires_at > CURRENT_TIMESTAMP;

    -- If a valid session is found, return USER ID
    IF v_user_id IS NOT NULL THEN
        RETURN v_user_id;
    ELSE
        -- If no valid session is found, return NULL
        RETURN NULL;
    END IF;
END;
$$ LANGUAGE plpgsql;

$$ LANGUAGE plpgsql;

DROP FUNCTION IF EXISTS public.get_user_data;

-- Create a function to retrieve user's data using the session token
CREATE OR REPLACE FUNCTION get_user_data(session_token VARCHAR)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    uid INTEGER;
    user_data JSONB;
BEGIN
    -- Validate the session token and get the user ID
    uid := validate_session(session_token);

    -- If the session is valid, retrieve the user's data
    IF uid IS NOT NULL THEN
        SELECT jsonb_build_object(
            'Id', u.id,
            'Username', u.username,
            'CreatedAt', u.created_at,
            'AvatarUrl', u.avatar_url,
            'ProfileQuote', u.profile_quote,
            'Description', u.description,
            'IsArtist', u.is_artist
        ) INTO user_data
        FROM users u
        WHERE u.id = uid;

        RETURN user_data;
    ELSE
        -- If the session is not valid, return an empty JSON object
        RETURN '{}'::JSONB;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Create a composite type for ResultMessage
CREATE TYPE result_message AS (
    status INT,
    error_message VARCHAR,
    json_data VARCHAR
);

CREATE OR REPLACE FUNCTION get_result_message(status INT, error_message VARCHAR, json_data JSONB)
RETURNS JSONB 
SECURITY DEFINER
AS $$
BEGIN
    RETURN jsonb_build_object(
        'Status', status,
        'ErrorMessage', error_message,
        'JsonData', json_data::TEXT
    );
END;
$$ LANGUAGE plpgsql

-- Create validate username function return empty string if username is valid, otherwise return an error message

CREATE OR REPLACE FUNCTION validate_username(ip_username VARCHAR)
RETURNS VARCHAR
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_error_message VARCHAR;
BEGIN
    -- Check if the username already exists
    SELECT id INTO v_user_id
    FROM users
    WHERE users.username = ip_username;

    -- If the username exists, return an error message
    IF v_user_id IS NOT NULL THEN
        v_error_message := 'Username already exists.';
        RETURN v_error_message;
    ELSE
        -- If the username is valid, return an empty string
        RETURN '';
    END IF;
END;

-- Create validate password function return empty string if password is valid, otherwise return an error message
CREATE OR REPLACE FUNCTION validate_password(ip_password VARCHAR)
RETURNS VARCHAR
SECURITY DEFINER
AS $$
DECLARE
    v_error_message VARCHAR;
BEGIN
    -- Check if the password is at least 8 characters long
    IF LENGTH(ip_password) < 8 THEN
        v_error_message := 'Password must be at least 8 characters long.';
        RETURN v_error_message;
    ELSE
        -- If the password is valid, return an empty string
        RETURN '';
    END IF;
END;

DROP FUNCTION IF EXISTS public.get_encrypted_password;

-- Create the get_encrypted_password function
CREATE OR REPLACE FUNCTION get_encrypted_password(ip_password VARCHAR)
RETURNS VARCHAR
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
BEGIN
    RETURN crypt(ip_password, gen_salt('bf'));
END;
$$;

-- Create the signup function, returns a message as json object with status 0 if successful, otherwise an error message and status 1
CREATE OR REPLACE FUNCTION user_signup(ip_username VARCHAR, ip_password VARCHAR)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_error_message VARCHAR;
    v_status INT;
    v_user_id INTEGER;
BEGIN
    -- Validate the username
    v_error_message := validate_username(ip_username);
    IF v_error_message <> '' THEN
        RETURN get_result_message(1, v_error_message, '{}'::JSONB);
    END IF;

    -- Validate the password
    v_error_message := validate_password(ip_password);
    IF v_error_message <> '' THEN
        RETURN get_result_message(1, v_error_message, '{}'::JSONB);
    END IF;

    -- Insert the new user
    INSERT INTO users (username, password_hash, avatar_url, profile_quote, description)
    VALUES (ip_username, get_encrypted_password(ip_password),'https://firebasestorage.googleapis.com/v0/b/onelastsong-5d5a8.appspot.com/o/images%2FUser.png?alt=media&token=ebf3514f-17e3-4360-a2fe-fd9a60cb1802', '', '')
    RETURNING id INTO v_user_id;

    -- Return success message
    RETURN get_result_message(0, '', '{}'::JSONB);
END;
$$;

-- Create a function to get the top n most liked audios
DROP FUNCTION IF EXISTS get_most_like_audios (n INT);

CREATE OR REPLACE FUNCTION get_most_like_audios(n INT)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- Use a CTE to calculate the number of likes for each audio
    WITH audio_likes AS (
        SELECT 
            a.id,
            a.title,
            a.artist,
            a.album_id,
            a.category_id,
            a.duration,
            a.url,
            a.cover_image_url,
            a.author_id,
            a.created_at,
            a.description,
            c.name AS country,
            cat.name AS category_name,
            ARRAY_AGG(g.name) AS genres,
            COUNT(l.audio_id) AS likes
        FROM audios a
        LEFT JOIN likes l ON a.id = l.audio_id
        LEFT JOIN countries c ON a.country_id = c.id
        LEFT JOIN categories cat ON a.category_id = cat.id
        LEFT JOIN audios_genres ag ON a.id = ag.audio_id
        LEFT JOIN genres g ON ag.genre_id = g.id
        GROUP BY a.id, c.name, cat.name
        ORDER BY COUNT(l.audio_id) DESC
        LIMIT n
    )
    -- Select the top n audios sorted by the number of likes
    SELECT json_agg(json_build_object(
        'AudioId', al.id,
        'Title', al.title,
        'Artist', al.artist,
        'AlbumId', al.album_id,
        'CategoryId', al.category_id,
        'Duration', al.duration,
        'Url', al.url,
        'CoverImageUrl', al.cover_image_url,
        'AuthorId', al.author_id,
        'CreatedAt', al.created_at,
        'Description', al.description,
        'Country', al.country,
        'CategoryName', al.category_name,
        'Genres', al.genres,
        'Likes', al.likes
    )) INTO v_json_data
    FROM audio_likes al;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);

END;
$$;

CREATE OR REPLACE FUNCTION get_album_item_count(album_id INTEGER)
RETURNS INTEGER AS $$
DECLARE
    count INTEGER;
BEGIN
    SELECT COUNT(*) INTO count
    FROM audios
    WHERE audios.album_id = album_id;
    RETURN count;
END;
$$ LANGUAGE plpgsql;

-- Function to get first n albums
-- Create a function to get the first n albums
CREATE OR REPLACE FUNCTION get_first_n_albums(n INT)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- Select the first n albums
    SELECT json_agg(json_build_object(
        'AlbumId', a.id,
        'Title', a.title,
        'Artist', a.artist,
        'CoverImageUrl', a.cover_image_url,
        'ReleaseDate', a.release_date,
        'CreatedAt', a.created_at,
        'UserId', a.user_id,
        'ItemCount', (
            SELECT COUNT(*)
            FROM audios au
            WHERE au.album_id = a.id
        )
    )) INTO v_json_data
    FROM (
        SELECT *
        FROM albums a
        ORDER BY a.created_at DESC
        LIMIT n
    ) a;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

-- Function get audio by id
DROP FUNCTION IF EXISTS get_audio_by_id;

CREATE OR REPLACE FUNCTION get_audio_by_id(ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- If not existed, return an error message
    IF NOT EXISTS (
        SELECT 1
        FROM audios a
        WHERE a.id = ip_audio_id
    ) THEN
        RETURN get_result_message(1, 'Audio not found', '[]'::JSONB);
    END IF;

    -- Select the audio by id
    SELECT Top 1 json_build_object(
        'AudioId', a.id,
        'Title', a.title,
        'Artist', a.artist,
        'AlbumId', a.album_id,
        'CategoryId', a.category_id,
        'Duration', a.duration,
        'Url', a.url,
        'CoverImageUrl', a.cover_image_url,
        'AuthorId', a.author_id,
        'CreatedAt', a.created_at,
        'Description', a.description,
        'CountryId', a.country_id,
        'Likes', (
            SELECT COUNT(*)
            FROM likes l
            WHERE l.audio_id = a.id
        )
    ) INTO v_json_data
    FROM audios a
    WHERE a.id = ip_audio_id;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

-- Function get all user's playlists
DROP FUNCTION IF EXISTS get_all_user_playlists (VARCHAR);

CREATE OR REPLACE FUNCTION get_all_user_playlists(ip_session_token VARCHAR)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INT;
    v_json_data JSONB;
BEGIN
    -- Get the user id
    v_user_id := validate_session(ip_session_token);

    IF v_user_id IS NULL THEN
        RETURN get_result_message(1, 'Invalid session token', '[]'::JSONB);
    END IF;

    -- Select all user's playlists
    SELECT json_agg(json_build_object(
        'PlaylistId', p.id,
        'Name', p.name,
        'CoverImageUrl', p.cover_image_url,
        'ItemCount', (
            SELECT COUNT(*)
            FROM playlist_audios pa
            WHERE pa.playlist_id = p.id
        ),
        'Audios', COALESCE((
            SELECT json_agg(json_build_object(
                'AudioId', a.id,
                'Title', a.title,
                'Artist', a.artist,
                'AlbumId', a.album_id,
                'CategoryId', a.category_id,
                'Duration', a.duration,
                'Url', a.url,
                'CoverImageUrl', a.cover_image_url,
                'AuthorId', a.author_id,
                'CreatedAt', a.created_at,
                'Description', a.description,
                'Likes', (
                    SELECT COUNT(*)
                    FROM likes l
                    WHERE l.audio_id = a.id
                )
            ) ORDER BY pa.added_at)
            FROM audios a
            JOIN playlist_audios pa ON pa.audio_id = a.id
            WHERE pa.playlist_id = p.id
        ), '[]'::json),
        'CreatedAt', p.created_at
    )) INTO v_json_data
    FROM playlists p
    WHERE p.user_id = v_user_id;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

CREATE OR REPLACE FUNCTION get_audio_by_id(ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- If not existed, return an error message
    IF NOT EXISTS (
        SELECT 1
        FROM audios a
        WHERE a.id = ip_audio_id
    ) THEN
        RETURN get_result_message(1, 'Audio not found', '[]'::JSONB);
    END IF;

    -- Select the audio by id
    SELECT json_build_object(
        'AudioId', a.id,
        'Title', a.title,
        'Artist', a.artist,
        'AlbumId', a.album_id,
        'CategoryId', a.category_id,
        'Duration', a.duration,
        'Url', a.url,
        'CoverImageUrl', a.cover_image_url,
        'AuthorId', a.author_id,
        'CreatedAt', a.created_at,
        'Description', a.description,
        'Country', c.name,
        'CategoryName', cat.name,
        'Genres', ARRAY_AGG(g.name),
        'Likes', (
            SELECT COUNT(*)
            FROM likes l
            WHERE l.audio_id = a.id
        )
    ) INTO v_json_data
    FROM audios a
    LEFT JOIN countries c ON a.country_id = c.id
    LEFT JOIN categories cat ON a.category_id = cat.id
    LEFT JOIN audios_genres ag ON a.id = ag.audio_id
    LEFT JOIN genres g ON ag.genre_id = g.id
    WHERE a.id = ip_audio_id
    GROUP BY a.id, c.name, cat.name;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

-- Add a new playlist for a user with a name and a cover image URL
DROP FUNCTION IF EXISTS add_user_playlist;

CREATE OR REPLACE FUNCTION add_user_playlist(ip_session_token VARCHAR, ip_name VARCHAR, ip_cover_image_url VARCHAR)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_playlist_id INTEGER;
    v_playlist JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, insert the new playlist
    IF v_user_id IS NOT NULL THEN
        -- Check if playlist is valid: <> '', not NULL, and <> 'Liked Playlist'
        IF ip_name = '' OR ip_name IS NULL OR ip_name = 'Liked Playlist' THEN
            RETURN get_result_message(1, 'Invalid playlist name', '{}'::JSONB);
        END IF;

        INSERT INTO playlists (user_id, name, cover_image_url, created_at)
        VALUES (v_user_id, ip_name, ip_cover_image_url, CURRENT_TIMESTAMP)
        RETURNING id INTO v_playlist_id;

        -- Retrieve the newly-added playlist details
        SELECT json_build_object(
            'PlaylistId', p.id,
            'Name', p.name,
            'CoverImageUrl', p.cover_image_url,
            'ItemCount', (
                SELECT COUNT(*)
                FROM playlist_audios pa
                WHERE pa.playlist_id = p.id
            ),
            'CreatedAt', p.created_at
        ) INTO v_playlist
        FROM playlists p
        WHERE p.id = v_playlist_id;

        -- Return the new playlist details
        RETURN get_result_message(0, '', v_playlist);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Get all artists
DROP FUNCTION IF EXISTS get_all_artists ();

CREATE OR REPLACE FUNCTION get_all_artists()
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- Select from users where u.is_artist = true
    SELECT json_agg(json_build_object(
        'Id', u.id,
        'Username', u.username,
        'CreatedAt', u.created_at,
        'AvatarUrl', u.avatar_url,
        'ProfileQuote', u.profile_quote,
        'Description', u.description,
        'IsArtist', u.is_artist
    )) INTO v_json_data
    FROM users u
    WHERE u.is_artist = true;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

-- Add an audio to a playlist --
DROP FUNCTION IF EXISTS add_audio_to_playlist;

CREATE OR REPLACE FUNCTION add_audio_to_playlist(ip_session_token VARCHAR, ip_playlist_id INTEGER, ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_playlist_owner_id INTEGER;
    v_audio_exists BOOLEAN;
    v_playlist_exists BOOLEAN;
    v_audio_in_playlist BOOLEAN;
    v_result JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, proceed with adding the audio to the playlist
    IF v_user_id IS NOT NULL THEN
        -- Check if the audio exists
        SELECT EXISTS (
            SELECT 1
            FROM audios a
            WHERE a.id = ip_audio_id
        ) INTO v_audio_exists;

        IF NOT v_audio_exists THEN
            RETURN get_result_message(1, 'Audio does not exist', '{}'::JSONB);
        END IF;

        -- Check if the playlist exists
        SELECT EXISTS (
            SELECT 1
            FROM playlists p
            WHERE p.id = ip_playlist_id
        ) INTO v_playlist_exists;

        IF NOT v_playlist_exists THEN
            RETURN get_result_message(1, 'Playlist does not exist', '{}'::JSONB);
        END IF;

        -- Check if the playlist belongs to the user
        SELECT user_id INTO v_playlist_owner_id
        FROM playlists
        WHERE id = ip_playlist_id;

        IF v_playlist_owner_id <> v_user_id THEN
            RETURN get_result_message(1, 'Playlist does not belong to the user', '{}'::JSONB);
        END IF;

        -- Check if the audio is already in the playlist
        SELECT EXISTS (
            SELECT 1
            FROM playlist_audios pa
            WHERE pa.playlist_id = ip_playlist_id AND pa.audio_id = ip_audio_id
        ) INTO v_audio_in_playlist;

        IF v_audio_in_playlist THEN
            RETURN get_result_message(1, 'Audio is already in the playlist', '{}'::JSONB);
        END IF;

        -- If the audio is the first one in the playlist, update the playlist cover image to the audio's cover image
        IF NOT EXISTS (
            SELECT 1
            FROM playlist_audios
            WHERE playlist_id = ip_playlist_id AND audio_id <> ip_audio_id
        ) THEN
            UPDATE playlists
            SET cover_image_url = (
                SELECT cover_image_url
                FROM audios
                WHERE id = ip_audio_id
            )
            WHERE id = ip_playlist_id;
        END IF;

        -- Add the audio to the playlist
        INSERT INTO playlist_audios (playlist_id, audio_id)
        VALUES (ip_playlist_id, ip_audio_id);

        -- Return success message
        RETURN get_result_message(0, '', '{}'::JSONB);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Remove an audio from a playlist --
DROP FUNCTION IF EXISTS remove_audio_from_playlist;

CREATE OR REPLACE FUNCTION remove_audio_from_playlist(ip_session_token VARCHAR, ip_playlist_id INTEGER, ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_playlist_owner_id INTEGER;
    v_audio_exists BOOLEAN;
    v_playlist_exists BOOLEAN;
    v_audio_in_playlist BOOLEAN;
    v_result JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, proceed with removing the audio from the playlist
    IF v_user_id IS NOT NULL THEN
        -- Check if the audio exists
        SELECT EXISTS (
            SELECT 1
            FROM audios a
            WHERE a.id = ip_audio_id
        ) INTO v_audio_exists;

        IF NOT v_audio_exists THEN
            RETURN get_result_message(1, 'Audio does not exist', '{}'::JSONB);
        END IF;

        -- Check if the playlist exists
        SELECT EXISTS (
            SELECT 1
            FROM playlists p
            WHERE p.id = ip_playlist_id
        ) INTO v_playlist_exists;

        IF NOT v_playlist_exists THEN
            RETURN get_result_message(1, 'Playlist does not exist', '{}'::JSONB);
        END IF;

        -- Check if the playlist belongs to the user
        SELECT user_id INTO v_playlist_owner_id
        FROM playlists
        WHERE id = ip_playlist_id;

        IF v_playlist_owner_id <> v_user_id THEN
            RETURN get_result_message(1, 'Playlist does not belong to the user', '{}'::JSONB);
        END IF;

        -- Check if the audio is in the playlist
        SELECT EXISTS (
            SELECT 1
            FROM playlist_audios pa
            WHERE pa.playlist_id = ip_playlist_id AND pa.audio_id = ip_audio_id
        ) INTO v_audio_in_playlist;

        IF NOT v_audio_in_playlist THEN
            RETURN get_result_message(1, 'Audio is not in the playlist', '{}'::JSONB);
        END IF;

        -- Remove the audio from the playlist
        DELETE FROM playlist_audios
        WHERE playlist_id = ip_playlist_id AND audio_id = ip_audio_id;

        -- Return success message
        RETURN get_result_message(0, '', '{}'::JSONB);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Delete a playlist --
DROP FUNCTION IF EXISTS delete_playlist;

CREATE OR REPLACE FUNCTION delete_playlist(ip_session_token VARCHAR, ip_playlist_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_playlist_owner_id INTEGER;
    v_playlist_exists BOOLEAN;
    v_result JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, proceed with deleting the playlist
    IF v_user_id IS NOT NULL THEN
        -- Check if the playlist exists
        SELECT EXISTS (
            SELECT 1
            FROM playlists p
            WHERE p.id = ip_playlist_id
        ) INTO v_playlist_exists;

        IF NOT v_playlist_exists THEN
            RETURN get_result_message(1, 'Playlist does not exist', '{}'::JSONB);
        END IF;

        -- Check if the playlist name is not "Liked Playlist"
        IF EXISTS (
            SELECT 1
            FROM playlists p
            WHERE p.id = ip_playlist_id AND p.name = 'Liked Playlist'
        ) THEN
            RETURN get_result_message(1, 'Cannot delete the "Liked Playlist"', '{}'::JSONB);
        END IF;

        -- Check if the playlist belongs to the user
        SELECT user_id INTO v_playlist_owner_id
        FROM playlists
        WHERE id = ip_playlist_id;

        IF v_playlist_owner_id <> v_user_id THEN
            RETURN get_result_message(1, 'Playlist does not belong to the user', '{}'::JSONB);
        END IF;

        -- Delete the playlist
        DELETE FROM playlists
        WHERE id = ip_playlist_id;

        -- Return success message
        RETURN get_result_message(0, '', '{}'::JSONB);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Save listening session --
DROP FUNCTION IF EXISTS save_listening_session;

CREATE OR REPLACE FUNCTION save_listening_session(ip_session_token VARCHAR, ip_audio_id INTEGER, ip_progress INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_audio_exists BOOLEAN;
    v_result JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, proceed with saving the listening session
    IF v_user_id IS NOT NULL THEN
        -- Check if the audio exists
        SELECT EXISTS (
            SELECT 1
            FROM audios a
            WHERE a.id = ip_audio_id
        ) INTO v_audio_exists;

        IF NOT v_audio_exists THEN
            RETURN get_result_message(1, 'Audio does not exist', '{}'::JSONB);
        END IF;

        -- if the listening session of the user not exists, insert a new record, else update the current record
        IF NOT EXISTS (
            SELECT 1
            FROM listening_sessions ls
            WHERE ls.user_id = v_user_id
        ) THEN
            INSERT INTO listening_sessions (user_id, audio_id, progress)
            VALUES (v_user_id, ip_audio_id, ip_progress);
        ELSE
            UPDATE listening_sessions
            SET progress = ip_progress, audio_id = ip_audio_id
            WHERE user_id = v_user_id;
        END IF;

        -- Return success message
        RETURN get_result_message(0, '', '{}'::JSONB);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Get listening session, return the corresponding listening session of the user
DROP FUNCTION IF EXISTS get_listening_session;

CREATE OR REPLACE FUNCTION get_listening_session(ip_session_token VARCHAR)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_listening_session JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, return the listening session
    IF v_user_id IS NOT NULL THEN
        SELECT json_build_object(
            'UserId', ls.user_id,
            'AudioId', ls.audio_id,
            'Progress', ls.progress
        ) INTO v_listening_session
        FROM listening_sessions ls
        WHERE ls.user_id = v_user_id;

        RETURN get_result_message(0, '', v_listening_session);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Get lyrics by audio id
DROP FUNCTION IF EXISTS get_lyrics;

CREATE OR REPLACE FUNCTION get_lyrics(ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- Check if the audio exists
    IF NOT EXISTS (
        SELECT 1
        FROM audios a
        WHERE a.id = ip_audio_id
    ) THEN
        -- if not exists return an error message
        RETURN get_result_message(1, 'Audio not found', '[]'::JSONB);
    END IF;

    -- Select the lyrics for the given audio id
    SELECT json_agg(json_build_object(
        'Id', l.id,
        'AudioId', l.audio_id,
        'Timestamp', l.timestamp,
        'LyricText', l.lyric
    ) ORDER BY l.timestamp) INTO v_json_data
    FROM lyrics l
    WHERE l.audio_id = ip_audio_id;

    -- If no lyrics are found, return an empty JSON array
    IF v_json_data IS NULL THEN
        v_json_data := '[]'::JSONB;
    END IF;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

-- Comment an audio
DROP FUNCTION IF EXISTS comment_audio;

CREATE OR REPLACE FUNCTION comment_audio(ip_session_token VARCHAR, ip_audio_id INTEGER, ip_comment TEXT)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_audio_exists BOOLEAN;
    v_result JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, proceed with saving the listening session
    IF v_user_id IS NOT NULL THEN
        -- Check if the audio exists
        SELECT EXISTS (
            SELECT 1
            FROM audios a
            WHERE a.id = ip_audio_id
        ) INTO v_audio_exists;

        IF NOT v_audio_exists THEN
            RETURN get_result_message(1, 'Audio does not exist', '{}'::JSONB);
        END IF;

        -- Insert the comment
        INSERT INTO audio_comments (user_id, audio_id, comment_text, created_at)
        VALUES (v_user_id, ip_audio_id, ip_comment, CURRENT_TIMESTAMP);

        -- Return success message
        RETURN get_result_message(0, '', '{}'::JSONB);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Get comments by audio id
DROP FUNCTION IF EXISTS get_comments_by_audio_id;

CREATE OR REPLACE FUNCTION get_comments_by_audio_id(ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- Check if the audio exists
    IF NOT EXISTS (
        SELECT 1
        FROM audios a
        WHERE a.id = ip_audio_id
    ) THEN
        -- if not exists return an error message
        RETURN get_result_message(1, 'Audio not found', '[]'::JSONB);
    END IF;

    -- Select the comments for the given audio id
    SELECT json_agg(json_build_object(
        'Id', c.id,
        'UserId', c.user_id,
        'AudioId', c.audio_id,
        'CommentText', c.comment_text,
        'CreatedAt', c.created_at
    ) ORDER BY c.created_at DESC) INTO v_json_data
    FROM audio_comments c
    WHERE c.audio_id = ip_audio_id;

    -- If no comments are found, return an empty JSON array
    IF v_json_data IS NULL THEN
        v_json_data := '[]'::JSONB;
    END IF;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

-- Rate an audio
DROP FUNCTION IF EXISTS rate_audio;

CREATE OR REPLACE FUNCTION rate_audio(ip_session_token VARCHAR, ip_audio_id INTEGER, ip_rating FLOAT)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_audio_exists BOOLEAN;
    v_rating_exists BOOLEAN;
    v_result JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, proceed with saving the listening session
    IF v_user_id IS NOT NULL THEN
        -- Check if the audio exists
        SELECT EXISTS (
            SELECT 1
            FROM audios a
            WHERE a.id = ip_audio_id
        ) INTO v_audio_exists;

        IF NOT v_audio_exists THEN
            RETURN get_result_message(1, 'Audio does not exist', '{}'::JSONB);
        END IF;

        -- Insert or update the rating
        SELECT EXISTS (
            SELECT 1
            FROM audio_ratings ar
            WHERE ar.user_id = v_user_id AND ar.audio_id = ip_audio_id
        ) INTO v_rating_exists;

        IF v_rating_exists THEN
            UPDATE audio_ratings
            SET rating = ip_rating
            WHERE user_id = v_user_id AND audio_id = ip_audio_id;
        ELSE
            INSERT INTO audio_ratings (user_id, audio_id, rating, rated_at)
            VALUES (v_user_id, ip_audio_id, ip_rating, CURRENT_TIMESTAMP);
        END IF;

        -- Return success message
        RETURN get_result_message(0, '', '{}'::JSONB);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Get rating score by audio id, return an object contains the average rating score and the number of ratings
DROP FUNCTION IF EXISTS get_rating_score_by_audio_id;

CREATE OR REPLACE FUNCTION get_rating_score_by_audio_id(ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- Check if the audio exists
    IF NOT EXISTS (
        SELECT 1
        FROM audios a
        WHERE a.id = ip_audio_id
    ) THEN
        -- if not exists return an error message
        RETURN get_result_message(1, 'Audio not found', '[]'::JSONB);
    END IF;

    -- Select the rating score for the given audio id
    SELECT json_build_object(
        'AverageRating', AVG(r.rating)::FLOAT,
        'RatingCount', COUNT(r.rating)
    ) INTO v_json_data
    FROM audio_ratings r
    WHERE r.audio_id = ip_audio_id;

    -- If no ratings are found, return an empty JSON object
    IF v_json_data IS NULL THEN
        v_json_data := '{}'::JSONB;
    END IF;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

-- Get user display info --
DROP FUNCTION IF EXISTS get_user_display_info;

CREATE OR REPLACE FUNCTION get_user_display_info(ip_user_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- Select basic info of the user with the given userId
    SELECT json_build_object(
        'UserId', u.id,
        'DisplayName', u.username,
        'AvatarUrl', u.avatar_url
    ) INTO v_json_data
    FROM users u
    WHERE u.id = ip_user_id;

    -- If no user is found, return an error message
    IF v_json_data IS NULL THEN
        RETURN get_result_message(1, 'User not found', '{}'::JSONB);
    END IF;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

-- Get user audio rating --
DROP FUNCTION IF EXISTS get_user_audio_rating;

CREATE OR REPLACE FUNCTION get_user_audio_rating(ip_user_id INTEGER, ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_json_data JSONB;
BEGIN
    -- Check if the rating exists
    IF NOT EXISTS (
        SELECT 1
        FROM audio_ratings ar
        WHERE ar.user_id = ip_user_id AND ar.audio_id = ip_audio_id
    ) THEN
        -- If not exists, return an error message
        RETURN get_result_message(1, 'Rating not found', '{}'::JSONB);
    END IF;

    -- Select the rating for the given user and audio id
    SELECT json_build_object(
        'UserId', ar.user_id,
        'AudioId', ar.audio_id,
        'Rating', ar.rating,
        'RatedAt', ar.rated_at
    ) INTO v_json_data
    FROM audio_ratings ar
    WHERE ar.user_id = ip_user_id AND ar.audio_id = ip_audio_id;

    -- Return the result message
    RETURN get_result_message(0, '', v_json_data);
END;
$$;

-- Add user play history, returns the new record in the play history
DROP FUNCTION IF EXISTS add_user_play_history;

CREATE OR REPLACE FUNCTION add_user_play_history(ip_session_token VARCHAR, ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_play_history JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, proceed with adding the play history
    IF v_user_id IS NOT NULL THEN
        -- Check if the audio exists
        IF NOT EXISTS (
            SELECT 1
            FROM audios a
            WHERE a.id = ip_audio_id
        ) THEN
            RETURN get_result_message(1, 'Audio does not exist', '{}'::JSONB);
        END IF;

        -- Insert the play history record
        INSERT INTO play_history (user_id, audio_id, played_at)
        VALUES (v_user_id, ip_audio_id, CURRENT_TIMESTAMP)
        RETURNING json_build_object(
            'AudioId', audio_id,
            'PlayedAt', played_at
        ) INTO v_play_history;

        -- Return the new play history record
        RETURN get_result_message(0, '', v_play_history);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Get user play history, returns the play history of the user, return emply array if no play history
DROP FUNCTION IF EXISTS get_user_play_history;

CREATE OR REPLACE FUNCTION get_user_play_history(ip_session_token VARCHAR)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_play_history JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, return the play history of the user
    IF v_user_id IS NOT NULL THEN
        -- Select the play history of the user
        SELECT json_agg(json_build_object(
            'AudioId', ph.audio_id,
            'PlayedAt', ph.played_at
        ) ORDER BY ph.played_at DESC) INTO v_play_history
        FROM play_history ph
        WHERE ph.user_id = v_user_id;

        -- If no play history is found, return an empty JSON array
        IF v_play_history IS NULL THEN
            v_play_history := '[]'::JSONB;
        END IF;

        -- Return the result message
        RETURN get_result_message(0, '', v_play_history);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Function to like an audio, add to likes table and liked playlist
CREATE OR REPLACE FUNCTION like_audio(ip_session_token VARCHAR, ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_audio_exists BOOLEAN;
    v_result JSONB;
    v_liked_playlist_id INTEGER;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, proceed with liking the audio
    IF v_user_id IS NOT NULL THEN
        -- Check if the audio exists
        SELECT EXISTS (
            SELECT 1
            FROM audios a
            WHERE a.id = ip_audio_id
        ) INTO v_audio_exists;

        IF NOT v_audio_exists THEN
            RETURN get_result_message(1, 'Audio does not exist', '{}'::JSONB);
        END IF;

        -- Check if the audio is already liked by the user
        IF EXISTS (
            SELECT 1
            FROM likes l
            WHERE l.user_id = v_user_id AND l.audio_id = ip_audio_id
        ) THEN
            RETURN get_result_message(1, 'Audio is already liked', '{}'::JSONB);
        END IF;

        -- Insert the like record
        INSERT INTO likes (user_id, audio_id, liked_at)
        VALUES (v_user_id, ip_audio_id, CURRENT_TIMESTAMP);

        -- Add the audio to the liked playlist
        SELECT id INTO v_liked_playlist_id
        FROM playlists
        WHERE user_id = v_user_id AND name = 'Liked Playlist';

        IF v_liked_playlist_id IS NULL THEN
            INSERT INTO playlists (user_id, name, cover_image_url, created_at)
            VALUES (v_user_id, 'Liked Playlist', '', CURRENT_TIMESTAMP)
            RETURNING id INTO v_liked_playlist_id;
        END IF;

        PERFORM add_audio_to_playlist(ip_session_token, v_liked_playlist_id, ip_audio_id);

        -- Return success message
        RETURN get_result_message(0, '', '{}'::JSONB);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- Remove like from an audio, remove from likes table and liked playlist
CREATE OR REPLACE FUNCTION remove_like_from_audio(ip_session_token VARCHAR, ip_audio_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_audio_exists BOOLEAN;
    v_result JSONB;
    v_liked_playlist_id INTEGER;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, proceed with removing the like from the audio
    IF v_user_id IS NOT NULL THEN
        -- Check if the audio exists
        SELECT EXISTS (
            SELECT 1
            FROM audios a
            WHERE a.id = ip_audio_id
        ) INTO v_audio_exists;

        IF NOT v_audio_exists THEN
            RETURN get_result_message(1, 'Audio does not exist', '{}'::JSONB);
        END IF;

        -- Check if the audio is liked by the user
        IF NOT EXISTS (
            SELECT 1
            FROM likes l
            WHERE l.user_id = v_user_id AND l.audio_id = ip_audio_id
        ) THEN
            RETURN get_result_message(1, 'Audio is not liked', '{}'::JSONB);
        END IF;

        -- Remove the like record
        DELETE FROM likes
        WHERE user_id = v_user_id AND audio_id = ip_audio_id;

        -- Remove the audio from the liked playlist
        SELECT id INTO v_liked_playlist_id
        FROM playlists
        WHERE user_id = v_user_id AND name = 'Liked Playlist';

        IF v_liked_playlist_id IS NOT NULL THEN
            PERFORM remove_audio_from_playlist(ip_session_token, v_liked_playlist_id, ip_audio_id);
        END IF;

        -- Return success message
        RETURN get_result_message(0, '', '{}'::JSONB);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- create function to update user profile
CREATE OR REPLACE FUNCTION update_user_profile(
    ip_session_token VARCHAR,
    ip_description TEXT,
    ip_avatar_url VARCHAR
)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is valid, update the user profile
    IF v_user_id IS NOT NULL THEN
        UPDATE users
        SET avatar_url = ip_avatar_url,
            description = ip_description
        WHERE id = v_user_id;

        -- Return the success message
        RETURN get_result_message(0, '', '{}'::JSONB);
    ELSE
        -- If the session is not valid, return an error message
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;
END;
$$;

-- get audios in a playlist, input token and playlist id
CREATE OR REPLACE FUNCTION get_audios_in_playlist(ip_session_token VARCHAR, ip_playlist_id INTEGER)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
    v_json_data JSONB;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- Check if the playlist belongs to the user
    IF NOT EXISTS (
        SELECT 1
        FROM playlists p
        WHERE p.id = ip_playlist_id AND p.user_id = v_user_id
    ) THEN
        -- If the playlist does not belong to the user, return an error message
        RETURN get_result_message(1, 'Playlist does not belong to the user', '{}'::JSONB);
    END IF;

    -- If the session is valid, retrieve the audios in the playlist
IF v_user_id IS NOT NULL THEN
    WITH audio_genres AS (
        SELECT 
            a.id AS audio_id,
            ARRAY_AGG(g.name) AS genres
        FROM audios a
        LEFT JOIN audios_genres ag ON a.id = ag.audio_id
        LEFT JOIN genres g ON ag.genre_id = g.id
        GROUP BY a.id
    )
    SELECT json_agg(json_build_object(
        'AudioId', a.id,
        'Title', a.title,
        'Artist', a.artist,
        'AlbumId', a.album_id,
        'CategoryId', a.category_id,
        'Duration', a.duration,
        'Url', a.url,
        'CoverImageUrl', a.cover_image_url,
        'AuthorId', a.author_id,
        'CreatedAt', a.created_at,
        'Description', a.description,
        'Country', c.name,
        'CategoryName', cat.name,
        'Genres', ag.genres,
        'Likes', (
            SELECT COUNT(*)
            FROM likes l
            WHERE l.audio_id = a.id
        )
    )) INTO v_json_data
    FROM audios a
    JOIN playlist_audios pa ON a.id = pa.audio_id
    LEFT JOIN countries c ON a.country_id = c.id
    LEFT JOIN categories cat ON a.category_id = cat.id
    LEFT JOIN audio_genres ag ON a.id = ag.audio_id
    WHERE pa.playlist_id = ip_playlist_id
    GROUP BY a.id, c.name, cat.name, ag.genres;

    -- Return the audios in the playlist
    RETURN get_result_message(0, '', v_json_data);
ELSE
    -- If the session is not valid, return an error message
    RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
END IF;
END;
$$;

-- Update user playlist --
CREATE OR REPLACE FUNCTION update_user_playlist(ip_session_token VARCHAR, ip_playlist_id INTEGER, ip_name VARCHAR, ip_cover_image_url VARCHAR)
RETURNS JSONB
LANGUAGE plpgsql
SECURITY DEFINER
AS $$
DECLARE
    v_user_id INTEGER;
BEGIN
    -- Validate the session token and get the user ID
    v_user_id := validate_session(ip_session_token);

    -- If the session is not valid, return an error message
    IF v_user_id IS NULL THEN
        RETURN get_result_message(1, 'Invalid session token', '{}'::JSONB);
    END IF;

    -- If ip_name is empty, NULL or equals "Liked Playlist" return an error message
    IF ip_name IS NULL OR ip_name = '' OR ip_name = 'Liked Playlist' THEN
        RETURN get_result_message(1, 'Invalid playlist name', '{}'::JSONB);
    END IF;

    -- Check if playlist exists
    IF NOT EXISTS (
        SELECT 1
        FROM playlists p
        WHERE p.id = ip_playlist_id
    ) THEN
        RETURN get_result_message(1, 'Playlist does not exist', '{}'::JSONB);
    END IF;

    -- Check if the playlist belongs to the user
    IF NOT EXISTS (
        SELECT 1
        FROM playlists p
        WHERE p.id = ip_playlist_id AND p.user_id = v_user_id
    ) THEN
        -- If the playlist does not belong to the user, return an error message
        RETURN get_result_message(1, 'Playlist does not belong to the user', '{}'::JSONB);
    END IF;

    -- Update the playlist
    UPDATE playlists
    SET name = ip_name,
        cover_image_url = ip_cover_image_url
    WHERE id = ip_playlist_id;

    -- Return the success message
    RETURN get_result_message(0, '', '{}'::JSONB);
END;
$$;

-- ### Triggers region ###
-- Trigger to call the function after a new user is inserted
CREATE TRIGGER after_user_insert
AFTER INSERT ON users
FOR EACH ROW
EXECUTE FUNCTION create_liked_playlist();

-- Security --
-- Create the restricted_user role with login capabilities and a password
CREATE ROLE restricted_user WITH LOGIN PASSWORD '12345678';
-- Revoke all privileges from restricted_user
REVOKE ALL ON ALL TABLES IN SCHEMA public FROM restricted_user;

REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM restricted_user;

REVOKE ALL ON ALL FUNCTIONS IN SCHEMA public FROM restricted_user;
-- Grant execute permissions on specific functions to restricted_user
GRANT EXECUTE ON FUNCTION user_login(VARCHAR, VARCHAR) TO restricted_user;

GRANT
EXECUTE ON FUNCTION validate_session (VARCHAR) TO restricted_user;

GRANT EXECUTE ON FUNCTION get_user_data (VARCHAR) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_result_message (INT, VARCHAR, JSONB) TO restricted_user;

GRANT
EXECUTE ON FUNCTION validate_username (VARCHAR) TO restricted_user;

GRANT
EXECUTE ON FUNCTION validate_password (VARCHAR) TO restricted_user;

GRANT
EXECUTE ON FUNCTION user_signup (VARCHAR, VARCHAR) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_most_like_audios (INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_album_item_count (INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_first_n_albums (INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_all_user_playlists (VARCHAR) TO restricted_user;

GRANT
EXECUTE ON FUNCTION add_user_playlist (VARCHAR, VARCHAR, VARCHAR) TO restricted_user;

GRANT EXECUTE ON FUNCTION get_all_artists () TO restricted_user;

GRANT
EXECUTE ON FUNCTION add_audio_to_playlist (VARCHAR, INT, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION remove_audio_from_playlist (VARCHAR, INT, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION delete_playlist (VARCHAR, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION save_listening_session (VARCHAR, INT, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION save_listening_session (VARCHAR, INT, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_listening_session (VARCHAR, INT) TO restricted_user;

GRANT EXECUTE ON FUNCTION get_lyrics (INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION comment_audio (VARCHAR, INT, TEXT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_comments_by_audio_id (INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION rate_audio (VARCHAR, INT, FLOAT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_rating_score_by_audio_id (INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_user_display_info (INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_user_audio_rating (INT, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION add_user_play_history (VARCHAR, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_user_play_history (VARCHAR) TO restricted_user;

GRANT
EXECUTE ON FUNCTION like_audio (VARCHAR, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION remove_like_from_audio (VARCHAR, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION update_user_profile (VARCHAR, TEXT, VARCHAR) TO restricted_user;

GRANT
EXECUTE ON FUNCTION get_audios_in_playlist (VARCHAR, INT) TO restricted_user;

GRANT
EXECUTE ON FUNCTION update_user_playlist (
    VARCHAR,
    INT,
    VARCHAR,
    VARCHAR
) TO restricted_user;

SELECT user_login ('test', 'test');

SELECT validate_session ( '7d683b5d-6c62-474f-b666-7df5017edabc' );

SELECT get_user_data ( '7d683b5d-6c62-474f-b666-7df5017edabc' );