-- Active: 1730359393972@@127.0.0.1@5432@postgres@public

-- Insert 3 users into the users table
INSERT INTO users (username, password_hash, avatar_url, profile_quote, description)
VALUES 
('user1', 'hashed_password1', 'http://example.com/avatar1.png', 'This is user1''s quote.', 'Description for user1.'),
('user2', 'hashed_password2', 'http://example.com/avatar2.png', 'This is user2''s quote.', 'Description for user2.'),
('user3', 'hashed_password3', 'http://example.com/avatar3.png', 'This is user3''s quote.', 'Description for user3.');