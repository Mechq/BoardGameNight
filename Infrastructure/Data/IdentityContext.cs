using Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class IdentityContext : IdentityDbContext<User>
{
    public IdentityContext(DbContextOptions<IdentityContext> options) : base(options)
    {
    }
    public DbSet<Address> Addresses { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        
        modelBuilder.Entity<Address>()
            .HasMany(a => a.Users)
            .WithOne(u => u.Address)
            .HasForeignKey(u => u.AddressId); 
    }
}