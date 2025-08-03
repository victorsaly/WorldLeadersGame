using Microsoft.EntityFrameworkCore;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Infrastructure.Data.Seed;
using System.Reflection;

namespace WorldLeaders.Infrastructure.Data;

/// <summary>
/// Entity Framework context for the World Leaders educational game
/// Configured for child safety, educational data tracking, and real-world learning
/// </summary>
public class WorldLeadersDbContext : DbContext
{
    public WorldLeadersDbContext(DbContextOptions<WorldLeadersDbContext> options)
        : base(options)
    {
    }

    // DbSets for educational game entities
    public DbSet<PlayerEntity> Players { get; set; } = null!;
    public DbSet<TerritoryEntity> Territories { get; set; } = null!;
    public DbSet<GameEventEntity> GameEvents { get; set; } = null!;
    public DbSet<AIInteractionEntity> AIInteractions { get; set; } = null!;
    public DbSet<LanguageProgressEntity> LanguageProgress { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from this assembly
        // This automatically discovers and applies all IEntityTypeConfiguration implementations
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Seed educational data for game functionality
        TerritorySeeder.SeedTerritories(modelBuilder);
        GameEventSeeder.SeedGameEvents(modelBuilder);
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
