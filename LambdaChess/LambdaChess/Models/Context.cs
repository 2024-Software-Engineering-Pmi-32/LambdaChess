namespace LambdaChess.Models;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

// ReSharper disable InconsistentNaming
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
                               // Because Postgres names are case sensitive and during migration they become lower case,
                               // we use snake case here for better reading.
public class players {
    [Key] public int player_id { get; init; }
    [NotNull, StringLength(50)] public string? username { get; init; }
    [NotNull, StringLength(100)] public string? email { get; init; }
    [NotNull, StringLength(255)] public string? password_hash { get; init; }
    public int rating { get; init; }
    [NotNull, StringLength(50)] public string? created_at { get; init; }
}

public class games {
    [Key] public int game_id { get; init; }
    [ForeignKey(nameof(players.player_id))] public int white_player_id { get; init; }
    [ForeignKey(nameof(players.player_id))] public int black_player_id { get; init; }
    
    // ReSharper disable once EntityFramework.ModelValidation.UnlimitedStringLength
    // Because PNG can get stupidly long if both players are stalling the game it needs unlimited length for safety
    [NotNull] public string? pgn { get; init; }
    [NotNull, StringLength(20)] public string? game_status { get; init; }
    [StringLength(20)] public string? game_result { get; init; }
    [NotNull, StringLength(50)] public string? start_time { get; init; }
    [StringLength(50)] public string? end_time { get; init; }
}

public class player_statistics {
    [Key] 
    public int player_id { get; init; }
    public int games_played { get; init; }
    public int wins { get; init; }
    public int losses { get; init; }
    public int draws { get; init; }
}

public class friendships {
    [Key] public int friendship_id { get; init; }
    [ForeignKey(nameof(players.player_id))] public int player_id_1 { get; init; }
    [ForeignKey(nameof(players.player_id))] public int player_id_2 { get; init; }
    [NotNull, StringLength(20)] public string? status { get; init; }
    [NotNull, StringLength(50)] public string? created_at { get; init; }
}
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.

public class ApplicationContext : DbContext {
    private DbSet<players>? players;
    private DbSet<games>? games;
    private DbSet<player_statistics>? playerStatistics;
    private DbSet<friendships>? friendships;

    public DbSet<players> Players => players ??= Set<players>();
    public DbSet<games> Games => games ??= Set<games>();
    public DbSet<player_statistics> PlayerStatistics => playerStatistics ??= Set<player_statistics>();
    public DbSet<friendships> Friendships => friendships ??= Set<friendships>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        // Will move connection string in encrypted file later...
        optionsBuilder.UseNpgsql("Host=localhost;Database=LambdaChess;Username=postgres;Password=qwsdcvgtrfmlg;Include Error Detail=true");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<players>().ToTable("players", "public");
        modelBuilder.Entity<games>().ToTable("games", "public");
        modelBuilder.Entity<player_statistics>().ToTable("player_statistics", "public");
        modelBuilder.Entity<friendships>().ToTable("friendships", "public");
    }
}