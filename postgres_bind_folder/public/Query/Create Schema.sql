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