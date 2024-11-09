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
CREATE TABLE users 
(
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) UNIQUE NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    avatar_url VARCHAR(255),
    profile_quote TEXT,
    description TEXT
);

-- Sessions table
CREATE TABLE sessions
(
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    session_token VARCHAR(255) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP NOT NULL
);

-- Albums table
CREATE TABLE albums
(
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    artist VARCHAR(255) NOT NULL,
    release_date DATE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    user_id INTEGER REFERENCES users(id) ON DELETE SET NULL
);

-- Categories table
CREATE TABLE categories
(
    id SERIAL PRIMARY KEY,
    name VARCHAR(255) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Audios table
CREATE TABLE audios
(
    id SERIAL PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    artist VARCHAR(255) NOT NULL,
    album_id INTEGER REFERENCES albums(id) ON DELETE SET NULL,
    category_id INTEGER REFERENCES categories(id) ON DELETE SET NULL,
    duration INTEGER NOT NULL, -- Duration in seconds
    url VARCHAR(255) NOT NULL,
    author_id INTEGER REFERENCES users(id) ON DELETE SET NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Playlists table
CREATE TABLE playlists
(
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    name VARCHAR(255) NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Playlist audios table (many-to-many relationship between playlists and audios)
CREATE TABLE playlist_audios
(
    playlist_id INTEGER REFERENCES playlists(id) ON DELETE CASCADE,
    audio_id INTEGER REFERENCES audios(id) ON DELETE CASCADE,
    PRIMARY KEY (playlist_id, audio_id)
);

-- Likes table (tracks which audios users have liked)
CREATE TABLE likes
(
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    audio_id INTEGER REFERENCES audios(id) ON DELETE CASCADE,
    liked_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, audio_id)
);

-- Listening sessions table
CREATE TABLE listening_sessions
(
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    audio_id INTEGER REFERENCES audios(id) ON DELETE CASCADE,
    started_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    ended_at TIMESTAMP
);

-- User settings table
CREATE TABLE user_settings
(
    user_id INTEGER PRIMARY KEY REFERENCES users(id) ON DELETE CASCADE,
    theme VARCHAR(50),
    notifications_enabled BOOLEAN DEFAULT TRUE,
    language VARCHAR(50) DEFAULT 'en'
);

-- Friends table (self-referencing many-to-many relationship)
CREATE TABLE friends
(
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    friend_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, friend_id)
);

-- Followers table (self-referencing many-to-many relationship)
CREATE TABLE followers
(
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    follower_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, follower_id)
);

-- ### Procedures region ###
-- Function to create a "Liked playlist" for each new user
CREATE OR REPLACE FUNCTION create_liked_playlist()
RETURNS TRIGGER AS $$
BEGIN
    INSERT INTO playlists (user_id, name, created_at)
    VALUES (NEW.id, 'Liked Playlist', CURRENT_TIMESTAMP);
    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

-- Trigger to call the function after a new user is inserted
CREATE TRIGGER after_user_insert
AFTER INSERT ON users
FOR EACH ROW
EXECUTE FUNCTION create_liked_playlist();

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
            'Description', u.description
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
    VALUES (ip_username, get_encrypted_password(ip_password),'', '', '')
    RETURNING id INTO v_user_id;

    -- Return success message
    RETURN get_result_message(0, '', '{}'::JSONB);
END;
$$;

-- Security --
-- Create the restricted_user role with login capabilities and a password
CREATE ROLE restricted_user WITH LOGIN PASSWORD '12345678';
-- Revoke all privileges from restricted_user
REVOKE ALL ON ALL TABLES IN SCHEMA public FROM restricted_user;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM restricted_user;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA public FROM restricted_user;
-- Grant execute permissions on specific functions to restricted_user
GRANT EXECUTE ON FUNCTION user_login(VARCHAR, VARCHAR) TO restricted_user;
GRANT EXECUTE ON FUNCTION validate_session(VARCHAR) TO restricted_user;
GRANT EXECUTE ON FUNCTION get_user_data(VARCHAR) TO restricted_user;
GRANT EXECUTE ON FUNCTION get_result_message(INT, VARCHAR, JSONB) TO restricted_user;
GRANT EXECUTE ON FUNCTION validate_username(VARCHAR) TO restricted_user;
GRANT EXECUTE ON FUNCTION validate_password(VARCHAR) TO restricted_user;
GRANT EXECUTE ON FUNCTION user_signup(VARCHAR, VARCHAR) TO restricted_user;

SELECT user_login('test', 'test');
SELECT validate_session('7d683b5d-6c62-474f-b666-7df5017edabc');
SELECT get_user_data('7d683b5d-6c62-474f-b666-7df5017edabc');