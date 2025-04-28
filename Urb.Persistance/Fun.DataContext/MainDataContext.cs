using Fun.Domain.Fun.Models;
using Microsoft.EntityFrameworkCore;
using Urb.Domain.Urb.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Initiative> Initiatives => Set<Initiative>();
    public DbSet<Fundraising> Fundraisings => Set<Fundraising>();
    public DbSet<Donate> Donations => Set<Donate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.CreatedInitiatives)
            .WithOne(i => i.User)
            .HasForeignKey(i => i.UserId);

        modelBuilder.Entity<Initiative>()
            .HasMany(i => i.Fundraisings)
            .WithOne(f => f.Initiative)
            .HasForeignKey(f => f.InitiativeId);

        modelBuilder.Entity<Fundraising>()
            .HasMany(f => f.Donations)
            .WithOne(d => d.Fundraising)
            .HasForeignKey(d => d.FundraisingId);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Donations)
            .WithOne(d => d.User)
            .HasForeignKey(d => d.UserId);
    }
}
