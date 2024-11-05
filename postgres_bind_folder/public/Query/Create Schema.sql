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








-- Active: 1730799499974@@127.0.0.1@5432@postgres@public
-- Sessions table
CREATE TABLE sessions
(
    id SERIAL PRIMARY KEY,
    user_id INTEGER REFERENCES users(id) ON DELETE CASCADE,
    session_token VARCHAR(255) UNIQUE NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    expires_at TIMESTAMP NOT NULL
);

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE OR REPLACE FUNCTION generate_token()
RETURNS UUID AS $$
BEGIN
    RETURN uuid_generate_v4();
END;
$$ LANGUAGE plpgsql;

-- Enable the uuid-ossp extension if not already enabled
CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

-- Create a function to generate a UUID token
CREATE OR REPLACE FUNCTION generate_token()
RETURNS UUID AS $$
BEGIN
    RETURN uuid_generate_v4();
END;
$$ LANGUAGE plpgsql;

-- Create the login function
CREATE OR REPLACE FUNCTION user_login(username VARCHAR, password_hash VARCHAR)
RETURNS VARCHAR AS $$
DECLARE
    user_id INTEGER;
    session_token UUID;
    expires_at TIMESTAMP;
BEGIN
    -- Verify the user's credentials
    SELECT id INTO user_id
    FROM users
    WHERE username = username AND password_hash = password_hash;

    -- If the user is found, create or update the session
    IF user_id IS NOT NULL THEN
        expires_at := CURRENT_TIMESTAMP + INTERVAL '30 days';

        -- Check if a session already exists
        SELECT session_token INTO session_token
        FROM sessions
        WHERE user_id = user_id;

        IF session_token IS NOT NULL THEN
            -- Update the existing session
            UPDATE sessions
            SET created_at = CURRENT_TIMESTAMP, expires_at = expires_at
            WHERE user_id = user_id;
        ELSE
            -- Insert a new session
            session_token := generate_token();
            INSERT INTO sessions (user_id, session_token, created_at, expires_at)
            VALUES (user_id, session_token, CURRENT_TIMESTAMP, expires_at);
        END IF;

        -- Return the session token
        RETURN session_token::VARCHAR;
    ELSE
        -- If the user is not found, return NULL
        RETURN NULL;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Create the session validation function
CREATE OR REPLACE FUNCTION validate_session(session_token UUID)
RETURNS BOOLEAN AS $$
DECLARE
    user_id INTEGER;
BEGIN
    -- Check if the session token is valid and not expired
    SELECT user_id INTO user_id
    FROM sessions
    WHERE session_token = session_token AND expires_at > CURRENT_TIMESTAMP;

    -- If a valid session is found, return TRUE
    IF user_id IS NOT NULL THEN
        RETURN TRUE;
    ELSE
        -- If no valid session is found, return FALSE
        RETURN FALSE;
    END IF;
END;
$$ LANGUAGE plpgsql;

-- Create a function to retrieve user's data using the session token
CREATE OR REPLACE FUNCTION get_user_data(session_token UUID)
RETURNS TABLE (
    user_id INTEGER,
    username VARCHAR,
    avatar_url VARCHAR,
    profile_quote TEXT,
    description TEXT,
    playlists JSONB
) AS $$
DECLARE
    uid INTEGER;
BEGIN
    -- Validate the session token and get the user ID
    uid := validate_session(session_token);

    -- If the session is valid, retrieve the user's data
    IF uid IS NOT NULL THEN
        RETURN QUERY
        SELECT 
            u.id,
            u.username,
            u.avatar_url,
            u.profile_quote,
            u.description,
            (
                SELECT jsonb_agg(jsonb_build_object('playlist_id', p.id, 'name', p.name, 'created_at', p.created_at))
                FROM playlists p
                WHERE p.user_id = u.id
            ) AS playlists
        FROM users u
        WHERE u.id = uid;
    ELSE
        -- If the session is not valid, return an empty result
        RETURN QUERY SELECT NULL::INTEGER, NULL::VARCHAR, NULL::VARCHAR, NULL::TEXT, NULL::TEXT, NULL::JSONB;
    END IF;
END;
$$ LANGUAGE plpgsql;


-- Security --
-- Create the restricted_user role with login capabilities and a password
CREATE ROLE restricted_user WITH LOGIN PASSWORD '12345678';
-- Revoke all privileges from restricted_user
REVOKE ALL ON ALL TABLES IN SCHEMA public FROM restricted_user;
REVOKE ALL ON ALL SEQUENCES IN SCHEMA public FROM restricted_user;
REVOKE ALL ON ALL FUNCTIONS IN SCHEMA public FROM restricted_user;
-- Grant execute permissions on specific functions to restricted_user
GRANT EXECUTE ON FUNCTION user_login(VARCHAR, VARCHAR) TO restricted_user;
GRANT EXECUTE ON FUNCTION validate_session(UUID) TO restricted_user;

