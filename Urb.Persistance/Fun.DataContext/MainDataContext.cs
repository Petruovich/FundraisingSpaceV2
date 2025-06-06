using Fun.Domain.Fun.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Urb.Domain.Urb.Models;

public class MainDataContext : IdentityDbContext<User, Role, int>
{
    public MainDataContext(DbContextOptions<MainDataContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Initiative> Initiatives => Set<Initiative>();
    public DbSet<InitiativeStat> InitiativeStats => Set<InitiativeStat>();
    public DbSet<Fundraising> Fundraisings => Set<Fundraising>();
    public DbSet<FundraisingStat> FundraisingStats => Set<FundraisingStat>();
    public DbSet<FundraisingDailyIncome> FundraisingDailyIncomes
                                                       => Set<FundraisingDailyIncome>();
    public DbSet<Donate> Donates => Set<Donate>();
    public DbSet<Subscribe> Subscribes => Set<Subscribe>();
    public DbSet<Token> Tokens => Set<Token>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Initiative>()
            .HasOne(i => i.User)
            .WithMany(u => u.Initiatives)
            .HasForeignKey(i => i.UserId)
            .OnDelete(DeleteBehavior.Cascade);


        modelBuilder.Entity<Donate>()
            .HasOne(d => d.User)
            .WithMany(u => u.Donates)
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Subscribe>()
            .HasOne(s => s.User)
            .WithMany(u => u.Subscribes)
            .HasForeignKey(s => s.UserId)
            .OnDelete(DeleteBehavior.Cascade);

 
        modelBuilder.Entity<Subscribe>()
            .HasOne(s => s.Initiative)
            .WithMany(i => i.Subscribes)
            .HasForeignKey(s => s.InitiativeId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<Fundraising>()
            .HasOne(f => f.Initiative)
            .WithMany(i => i.Fundraisings)
            .HasForeignKey(f => f.InitiativeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Fundraising>()
            .HasOne(f => f.CreatedBy)
            .WithMany(u => u.Fundraisings)
            .HasForeignKey(f => f.CreatedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Donate>()
            .HasOne(d => d.Fundraising)
            .WithMany(f => f.Donates)
            .HasForeignKey(d => d.FundraisingId)
            .OnDelete(DeleteBehavior.Cascade);

     
        modelBuilder.Entity<Fundraising>()
            .HasOne(f => f.Stat)
            .WithOne(s => s.Fundraising)
            .HasForeignKey<FundraisingStat>(s => s.FundraisingId)
            .OnDelete(DeleteBehavior.Cascade);

       
        modelBuilder.Entity<FundraisingDailyIncome>()
            .HasOne(d => d.FundraisingStat)
            .WithMany(s => s.DailyIncomes)
            .HasForeignKey(d => d.FundraisingStatId)
            .OnDelete(DeleteBehavior.Cascade);

       
        modelBuilder.Entity<Initiative>()
            .HasOne(i => i.Stat)
            .WithOne(s => s.Initiative)
            .HasForeignKey<InitiativeStat>(s => s.InitiativeId)
            .OnDelete(DeleteBehavior.Cascade);

       
        modelBuilder.Entity<Initiative>()
            .HasOne(i => i.Category)
            .WithMany()
            .HasForeignKey(i => i.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

 
        modelBuilder.Entity<Donate>()
            .Property(d => d.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Fundraising>()
            .Property(f => f.GoalAmount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<FundraisingDailyIncome>()
            .Property(d => d.Amount)
            .HasPrecision(18, 2);

        modelBuilder.Entity<FundraisingStat>()
            .Property(s => s.Goal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<FundraisingStat>()
            .Property(s => s.TotalCollected)
            .HasPrecision(18, 2);
    }
}
