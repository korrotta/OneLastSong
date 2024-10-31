-- Active: 1730359393972@@127.0.0.1@5432@postgres@public
SELECT * from users;
SELECT * from playlists;
-- Create a view to show each user's playlists
CREATE VIEW user_playlists AS
SELECT 
    u.id AS user_id,
    u.username,
    p.id AS playlist_id,
    p.name AS playlist_name,
    p.created_at AS playlist_created_at
FROM 
    users u
JOIN 
    playlists p ON u.id = p.user_id;