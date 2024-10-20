using Domain;
using Microsoft.EntityFrameworkCore;

namespace Presentation.Data;

public class GameNightContext : DbContext
{
    public GameNightContext(DbContextOptions<GameNightContext> options) : base(options)
    {
    }
    public DbSet<Evening> Evenings { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Game> Games { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<EveningParticipant> EveningParticipants { get; set; }
    public DbSet<EveningGame> EveningGame  { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
     modelBuilder.Entity<Evening>()
            .HasOne(e => e.Host)
            .WithMany()
            .HasForeignKey(e => e.HostId)
            .OnDelete(DeleteBehavior.Restrict); 

        
        modelBuilder.Entity<EveningParticipant>()
            .HasKey(ep => new { ep.EveningId, ep.ParticipantId }); 

        modelBuilder.Entity<EveningParticipant>()
            .HasOne(ep => ep.Evening)
            .WithMany(e => e.Participants) 
            .HasForeignKey(ep => ep.EveningId)
            .OnDelete(DeleteBehavior.Restrict); 

        modelBuilder.Entity<EveningParticipant>()
            .HasOne(ep => ep.Participant)
            .WithMany()
            .HasForeignKey(ep => ep.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict); 
        
        
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