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

    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Evening>()
            .HasOne(e => e.Host)
            .WithMany()
            .HasForeignKey(e => e.HostId)
            .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cascade delete issues

        // Configuring many-to-many relationship through EveningParticipant
        modelBuilder.Entity<EveningParticipant>()
            .HasKey(ep => new { ep.EveningId, ep.ParticipantId }); // Composite key

        modelBuilder.Entity<EveningParticipant>()
            .HasOne(ep => ep.Evening)
            .WithMany(e => e.Participants) // Assuming Participants is a collection of EveningParticipants
            .HasForeignKey(ep => ep.EveningId)
            .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cascade delete issues

        modelBuilder.Entity<EveningParticipant>()
            .HasOne(ep => ep.Participant)
            .WithMany()
            .HasForeignKey(ep => ep.ParticipantId)
            .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cascade delete issues
    }


}