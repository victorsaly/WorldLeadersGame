using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for AIInteractionEntity
/// Child-safe AI interaction tracking for educational purposes
/// </summary>
public class AIInteractionEntityConfiguration : IEntityTypeConfiguration<AIInteractionEntity>
{
    public void Configure(EntityTypeBuilder<AIInteractionEntity> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Table name
        builder.ToTable("AIInteractions");
        
        // AI agent classification
        builder.Property(e => e.AgentType)
            .HasConversion<string>()
            .HasComment("Type of AI agent for educational personalization");
        
        // Interaction content with child safety considerations
        builder.Property(e => e.PlayerInput)
            .HasMaxLength(1000)
            .HasComment("Child's input to AI agent (content moderated)");
        
        builder.Property(e => e.AgentResponse)
            .HasMaxLength(1000)
            .HasComment("AI agent response (age-appropriate and educational)");
        
        // Educational feedback tracking
        builder.Property(e => e.WasHelpful)
            .HasComment("Whether child found the AI interaction helpful");
        
        // Foreign key to player
        builder.Property(e => e.PlayerId)
            .HasComment("Reference to player for personalized learning");
        
        // Audit fields for child safety compliance
        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .HasComment("Interaction timestamp for session tracking");
        
        // Foreign key relationship
        builder.HasOne<PlayerEntity>()
            .WithMany()
            .HasForeignKey(e => e.PlayerId)
            .OnDelete(DeleteBehavior.Cascade)
            .HasConstraintName("FK_AIInteractions_Players_PlayerId");
        
        // Indexes for efficient querying and child safety
        builder.HasIndex(e => e.PlayerId)
            .HasDatabaseName("IX_AIInteractions_PlayerId");
        
        builder.HasIndex(e => e.AgentType)
            .HasDatabaseName("IX_AIInteractions_AgentType");
        
        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_AIInteractions_CreatedAt");
        
        builder.HasIndex(e => new { e.PlayerId, e.AgentType })
            .HasDatabaseName("IX_AIInteractions_Player_Agent");
    }
}