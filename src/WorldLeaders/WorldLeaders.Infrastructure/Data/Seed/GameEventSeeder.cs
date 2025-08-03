using Microsoft.EntityFrameworkCore;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Infrastructure.Data.Seed;

/// <summary>
/// Seed data for educational game events
/// Age-appropriate events that teach decision-making and consequences
/// </summary>
public static class GameEventSeeder
{
    /// <summary>
    /// Seeds the database with educational game events for 12-year-old players
    /// Events teach economics, social responsibility, and decision-making skills
    /// </summary>
    public static void SeedGameEvents(ModelBuilder modelBuilder)
    {
        var events = GetEducationalGameEvents();
        
        modelBuilder.Entity<GameEventEntity>().HasData(events);
    }

    /// <summary>
    /// Get age-appropriate educational game events
    /// Each event teaches valuable lessons while being fun and engaging
    /// </summary>
    private static List<GameEventEntity> GetEducationalGameEvents()
    {
        return new List<GameEventEntity>
        {
            // Positive Career Events
            CreateEvent("Excellent Work Performance!", 
                "Your hard work has been noticed by your supervisor. Great job!", 
                EventType.Career, 500, 5, 10, true, "⭐"),
            
            CreateEvent("Community Award", 
                "You received an award for helping your community. People appreciate your kindness!", 
                EventType.Social, 200, 10, 15, true, "🏆"),
            
            CreateEvent("New Skill Learned", 
                "You learned a valuable new skill that makes you better at your job!", 
                EventType.Career, 300, 3, 8, true, "📚"),
            
            // Positive Economic Events
            CreateEvent("Market Success", 
                "The economy is doing well, and your investments are growing!", 
                EventType.Economic, 800, 5, 5, true, "📈"),
            
            CreateEvent("International Trade Deal", 
                "A new trade agreement brings prosperity to your region!", 
                EventType.International, 600, 8, 12, true, "🤝"),
            
            // Challenging but Educational Events
            CreateEvent("Economic Challenge", 
                "Economic conditions are tough, but this is a chance to learn resilience!", 
                EventType.Economic, -300, -2, -5, false, "📉"),
            
            CreateEvent("Natural Weather Event", 
                "Unexpected weather affects crops, teaching us about nature's impact on economy.", 
                EventType.Natural, -200, 0, -3, false, "🌧️"),
            
            CreateEvent("Learning Opportunity", 
                "A difficult situation becomes a chance to grow and learn new skills!", 
                EventType.Career, -100, 2, 5, false, "🎓"),
            
            // Social and Community Events
            CreateEvent("Community Festival", 
                "Your neighborhood organizes a wonderful festival that brings everyone together!", 
                EventType.Social, 100, 3, 20, true, "🎉"),
            
            CreateEvent("Helping Others", 
                "You helped someone in need, and it made everyone feel good!", 
                EventType.Social, 0, 5, 15, true, "❤️"),
            
            CreateEvent("Cultural Exchange", 
                "You learned about a different culture and made new friends!", 
                EventType.International, 150, 7, 10, true, "🌍"),
            
            // Educational Natural Events
            CreateEvent("Beautiful Harvest", 
                "Nature provided an excellent harvest this year. Farmers are happy!", 
                EventType.Natural, 400, 2, 8, true, "🌾"),
            
            CreateEvent("Scientific Discovery", 
                "Scientists in your region made an amazing discovery that helps everyone!", 
                EventType.Career, 300, 8, 12, true, "🔬"),
            
            // Character Building Events
            CreateEvent("Acts of Kindness", 
                "Your kind actions inspired others to be kind too!", 
                EventType.Social, 50, 8, 25, true, "😊"),
            
            CreateEvent("Problem Solving", 
                "You found a creative solution to a community problem!", 
                EventType.Social, 200, 10, 15, true, "💡"),
            
            CreateEvent("Environmental Care", 
                "Your efforts to protect the environment are appreciated by everyone!", 
                EventType.Natural, 150, 5, 18, true, "🌱")
        };
    }

    /// <summary>
    /// Helper method to create educational game event entities
    /// Ensures all events are age-appropriate and educational
    /// </summary>
    private static GameEventEntity CreateEvent(
        string title,
        string description,
        EventType type,
        int incomeEffect,
        int reputationEffect,
        int happinessEffect,
        bool isPositive,
        string iconEmoji)
    {
        return new GameEventEntity
        {
            Id = Guid.NewGuid(),
            Title = title,
            Description = description,
            Type = type,
            IncomeEffect = incomeEffect,
            ReputationEffect = reputationEffect,
            HappinessEffect = happinessEffect,
            IsPositive = isPositive,
            IconEmoji = iconEmoji,
            CreatedAt = DateTime.UtcNow,
            IsDeleted = false
        };
    }
}