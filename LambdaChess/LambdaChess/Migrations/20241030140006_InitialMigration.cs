using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LambdaChess.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "friendships",
                schema: "public",
                columns: table => new
                {
                    friendship_id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id_1 = table.Column<int>(type: "integer", nullable: false),
                    player_id_2 = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_friendships", x => x.friendship_id);
                });

            migrationBuilder.CreateTable(
                name: "games",
                schema: "public",
                columns: table => new
                {
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    white_player_id = table.Column<int>(type: "integer", nullable: false),
                    black_player_id = table.Column<int>(type: "integer", nullable: false),
                    pgn = table.Column<string>(type: "text", nullable: false),
                    game_status = table.Column<string>(type: "text", nullable: false),
                    game_result = table.Column<string>(type: "text", nullable: false),
                    start_time = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false),
                    end_time = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.game_id);
                });

            migrationBuilder.CreateTable(
                name: "player_statistics",
                schema: "public",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    games_played = table.Column<int>(type: "integer", nullable: false),
                    wins = table.Column<int>(type: "integer", nullable: false),
                    losses = table.Column<int>(type: "integer", nullable: false),
                    draws = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_player_statistics", x => x.player_id);
                });

            migrationBuilder.CreateTable(
                name: "players",
                schema: "public",
                columns: table => new
                {
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    username = table.Column<string>(type: "text", nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: false),
                    rating = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<byte[]>(type: "bytea", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.player_id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "friendships",
                schema: "public");

            migrationBuilder.DropTable(
                name: "games",
                schema: "public");

            migrationBuilder.DropTable(
                name: "player_statistics",
                schema: "public");

            migrationBuilder.DropTable(
                name: "players",
                schema: "public");
        }
    }
}
