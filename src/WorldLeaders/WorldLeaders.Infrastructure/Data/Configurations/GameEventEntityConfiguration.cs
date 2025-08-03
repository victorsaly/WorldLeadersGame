using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for GameEventEntity
/// Educational events that teach decision-making and consequences
/// </summary>
public class GameEventEntityConfiguration : IEntityTypeConfiguration<GameEventEntity>
{
    public void Configure(EntityTypeBuilder<GameEventEntity> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Table name
        builder.ToTable("GameEvents");
        
        // Event information
        builder.Property(e => e.Title)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Child-friendly event title");
        
        builder.Property(e => e.Description)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("Educational event description for 12-year-olds");
        
        // Event classification
        builder.Property(e => e.Type)
            .HasConversion<string>()
            .HasComment("Event category for educational learning");
        
        // Game effects for learning consequences
        builder.Property(e => e.IncomeEffect)
            .HasComment("Income change from event (positive or negative)");
        
        builder.Property(e => e.ReputationEffect)
            .HasComment("Reputation change from event");
        
        builder.Property(e => e.HappinessEffect)
            .HasComment("Population happiness change from event");
        
        // Event characteristics
        builder.Property(e => e.IsPositive)
            .HasComment("Whether event is generally positive for learning");
        
        builder.Property(e => e.IconEmoji)
            .HasMaxLength(10)
            .HasDefaultValue("🎲")
            .HasComment("Child-friendly emoji for visual representation");
        
        // Audit fields
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Event creation timestamp");
        
        // Soft delete
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasComment("Soft delete flag for content management");
        
        // Global query filter for soft deletes
        builder.HasQueryFilter(e => !e.IsDeleted);
        
        // Indexes for efficient querying
        builder.HasIndex(e => e.Type)
            .HasDatabaseName("IX_GameEvents_Type");
        
        builder.HasIndex(e => e.IsPositive)
            .HasDatabaseName("IX_GameEvents_IsPositive");
        
        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName("IX_GameEvents_IsDeleted");
    }
}