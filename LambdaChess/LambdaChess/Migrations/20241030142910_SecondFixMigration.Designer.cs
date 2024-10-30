﻿// <auto-generated />
using System;
using LambdaChess.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LambdaChess.Migrations
{
    [DbContext(typeof(ApplicationContext))]
    [Migration("20241030142910_SecondFixMigration")]
    partial class SecondFixMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("LambdaChess.Models.friendships", b =>
                {
                    b.Property<Guid>("friendship_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("created_at")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("player_id_1")
                        .HasColumnType("integer");

                    b.Property<int>("player_id_2")
                        .HasColumnType("integer");

                    b.Property<string>("status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("friendship_id");

                    b.ToTable("friendships", "public");
                });

            modelBuilder.Entity("LambdaChess.Models.games", b =>
                {
                    b.Property<Guid>("game_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<int>("black_player_id")
                        .HasColumnType("integer");

                    b.Property<string>("end_time")
                        .HasColumnType("text");

                    b.Property<string>("game_result")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("game_status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("pgn")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("start_time")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("white_player_id")
                        .HasColumnType("integer");

                    b.HasKey("game_id");

                    b.ToTable("games", "public");
                });

            modelBuilder.Entity("LambdaChess.Models.player_statistics", b =>
                {
                    b.Property<Guid>("player_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("draws")
                        .HasColumnType("integer");

                    b.Property<int>("games_played")
                        .HasColumnType("integer");

                    b.Property<int>("losses")
                        .HasColumnType("integer");

                    b.Property<int>("wins")
                        .HasColumnType("integer");

                    b.HasKey("player_id");

                    b.ToTable("player_statistics", "public");
                });

            modelBuilder.Entity("LambdaChess.Models.players", b =>
                {
                    b.Property<Guid>("player_id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid")
                        .HasDefaultValueSql("uuid_generate_v4()");

                    b.Property<string>("created_at")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("password_hash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("rating")
                        .HasColumnType("integer");

                    b.Property<string>("username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("player_id");

                    b.ToTable("players", "public");
                });
#pragma warning restore 612, 618
        }
    }
}
