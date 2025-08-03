using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldLeaders.Infrastructure.Entities;

namespace WorldLeaders.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for LanguageProgressEntity
/// Speech recognition and pronunciation learning progress tracking
/// </summary>
public class LanguageProgressEntityConfiguration : IEntityTypeConfiguration<LanguageProgressEntity>
{
    public void Configure(EntityTypeBuilder<LanguageProgressEntity> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Table name
        builder.ToTable("LanguageProgress");
        
        // Language identification
        builder.Property(e => e.LanguageCode)
            .IsRequired()
            .HasMaxLength(2)
            .HasComment("ISO 639-1 language code for territory languages");
        
        builder.Property(e => e.LanguageName)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Human-readable language name for child UI");
        
        // Learning progress metrics
        builder.Property(e => e.AccuracyPercentage)
            .HasComment("Speech recognition accuracy (0-100%) for progress tracking");
        
        builder.Property(e => e.ChallengesCompleted)
            .HasDefaultValue(0)
            .HasComment("Number of pronunciation challenges completed");
        
        builder.Property(e => e.HasPassed)
            .HasDefaultValue(false)
            .HasComment("Whether child achieved 70%+ accuracy requirement");
        
        // Activity tracking
        builder.Property(e => e.LastPracticeAt)
            .HasComment("Last practice session timestamp");
        
        // Foreign key to player
        builder.Property(e => e.PlayerId)
            .HasComment("Reference to player for personalized learning progress");
        
        // Audit fields
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Language learning start timestamp");
        
        builder.Property(e => e.UpdatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Last progress update timestamp");
        
        // Foreign key relationship
        builder.HasOne<PlayerEntity>()
            .WithMany()
            .HasForeignKey(e => e.PlayerId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_LanguageProgress_Players_PlayerId");
        
        // Unique constraint for player-language combination
        builder.HasIndex(e => new { e.PlayerId, e.LanguageCode })
            .IsUnique()
            .HasDatabaseName("IX_LanguageProgress_Player_Language");
        
        // Additional indexes for educational analytics
        builder.HasIndex(e => e.LanguageCode)
            .HasDatabaseName("IX_LanguageProgress_LanguageCode");
        
        builder.HasIndex(e => e.AccuracyPercentage)
            .HasDatabaseName("IX_LanguageProgress_AccuracyPercentage");
        
        builder.HasIndex(e => e.HasPassed)
            .HasDatabaseName("IX_LanguageProgress_HasPassed");
        
        builder.HasIndex(e => e.LastPracticeAt)
            .HasDatabaseName("IX_LanguageProgress_LastPracticeAt");
    }
}