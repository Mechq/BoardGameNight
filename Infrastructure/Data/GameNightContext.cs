using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class GameNightContext : DbContext
{
    public GameNightContext(DbContextOptions<GameNightContext> options) : base(options)
    {
    }
    public DbSet<Evening> Evenings { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<EveningParticipant> EveningParticipants { get; set; }
    public DbSet<EveningGame> EveningGame  { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Address>().Ignore(a => a.Users);

        
        modelBuilder.Entity<Evening>()
            .Property(e => e.HostId)
            .IsRequired();  

        
        modelBuilder.Entity<Evening>()
            .HasOne(e => e.Address) 
            .WithMany()  
            .HasForeignKey(e => e.AddressId)  
            .OnDelete(DeleteBehavior.Restrict);


        
        modelBuilder.Entity<EveningParticipant>()
            .HasKey(ep => new { ep.EveningId, ep.ParticipantId });

        modelBuilder.Entity<EveningParticipant>()
            .HasOne(ep => ep.Evening)
            .WithMany(e => e.Participants)
            .HasForeignKey(ep => ep.EveningId)
            .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.Entity<EveningParticipant>()
            .Property(ep => ep.ParticipantId)
            .IsRequired();  

        // EveningGame entity
        modelBuilder.Entity<EveningGame>()
            .HasKey(eg => new { eg.EveningId, eg.GameId });

        modelBuilder.Entity<EveningGame>()
            .HasOne(eg => eg.Evening)
            .WithMany(e => e.Games)
            .HasForeignKey(eg => eg.EveningId);

        modelBuilder.Entity<EveningGame>()
            .HasOne(eg => eg.Game)
            .WithMany(g => g.EveningGames)
            .HasForeignKey(eg => eg.GameId);
        
         modelBuilder.Entity<Address>().HasData(
            new Address { Id = 1, Street = "Main Street", City = "Rotterdam", HouseNumber = 42 },
            new Address { Id = 2, Street = "Baker Street", City = "London", HouseNumber = 221 }
        );

        modelBuilder.Entity<Game>().HasData(
            new Game
            {
                Id = 1,
                Name = "Catan",
                Description = "A strategic board game of trading and building.",
                Genre = Genre.Fantasy,
                TypeOfGame = GameType.BoardGame,
                ImageURL = "https://example.com/catan.jpg",
                IsAgeRestricted = false
            },
            new Game
            {
                Id = 2,
                Name = "Call of Duty",
                Description = "Fast-paced first-person shooter video game.",
                Genre = Genre.SciFi,
                TypeOfGame = GameType.VideoGame,
                ImageURL = "https://example.com/cod.jpg",
                IsAgeRestricted = true
            },
            new Game
            {
                Id = 3,
                Name = "UNO",
                Description = "A classic card game of colors and numbers.",
                Genre = Genre.Other,
                TypeOfGame = GameType.Cards,
                ImageURL = "https://example.com/uno.jpg",
                IsAgeRestricted = false
            }
        );

        modelBuilder.Entity<Evening>().HasData(
            new Evening
            {
                Id = 1,
                HostId = "host123",
                MaxUsers = 8,
                HostDate = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                Allergy = "Peanuts",
                AddressId = 1
            },
            new Evening
            {
                Id = 2,
                HostId = "host456",
                MaxUsers = 10,
                HostDate = DateOnly.FromDateTime(DateTime.Now.AddDays(14)),
                Allergy = "None",
                AddressId = 2
            }
        );

        modelBuilder.Entity<EveningGame>().HasData(
            new EveningGame { EveningId = 1, GameId = 1 },
            new EveningGame { EveningId = 1, GameId = 3 },
            new EveningGame { EveningId = 2, GameId = 2 }
        );
    
    }
    



}