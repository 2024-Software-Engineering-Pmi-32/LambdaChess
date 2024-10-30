using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace LambdaChess.Models;

public class players {
    [Key] public Guid player_id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string password_hash { get; set; }
    public int rating { get; set; }
    public string created_at { get; set; }

    public players() {
        player_id = Guid.NewGuid();
    }
}

public class games {
    [Key] public Guid game_id { get; set; }
    public int white_player_id { get; set; }
    public int black_player_id { get; set; }
    public string pgn { get; set; }
    public string game_status { get; set; }
    public string game_result { get; set; }
    public string start_time { get; set; }
    public string? end_time { get; set; }
    
    public games() {
        game_id = Guid.NewGuid();
    }
}

public class player_statistics {
    [Key] public Guid player_id { get; set; }
    public int games_played { get; set; }
    public int wins { get; set; }
    public int losses { get; set; }
    public int draws { get; set; }
}

public class friendships {
    [Key] public Guid friendship_id { get; set; }
    public int player_id_1 { get; set; }
    public int player_id_2 { get; set; }
    public string status { get; set; }
    public string created_at { get; set; }

    public friendships() {
        friendship_id = Guid.NewGuid();
    }
}

public class ApplicationContext : DbContext {
    public DbSet<players> Players { get; set; }
    public DbSet<games> Games { get; set; }
    public DbSet<player_statistics> PlayerStatistics { get; set; }
    public DbSet<friendships> Friendships { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        optionsBuilder.UseNpgsql("Host=localhost;Database=LambdaChess;Username=postgres;Password=qwsdcvgtrfmlg");
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<players>().ToTable("players", "public");
        modelBuilder.Entity<games>().ToTable("games", "public");
        modelBuilder.Entity<player_statistics>().ToTable("player_statistics", "public");
        modelBuilder.Entity<friendships>().ToTable("friendships", "public");
        
        modelBuilder.Entity<players>()
            .Property(p => p.player_id)
            .HasDefaultValueSql("uuid_generate_v1()");
        modelBuilder.Entity<games>()
            .Property(g => g.game_id)
            .HasDefaultValueSql("uuid_generate_v1()");
        modelBuilder.Entity<friendships>()
            .Property(f => f.friendship_id)
            .HasDefaultValueSql("uuid_generate_v1()");
    }
}