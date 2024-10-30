using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LambdaChess.Migrations
{
    /// <inheritdoc />
    public partial class SecondFixMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "created_at",
                schema: "public",
                table: "players",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<string>(
                name: "start_time",
                schema: "public",
                table: "games",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);

            migrationBuilder.AlterColumn<string>(
                name: "end_time",
                schema: "public",
                table: "games",
                type: "text",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "created_at",
                schema: "public",
                table: "friendships",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea",
                oldRowVersion: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "created_at",
                schema: "public",
                table: "players",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<byte[]>(
                name: "start_time",
                schema: "public",
                table: "games",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<byte[]>(
                name: "end_time",
                schema: "public",
                table: "games",
                type: "bytea",
                rowVersion: true,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<byte[]>(
                name: "created_at",
                schema: "public",
                table: "friendships",
                type: "bytea",
                rowVersion: true,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
