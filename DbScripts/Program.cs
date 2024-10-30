using Npgsql;
using Faker;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string connectionString = "Host=localhost;Port=5432;Database=lambdachessdb;Username=postgres;Password=password";

        await using (var connection = new NpgsqlConnection(connectionString))
        {
            try
            {
                await connection.OpenAsync();
                Console.WriteLine("Підключено до бази даних...");

                // Виклик методу для заповнення бази тестовими даними
                await FillDatabaseWithTestData(connection);

                // Виклик методу для виведення даних з таблиць
                await DisplayTableData(connection);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
    }

    // Метод для заповнення бази даних тестовими рендомними даними
    private static async Task FillDatabaseWithTestData(NpgsqlConnection connection)
    {
        Random random = new();
        string[] statusOptions = { "pending", "accepted", "rejected" };
        string[] tournamentStatuses = { "upcoming", "ongoing", "completed" };

        for (int i = 1; i <= 50; i++)
        {
            // Генерація випадкових гравців з використанням Faker
            string username = Internet.UserName();
            string email = Internet.Email();
            string passwordHash = Guid.NewGuid().ToString();  // Простий хеш-приклад
            var insertPlayer = $"INSERT INTO players (username, email, password_hash) VALUES ('{username}', '{email}', '{passwordHash}')";

            using (var cmd = new NpgsqlCommand(insertPlayer, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Додавання випадкових статистичних даних для гравців
            var insertPlayerStats = $"INSERT INTO player_statistics (player_id, games_played, wins, losses, draws) " +
                                    $"VALUES ({i}, {random.Next(10, 100)}, {random.Next(5, 50)}, {random.Next(3, 25)}, {random.Next(1, 10)})";
            using (var cmd = new NpgsqlCommand(insertPlayerStats, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }

            // Додавання випадкових ігор
            if (i % 2 == 0) // Для кожного другого гравця
            {
                var insertGame = $"INSERT INTO games (white_player_id, black_player_id, pgn, status, result) " +
                                 $"VALUES ({i - 1}, {i}, '1. e4 e5 2. Nf3 Nc6 3. Bb5 a6', 'completed', 'white_win')";
                using (var cmd = new NpgsqlCommand(insertGame, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Додавання випадкових друзів
            if (i % 3 == 0) // Для кожного третього гравця
            {
                var insertFriendship = $"INSERT INTO friendships (player_id_1, player_id_2, status) " +
                                       $"VALUES ({i - 2}, {i}, '{statusOptions[random.Next(3)]}')";
                using (var cmd = new NpgsqlCommand(insertFriendship, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Додавання випадкових турнірів
            if (i % 5 == 0) // Для кожного п'ятого гравця
            {
                string tournamentName = Company.Name();
                var insertTournament = $"INSERT INTO tournaments (name, status, start_date, end_date, prize_pool) " +
                                       $"VALUES ('{tournamentName}', '{tournamentStatuses[random.Next(3)]}', '2024-01-01', '2024-01-10', {random.Next(1000, 5000)})";
                using (var cmd = new NpgsqlCommand(insertTournament, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }

                // Додавання учасників турніру
                var insertTournamentParticipant = $"INSERT INTO tournament_participants (tournament_id, player_id, seed) " +
                                                  $"VALUES ({i / 5}, {i}, {random.Next(1, 100)})";
                using (var cmd = new NpgsqlCommand(insertTournamentParticipant, connection))
                {
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            // Додавання записів у таблицю leaderboard
            var insertLeaderboard = $"INSERT INTO leaderboard (player_id, rating, rank) " +
                                    $"VALUES ({i}, {random.Next(800, 2500)}, {i})";
            using (var cmd = new NpgsqlCommand(insertLeaderboard, connection))
            {
                await cmd.ExecuteNonQueryAsync();
            }
        }

        Console.WriteLine("Базу даних заповнено тестовими даними.");
    }

    // Метод для виведення даних з таблиць
    private static async Task DisplayTableData(NpgsqlConnection connection)
    {
        // Виведення гравців
        Console.WriteLine("Дані з таблиці players:");
        using (var cmd = new NpgsqlCommand("SELECT player_id, username, email, rating FROM players", connection))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"ID: {reader["player_id"]}, Username: {reader["username"]}, Email: {reader["email"]}, Rating: {reader["rating"]}");
            }
        }

        // Виведення ігор
        Console.WriteLine("\nДані з таблиці games:");
        using (var cmd = new NpgsqlCommand("SELECT game_id, white_player_id, black_player_id, status, result FROM games", connection))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Game ID: {reader["game_id"]}, White Player ID: {reader["white_player_id"]}, Black Player ID: {reader["black_player_id"]}, Status: {reader["status"]}, Result: {reader["result"]}");
            }
        }

        // Виведення турнірів
        Console.WriteLine("\nДані з таблиці tournaments:");
        using (var cmd = new NpgsqlCommand("SELECT tournament_id, name, status, prize_pool FROM tournaments", connection))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Tournament ID: {reader["tournament_id"]}, Name: {reader["name"]}, Status: {reader["status"]}, Prize Pool: {reader["prize_pool"]}");
            }
        }

        // Виведення статистики гравців
        Console.WriteLine("\nДані з таблиці player_statistics:");
        using (var cmd = new NpgsqlCommand("SELECT player_id, games_played, wins, losses, draws FROM player_statistics", connection))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Player ID: {reader["player_id"]}, Games Played: {reader["games_played"]}, Wins: {reader["wins"]}, Losses: {reader["losses"]}, Draws: {reader["draws"]}");
            }
        }

        // Виведення учасників турнірів
        Console.WriteLine("\nДані з таблиці tournament_participants:");
        using (var cmd = new NpgsqlCommand("SELECT tournament_id, player_id, seed FROM tournament_participants", connection))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Tournament ID: {reader["tournament_id"]}, Player ID: {reader["player_id"]}, Seed: {reader["seed"]}");
            }
        }

        // Виведення таблиці лідерів
        Console.WriteLine("\nДані з таблиці leaderboard:");
        using (var cmd = new NpgsqlCommand("SELECT player_id, rating, rank FROM leaderboard", connection))
        using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                Console.WriteLine($"Player ID: {reader["player_id"]}, Rating: {reader["rating"]}, Rank: {reader["rank"]}");
            }
        }
    }
}
