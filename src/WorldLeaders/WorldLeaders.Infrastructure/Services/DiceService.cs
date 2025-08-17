using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Entities;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Interactive dice rolling service implementation for educational job progression
/// Context: Child-friendly dice mechanics for 12-year-old players
/// Educational Objective: Teach career progression and encouraging resilience
/// Safety: Positive messaging for ALL dice outcomes, no negative feedback
/// </summary>
public class DiceService : IDiceService
{
    private readonly WorldLeadersDbContext _context;
    private readonly ILogger<DiceService> _logger;
    private readonly Random _random;

    public DiceService(WorldLeadersDbContext context, ILogger<DiceService> logger)
    {
        _context = context;
        _logger = logger;
        _random = new Random();
    }

    public async Task<DiceRollResult> RollForJobAsync(Guid playerId)
    {
        try
        {
            var player = await _context.Players.FindAsync(playerId);
            if (player == null)
            {
                throw new ArgumentException($"Player {playerId} not found");
            }

            // Roll the dice (1-6)
            var diceValue = _random.Next(1, 7);
            
            // Determine new job based on dice roll
            var newJob = JobProgressionMapping.GetJobFromDiceRoll(diceValue);
            
            // Calculate income and reputation changes
            var newIncome = JobProgressionMapping.GetJobIncome(newJob);
            var incomeChange = newIncome - player.Income;
            var reputationBonus = JobProgressionMapping.GetJobReputationBonus(newJob);
            var happinessBonus = 5; // Always positive - children feel good about trying

            // Update player stats
            player.CurrentJob = newJob;
            player.Income = newIncome;
            player.Reputation = Math.Min(100, player.Reputation + reputationBonus);
            player.Happiness = Math.Min(100, player.Happiness + happinessBonus);
            player.LastActiveAt = DateTime.UtcNow;

            // Save dice roll history
            var rollHistory = new DiceRollHistoryEntity
            {
                PlayerId = playerId,
                DiceValue = diceValue,
                ResultingJob = newJob,
                IncomeChange = incomeChange,
                ReputationChange = reputationBonus,
                HappinessChange = happinessBonus,
                PlayerReaction = "excited", // Default positive reaction
                RolledAt = DateTime.UtcNow
            };

            _context.DiceRollHistory.Add(rollHistory);
            await _context.SaveChangesAsync();

            // Create encouraging message and job description
            var encouragingMessage = GetEncouragingMessage(diceValue, newJob);
            var jobDescription = GetJobDescription(newJob);

            _logger.LogInformation("Player {PlayerId} rolled {DiceValue} and got {Job} with income {Income}", 
                playerId, diceValue, newJob, newIncome);

            return new DiceRollResult(
                diceValue,
                newJob,
                incomeChange,
                reputationBonus,
                happinessBonus,
                encouragingMessage,
                jobDescription
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rolling dice for player {PlayerId}", playerId);
            throw;
        }
    }

    public string GetEncouragingMessage(int diceValue, JobLevel newJob)
    {
        // Educational principle: ALL outcomes are positive and encouraging
        var jobName = newJob.ToString();
        
        return diceValue switch
        {
            1 => $"🌱 Amazing! You rolled a 1 and your new role is {jobName}! You're learning to help others and developing valuable skills through education!",
            2 => $"🌸 Fantastic! You rolled a 2 and your new role is {jobName}! You're growing your knowledge and understanding the world better through learning!",
            3 => $"🏪 Wonderful! You rolled a 3 and your new role is {jobName}! You're building important skills and helping your community through education and progress!",
            4 => $"🎨 Excellent! You rolled a 4 and your new role is {jobName}! Your creativity and skills are growing as you learn to make unique contributions with advanced knowledge!",
            5 => $"🏛️ Excellent! You rolled a 5 and your new role is {jobName}! You're learning to lead and understand how to make important decisions through education!",
            6 => $"💼 Outstanding! You rolled a 6 and your new role is {jobName}! You're developing leadership skills and creating opportunities through advanced education and learning!",
            _ => $"🎲 Great roll! Your new role as {jobName} teaches you valuable skills and knowledge for your journey to become a world leader through education!"
        };
    }

    public string GetJobDescription(JobLevel job)
    {
        // Educational descriptions that teach real-world career concepts
        return job switch
        {
            JobLevel.Student => "Students learn and grow every day! They develop knowledge, critical thinking, and explore different subjects. This educational foundation helps you build skills and prepares you for your future career journey!",
            JobLevel.Farmer => "Farmers grow food that feeds everyone! They learn about nature, weather, and sustainable practices. This helps you develop knowledge about economics through agriculture and resource management.",
            JobLevel.Gardener => "Gardeners create beautiful spaces and help the environment! They learn about plants, design, and caring for our planet. This work builds skills in environmental science and community beautification.",
            JobLevel.Shopkeeper => "Shopkeepers run local businesses and help their communities! They learn about money, customer service, and entrepreneurship. This work teaches business skills and community engagement.",
            JobLevel.Artisan => "Artisans create unique products with their hands and creativity! They develop specialized skills and artistic vision. This work builds innovation knowledge, quality craftsmanship, and cultural appreciation.",
            JobLevel.Politician => "Politicians help make decisions that affect everyone! They learn about government, law, and public service. This work teaches civic responsibility and democratic participation skills.",
            JobLevel.BusinessLeader => "Business Leaders create companies and jobs for others! They learn about economics, management, and strategy. This work develops leadership skills and economic knowledge.",
            JobLevel.Teacher => "Teachers share knowledge and help others learn! They develop communication skills and educational expertise. This work builds understanding of how to guide and inspire others to grow.",
            JobLevel.Manager => "Managers coordinate teams and projects! They learn leadership, organization, and strategic thinking. This work teaches advanced skills in guiding others and achieving goals through education.",
            _ => "Every job is valuable and helps you learn important life skills through knowledge and education!"
        };
    }

    public async Task<List<DiceRollHistory>> GetDiceHistoryAsync(Guid playerId)
    {
        try
        {
            var history = await _context.DiceRollHistory
                .Where(h => h.PlayerId == playerId)
                .OrderByDescending(h => h.RolledAt)
                .Take(10) // Last 10 rolls for UI display
                .ToListAsync();

            return history.Select(h => new DiceRollHistory(
                h.Id,
                h.PlayerId,
                h.DiceValue,
                h.ResultingJob,
                h.IncomeChange,
                h.RolledAt,
                h.PlayerReaction
            )).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting dice history for player {PlayerId}", playerId);
            return new List<DiceRollHistory>();
        }
    }

    public async Task SaveDiceRollAsync(DiceRollHistory rollHistory)
    {
        try
        {
            var entity = new DiceRollHistoryEntity
            {
                Id = rollHistory.Id,
                PlayerId = rollHistory.PlayerId,
                DiceValue = rollHistory.DiceValue,
                ResultingJob = rollHistory.ResultingJob,
                IncomeChange = rollHistory.IncomeChange,
                PlayerReaction = rollHistory.PlayerReaction,
                RolledAt = rollHistory.RolledAt
            };

            _context.DiceRollHistory.Add(entity);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Saved dice roll history for player {PlayerId}", rollHistory.PlayerId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving dice roll history for player {PlayerId}", rollHistory.PlayerId);
            throw;
        }
    }

    /// <summary>
    /// Get dice roll history for a player (alias for test compatibility)
    /// Educational Objective: Allow players to review their career progression journey
    /// </summary>
    public async Task<List<DiceRollHistory>> GetDiceRollHistoryAsync(Guid playerId)
    {
        return await GetDiceHistoryAsync(playerId);
    }
}