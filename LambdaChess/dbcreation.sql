DROP TABLE IF EXISTS tournament_participants;
DROP TABLE IF EXISTS leaderboard;
DROP TABLE IF EXISTS friendships;
DROP TABLE IF EXISTS player_statistics;
DROP TABLE IF EXISTS games;
DROP TABLE IF EXISTS tournaments;
DROP TABLE IF EXISTS players;

CREATE TABLE players (
player_id SERIAL PRIMARY KEY,
username VARCHAR(50) UNIQUE NOT NULL,
email VARCHAR(100) UNIQUE NOT NULL,
password_hash VARCHAR(255) NOT NULL,
rating INT DEFAULT 1200,
created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE games (
game_id SERIAL PRIMARY KEY,
white_player_id INT REFERENCES players(player_id) ON DELETE CASCADE,
black_player_id INT REFERENCES players(player_id) ON DELETE CASCADE,
pgn TEXT NOT NULL, -- Stores the game moves and metadata in PGN format
status VARCHAR(20) CHECK (status IN ('ongoing', 'completed', 'draw', 'resigned')),
result VARCHAR(20) CHECK (result IN ('white_win', 'black_win', 'draw')),
start_time TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
end_time TIMESTAMP
);

CREATE TABLE player_statistics (
player_id INT PRIMARY KEY REFERENCES players(player_id) ON DELETE CASCADE,
games_played INT DEFAULT 0,
wins INT DEFAULT 0,
losses INT DEFAULT 0,
draws INT DEFAULT 0
);

CREATE TABLE friendships (
friendship_id SERIAL PRIMARY KEY,
player_id_1 INT REFERENCES players(player_id) ON DELETE CASCADE,
player_id_2 INT REFERENCES players(player_id) ON DELETE CASCADE,
status VARCHAR(20) CHECK (status IN ('pending', 'accepted', 'rejected')),
created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
UNIQUE (player_id_1, player_id_2)
);

CREATE TABLE leaderboard (
leaderboard_id SERIAL PRIMARY KEY,
player_id INT REFERENCES players(player_id) ON DELETE CASCADE,
rating INT NOT NULL,
rank INT NOT NULL,
updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE tournaments (
tournament_id SERIAL PRIMARY KEY,
name VARCHAR(100) NOT NULL,
status VARCHAR(20) CHECK (status IN ('upcoming', 'ongoing', 'completed')),
start_date DATE,
end_date DATE,
prize_pool DECIMAL(10, 2)
);

CREATE TABLE tournament_participants (
tournament_id INT REFERENCES tournaments(tournament_id) ON DELETE CASCADE,
player_id INT REFERENCES players(player_id) ON DELETE CASCADE,
seed INT,
PRIMARY KEY (tournament_id, player_id)
);
