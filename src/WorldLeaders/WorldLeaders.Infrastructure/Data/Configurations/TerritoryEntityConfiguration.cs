using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for TerritoryEntity
/// Real-world GDP-based territories for educational economics learning
/// </summary>
public class TerritoryEntityConfiguration : IEntityTypeConfiguration<TerritoryEntity>
{
    public void Configure(EntityTypeBuilder<TerritoryEntity> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Table name
        builder.ToTable("Territories");
        
        // Country information configuration
        builder.Property(e => e.CountryName)
            .IsRequired()
            .HasMaxLength(100)
            .HasComment("Official country name for geography education");
        
        builder.Property(e => e.CountryCode)
            .IsRequired()
            .HasMaxLength(2)
            .HasComment("ISO 3166-1 alpha-2 country code");
        
        // Economic data for educational learning
        builder.Property(e => e.GdpInBillions)
            .HasColumnType("decimal(18,2)")
            .HasComment("GDP in billions USD for economics education");
        
        builder.Property(e => e.RealGDP)
            .HasComment("Real GDP in USD from World Bank data");
        
        builder.Property(e => e.GDPRank)
            .HasComment("World GDP ranking for educational context");
        
        // Game mechanics configuration
        builder.Property(e => e.Cost)
            .HasComment("Purchase cost in game currency");
        
        builder.Property(e => e.ReputationRequired)
            .HasComment("Minimum reputation percentage (0-100) required");
        
        builder.Property(e => e.MonthlyIncome)
            .HasComment("Monthly income generated when owned");
        
        // Territory tier for difficulty progression
        builder.Property(e => e.Tier)
            .HasConversion<string>()
            .HasDefaultValue(TerritoryTier.Small)
            .HasComment("Territory difficulty tier based on GDP ranking");
        
        // Language learning integration
        builder.Property(e => e.OfficialLanguagesJson)
            .HasColumnName("OfficialLanguages")
            .HasMaxLength(500)
            .HasComment("JSON array of official languages for speech challenges");
        
        // Ownership and availability
        builder.Property(e => e.IsAvailable)
            .HasDefaultValue(true)
            .HasComment("Whether territory is available for purchase");
        
        builder.Property(e => e.OwnedByPlayerId)
            .HasComment("Player ID who owns this territory (nullable)");
        
        builder.Property(e => e.AcquiredAt)
            .HasComment("When territory was acquired by current owner");
        
        // Audit fields
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Territory record creation timestamp");
        
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Last update timestamp");
        
        // Soft delete for data integrity
        builder.Property(e => e.IsDeleted)
            .HasDefaultValue(false)
            .HasComment("Soft delete flag for data protection");
        
        // Global query filter for soft deletes
        builder.HasQueryFilter(e => !e.IsDeleted);
        
        // Indexes for efficient querying
        builder.HasIndex(e => e.CountryCode)
            .IsUnique()
            .HasDatabaseName("IX_Territories_CountryCode");
        
        builder.HasIndex(e => e.IsAvailable)
            .HasDatabaseName("IX_Territories_IsAvailable");
        
        builder.HasIndex(e => e.Tier)
            .HasDatabaseName("IX_Territories_Tier");
        
        builder.HasIndex(e => e.GDPRank)
            .HasDatabaseName("IX_Territories_GDPRank");
        
        builder.HasIndex(e => e.OwnedByPlayerId)
            .HasDatabaseName("IX_Territories_OwnedByPlayerId");
        
        builder.HasIndex(e => e.IsDeleted)
            .HasDatabaseName("IX_Territories_IsDeleted");
        
        // Foreign key relationship to Player
        builder.HasOne<PlayerEntity>()
            .WithMany()
            .HasForeignKey(e => e.OwnedByPlayerId)
            .OnDelete(DeleteBehavior.SetNull)
            .HasConstraintName("FK_Territories_Players_OwnedByPlayerId");
    }
}