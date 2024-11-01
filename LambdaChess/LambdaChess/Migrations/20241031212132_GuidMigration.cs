using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LambdaChess.Migrations
{
    /// <inheritdoc />
    public partial class GuidMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "player_id",
                schema: "public",
                table: "players",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v1()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<Guid>(
                name: "game_id",
                schema: "public",
                table: "games",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v1()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");

            migrationBuilder.AlterColumn<Guid>(
                name: "friendship_id",
                schema: "public",
                table: "friendships",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v1()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v4()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "player_id",
                schema: "public",
                table: "players",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v1()");

            migrationBuilder.AlterColumn<Guid>(
                name: "game_id",
                schema: "public",
                table: "games",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v1()");

            migrationBuilder.AlterColumn<Guid>(
                name: "friendship_id",
                schema: "public",
                table: "friendships",
                type: "uuid",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "uuid_generate_v1()");
        }
    }
}
