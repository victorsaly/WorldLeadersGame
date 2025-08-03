using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for PlayerEntity
/// Educational game database design for 12-year-old players
/// </summary>
public class PlayerEntityConfiguration : IEntityTypeConfiguration<PlayerEntity>
{
    public void Configure(EntityTypeBuilder<PlayerEntity> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Table name
        builder.ToTable("Players");
        
        // Username configuration - child-safe constraints
        builder.Property(e => e.Username)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Child-friendly username for educational game");
        
        // Game progression properties
        builder.Property(e => e.Income)
            .HasDefaultValue(1000)
            .HasComment("Monthly income from job and territories");
        
        builder.Property(e => e.Reputation)
            .HasDefaultValue(0)
            .HasComment("Reputation percentage (0-100) for territory acquisition");
        
        builder.Property(e => e.Happiness)
            .HasDefaultValue(50)
            .HasComment("Population happiness meter (0-100) - game over at 0");
        
        // Enum configurations with string conversion for readability
        builder.Property(e => e.CurrentJob)
            .HasConversion<string>()
            .HasDefaultValue(JobLevel.Farmer)
            .HasComment("Current job level from dice roll progression");
        
        builder.Property(e => e.CurrentGameState)
            .HasConversion<string>()
            .HasDefaultValue(GameState.NotStarted)
            .HasComment("Current state of the educational game session");
        
        // Audit fields for child safety and progress tracking
        builder.Property(e => e.GameStartedAt)
            .HasComment("When the child started playing the game");
        
        builder.Property(e => e.LastActiveAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Last activity timestamp for session management");
        
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Account creation timestamp");
        
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Last update timestamp - auto-updated on save");
        
        // Soft delete for child data protection
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasComment("Soft delete flag for child data protection");
        
        // Global query filter for soft deletes
        builder.HasQueryFilter(e => !e.IsDeleted);
        
        // Indexes for performance
        builder.HasIndex(e => e.Username)
            .IsUnique()
            .HasDatabaseName("IX_Players_Username");
        
        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName("IX_Players_IsDeleted");
        
        builder.HasIndex(e => e.LastActiveAt)
            .HasDatabaseName("IX_Players_LastActiveAt");
    }
}