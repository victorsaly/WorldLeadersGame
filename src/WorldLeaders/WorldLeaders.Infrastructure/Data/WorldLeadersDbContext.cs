using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Infrastructure.Data.Seed;
using WorldLeaders.Shared.Models;
using System.Reflection;

namespace WorldLeaders.Infrastructure.Data;

/// <summary>
/// Entity Framework context for the World Leaders educational game
/// Configured for child safety, educational data tracking, and real-world learning
/// Enhanced with Identity framework for secure authentication
/// </summary>
public class WorldLeadersDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public WorldLeadersDbContext(DbContextOptions<WorldLeadersDbContext> options)
        : base(options)
    {
    }

    // DbSets for educational game entities
    public DbSet<CharacterPersonaEntity> CharacterPersonas { get; set; } = null!;
    public DbSet<PlayerEntity> Players { get; set; } = null!;
    public DbSet<TerritoryEntity> Territories { get; set; } = null!;
    public DbSet<GameEventEntity> GameEvents { get; set; } = null!;
    public DbSet<AIInteractionEntity> AIInteractions { get; set; } = null!;
    public DbSet<LanguageProgressEntity> LanguageProgress { get; set; } = null!;
    public DbSet<GameSessionEntity> GameSessions { get; set; } = null!;
    public DbSet<DiceRollHistoryEntity> DiceRollHistory { get; set; } = null!;
    public DbSet<PlayerAchievementEntity> PlayerAchievements { get; set; } = null!;

    // DbSets for authentication and safety entities
    public DbSet<UserSession> UserSessions { get; set; } = null!;
    public DbSet<UserCostTracking> UserCostTracking { get; set; } = null!;
    public DbSet<ChildSafetyAudit> ChildSafetyAudits { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Identity entities with Guid keys
        modelBuilder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("AspNetUsers");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            // Configure additional properties
            entity.Property(e => e.DisplayName).IsRequired().HasMaxLength(50);
            entity.Property(e => e.DateOfBirth).IsRequired();
            entity.Property(e => e.ParentalEmail).HasMaxLength(255);
            entity.Property(e => e.SchoolName).HasMaxLength(200);
            entity.Property(e => e.Role).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            
            // Computed property for IsChild
            entity.Ignore(e => e.IsChild);
            entity.Ignore(e => e.RequiresChildSafety);
            
            // Navigation to Player
            entity.HasOne(e => e.Player)
                  .WithOne()
                  .HasForeignKey<ApplicationUser>(e => e.PlayerId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable("AspNetRoles");
        });

        modelBuilder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("AspNetUserRoles");
        });

        modelBuilder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("AspNetUserClaims");
        });

        modelBuilder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("AspNetUserLogins");
        });

        modelBuilder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("AspNetRoleClaims");
        });

        modelBuilder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("AspNetUserTokens");
        });

        // Configure authentication entities
        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.SessionToken).IsRequired().HasMaxLength(255);
            entity.Property(e => e.StartedAt).IsRequired();
            entity.Property(e => e.ExpiresAt).IsRequired();
            entity.Property(e => e.LastActivityAt).IsRequired();
            entity.Property(e => e.IpAddress).HasMaxLength(45); // IPv6 length
            entity.Property(e => e.UserAgent).HasMaxLength(500);
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => e.SessionToken).IsUnique();
            entity.HasIndex(e => new { e.UserId, e.IsActive });
        });

        modelBuilder.Entity<UserCostTracking>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Date).IsRequired();
            entity.Property(e => e.AiServiceCost).HasPrecision(10, 4);
            entity.Property(e => e.SpeechServiceCost).HasPrecision(10, 4);
            entity.Property(e => e.ContentModerationCost).HasPrecision(10, 4);
            
            // Computed property for TotalCost and IsOverDailyLimit
            entity.Ignore(e => e.TotalCost);
            entity.Ignore(e => e.IsOverDailyLimit);
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.UserId, e.Date }).IsUnique();
        });

        modelBuilder.Entity<ChildSafetyAudit>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.Description).IsRequired().HasMaxLength(1000);
            entity.Property(e => e.ActionDescription).HasMaxLength(1000);
            entity.Property(e => e.Timestamp).IsRequired();
            
            entity.HasOne(e => e.User)
                  .WithMany()
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
                  
            entity.HasIndex(e => new { e.UserId, e.EventType, e.Timestamp });
        });

        // Apply all entity configurations from this assembly
        // This automatically discovers and applies all IEntityTypeConfiguration implementations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Seed educational data for game functionality
        TerritorySeeder.SeedTerritories(modelBuilder);
        GameEventSeeder.SeedGameEvents(modelBuilder);
        CharacterPersonaSeeder.SeedCharacterPersonas(modelBuilder);
    }

    /// <summary>
    /// Override SaveChanges to automatically update audit fields
    /// Ensures proper tracking for child safety and educational progress
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically update audit fields
    /// Ensures proper tracking for child safety and educational progress
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Automatically update audit fields (UpdatedAt) on entity changes
    /// Critical for child safety compliance and educational progress tracking
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // Update the UpdatedAt field if it exists
            if (entry.Entity.GetType().GetProperty("UpdatedAt") != null)
            {
                entry.Property("UpdatedAt").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
