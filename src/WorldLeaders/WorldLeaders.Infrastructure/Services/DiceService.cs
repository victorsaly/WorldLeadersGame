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
            int diceValue = _random.Next(1, 7);
            
            // Determine new job based on dice roll
            JobLevel newJob = JobProgressionMapping.GetJobFromDiceRoll(diceValue);
            
            // Calculate income and reputation changes
            int newIncome = JobProgressionMapping.GetJobIncome(newJob);
            int reputationBonus = JobProgressionMapping.GetJobReputationBonus(newJob);
            int happinessBonus = 5; // Always positive - children feel good about trying

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
                IncomeChange = newIncome,
                ReputationChange = reputationBonus,
                HappinessChange = happinessBonus,
                PlayerReaction = "excited", // Default positive reaction
                RolledAt = DateTime.UtcNow
            };

            _context.DiceRollHistory.Add(rollHistory);
            await _context.SaveChangesAsync();

            // Create encouraging message and job description
            string encouragingMessage = GetEncouragingMessage(diceValue, newJob);
            string jobDescription = GetJobDescription(newJob);

            _logger.LogInformation("Player {PlayerId} rolled {DiceValue} and got {Job} with income {Income}", 
                playerId, diceValue, newJob, newIncome);

            return new DiceRollResult(
                diceValue,
                newJob,
                newIncome,
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
        return diceValue switch
        {
            1 => "🌱 Amazing! You rolled a 1 and became a Farmer! You're helping feed the world - that's incredibly important work!",
            2 => "🌸 Fantastic! You rolled a 2 and became a Gardener! You're making the world more beautiful and helping the environment!",
            3 => "🏪 Wonderful! You rolled a 3 and became a Shopkeeper! You're learning business skills and helping your community!",
            4 => "🎨 Excellent! You rolled a 4 and became an Artisan! Your creativity and skills are making unique and valuable things!",
            5 => "🏛️ Outstanding! You rolled a 5 and became a Politician! You're learning to lead and help make important decisions!",
            6 => "💼 Incredible! You rolled a 6 and became a Business Leader! You're developing leadership skills and creating opportunities!",
            _ => "🎲 Great roll! Every job teaches you valuable skills for your journey to becoming a world leader!"
        };
    }

    public string GetJobDescription(JobLevel job)
    {
        // Educational descriptions that teach real-world career concepts
        return job switch
        {
            JobLevel.Farmer => "Farmers grow food that feeds everyone! They understand nature, weather, and sustainable practices. This teaches you about economics through agriculture and resource management.",
            JobLevel.Gardener => "Gardeners create beautiful spaces and help the environment! They learn about plants, design, and caring for our planet. This builds skills in environmental science and community beautification.",
            JobLevel.Shopkeeper => "Shopkeepers run local businesses and serve their communities! They learn about money, customer service, and entrepreneurship. This teaches business fundamentals and community engagement.",
            JobLevel.Artisan => "Artisans create unique products with their hands and creativity! They develop specialized skills and artistic vision. This builds innovation, quality craftsmanship, and cultural appreciation.",
            JobLevel.Politician => "Politicians help make decisions that affect everyone! They learn about government, law, and public service. This teaches civic responsibility and democratic participation.",
            JobLevel.BusinessLeader => "Business Leaders create companies and jobs for others! They understand economics, management, and strategy. This develops leadership skills and economic understanding.",
            _ => "Every job is valuable and teaches important life skills!"
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
}