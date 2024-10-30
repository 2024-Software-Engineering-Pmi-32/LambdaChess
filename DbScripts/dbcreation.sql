DROP TABLE IF EXISTS friendships;
DROP TABLE IF EXISTS player_statistics;
DROP TABLE IF EXISTS games;
DROP TABLE IF EXISTS players;

CREATE TABLE players (
	player_id SERIAL PRIMARY KEY,
	username VARCHAR(50) UNIQUE NOT NULL,
	email VARCHAR(100) UNIQUE NOT NULL,
	password_hash VARCHAR(255) NOT NULL,
	rating INT DEFAULT 800,
	created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE games (
	game_id SERIAL PRIMARY KEY,
	white_player_id INT REFERENCES players(player_id) ON DELETE CASCADE,
	black_player_id INT REFERENCES players(player_id) ON DELETE CASCADE,
	pgn TEXT NOT NULL, -- Stores the game moves and metadata in PGN format
	game_status VARCHAR(20) CHECK (game_status IN ('ongoing', 'completed', 'draw', 'resigned')),
	game_result VARCHAR(20) CHECK (game_result IN ('white_win', 'black_win', 'draw')),
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
