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
    }



}