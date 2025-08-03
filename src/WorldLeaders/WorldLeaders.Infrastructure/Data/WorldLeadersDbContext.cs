using Microsoft.EntityFrameworkCore;
using WorldLeaders.Infrastructure.Entities;

namespace WorldLeaders.Infrastructure.Data;

/// <summary>
/// Entity Framework context for the World Leaders educational game
/// </summary>
public class WorldLeadersDbContext : DbContext
{
    public WorldLeadersDbContext(DbContextOptions<WorldLeadersDbContext> options)
        : base(options)
    {
    }

    public DbSet<PlayerEntity> Players { get; set; }
    public DbSet<TerritoryEntity> Territories { get; set; }
    public DbSet<GameEventEntity> GameEvents { get; set; }
    public DbSet<AIInteractionEntity> AIInteractions { get; set; }
    public DbSet<LanguageProgressEntity> LanguageProgress { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure Player entity
        modelBuilder.Entity<PlayerEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(50);
            entity.Property(e => e.Income)
                .HasDefaultValue(1000); // Start with farmer income
            entity.Property(e => e.Reputation)
                .HasDefaultValue(0);
            entity.Property(e => e.Happiness)
                .HasDefaultValue(50);
            entity.Property(e => e.CurrentJob)
                .HasConversion<string>()
                .HasDefaultValue("Farmer");
            entity.Property(e => e.CurrentGameState)
                .HasConversion<string>()
                .HasDefaultValue("NotStarted");

            // Soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure Territory entity
        modelBuilder.Entity<TerritoryEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CountryName)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.CountryCode)
                .IsRequired()
                .HasMaxLength(2);
            entity.Property(e => e.GdpInBillions)
                .HasColumnType("decimal(18,2)");
            entity.Property(e => e.OfficialLanguagesJson)
                .HasColumnName("OfficialLanguages")
                .HasMaxLength(500);
            entity.Property(e => e.Tier)
                .HasConversion<string>();

            // Index for efficient querying
            entity.HasIndex(e => e.CountryCode).IsUnique();
            entity.HasIndex(e => e.IsAvailable);

            // Soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure GameEvent entity
        modelBuilder.Entity<GameEventEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(100);
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(500);
            entity.Property(e => e.Type)
                .HasConversion<string>();

            // Soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
        });

        // Configure AIInteraction entity
        modelBuilder.Entity<AIInteractionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AgentType)
                .HasConversion<string>();
            entity.Property(e => e.PlayerInput)
                .HasMaxLength(1000);
            entity.Property(e => e.AgentResponse)
                .HasMaxLength(1000);

            // Foreign key to Player
            entity.HasOne<PlayerEntity>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for player queries
            entity.HasIndex(e => e.PlayerId);
        });

        // Configure LanguageProgress entity
        modelBuilder.Entity<LanguageProgressEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.LanguageCode)
                .IsRequired()
                .HasMaxLength(2);
            entity.Property(e => e.LanguageName)
                .IsRequired()
                .HasMaxLength(50);

            // Foreign key to Player
            entity.HasOne<PlayerEntity>()
                .WithMany()
                .HasForeignKey(e => e.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Unique constraint for player-language combination
            entity.HasIndex(e => new { e.PlayerId, e.LanguageCode }).IsUnique();
        });
    }
}
