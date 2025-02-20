using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ChallengeContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Team> Teams { get; set; }
    public DbSet<Counter> Counters { get; set; }

    public ChallengeContext(DbContextOptions<ChallengeContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        EntityTypeBuilder<User> userEntity = modelBuilder.Entity<User>();
        userEntity.HasKey(u => u.Id);

        EntityTypeBuilder<Team> teamEntity = modelBuilder.Entity<Team>();
        teamEntity.HasKey(g => g.Id);

        EntityTypeBuilder<Counter> counterEntity = modelBuilder.Entity<Counter>();
        counterEntity.HasKey(c => c.Id);
        counterEntity.Property(c => c.UpdatedAt).HasColumnType("timestamp with time zone");

        // Add Relationships
        // User - Group (One-to-many, User belongs to one Group)
        userEntity.HasOne(u => u.Team)
                  .WithMany(t => t.Users)
                  .HasForeignKey(u => u.TeamId)
                  .OnDelete(DeleteBehavior.SetNull);

        // User - Counter (One-to-many, Cascade delete for Counters when User is deleted)
        userEntity.HasMany(u => u.Counters)
                  .WithOne(c => c.User)
                  .HasForeignKey(c => c.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
    }
}