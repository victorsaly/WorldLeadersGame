using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.Services;

/// <summary>
/// Interactive dice rolling service for job progression and educational mechanics
/// Child-friendly design with encouraging messages for ALL outcomes
/// </summary>
public interface IDiceService
{
    /// <summary>
    /// Roll a 6-sided die and determine job progression
    /// </summary>
    Task<DiceRollResult> RollForJobAsync(Guid playerId);
    
    /// <summary>
    /// Get encouraging message for any dice roll outcome
    /// Ensures positive educational experience for 12-year-old players
    /// </summary>
    string GetEncouragingMessage(int diceValue, JobLevel newJob);
    
    /// <summary>
    /// Get detailed job description for educational learning
    /// </summary>
    string GetJobDescription(JobLevel job);
    
    /// <summary>
    /// Get dice roll history for statistics tracking
    /// </summary>
    Task<List<DiceRollHistory>> GetDiceHistoryAsync(Guid playerId);
    
    /// <summary>
    /// Get dice roll history for statistics tracking (alias for test compatibility)
    /// </summary>
    Task<List<DiceRollHistory>> GetDiceRollHistoryAsync(Guid playerId);
    
    /// <summary>
    /// Save dice roll result to history
    /// </summary>
    Task SaveDiceRollAsync(DiceRollHistory rollHistory);
}

/// <summary>
/// Historical dice roll data for player statistics
/// </summary>
public record DiceRollHistory(
    Guid Id,
    Guid PlayerId,
    int DiceValue,
    JobLevel ResultingJob,
    int IncomeChange,
    DateTime RolledAt,
    string PlayerReaction
)
{
    /// <summary>
    /// Alias for ResultingJob to support test compatibility
    /// </summary>
    public JobLevel NewJob => ResultingJob;
};

/// <summary>
/// Job progression mapping for dice results
/// Educational design: 1-2 Basic, 3-4 Skilled, 5-6 Leadership
/// </summary>
public static class JobProgressionMapping
{
    /// <summary>
    /// Map dice roll to job level with educational progression
    /// </summary>
    public static JobLevel GetJobFromDiceRoll(int diceValue) => diceValue switch
    {
        1 => JobLevel.Farmer,
        2 => JobLevel.Gardener,
        3 => JobLevel.Shopkeeper,
        4 => JobLevel.Artisan,
        5 => JobLevel.Politician,
        6 => JobLevel.BusinessLeader,
        _ => JobLevel.Farmer
    };
    
    /// <summary>
    /// Get income for job level based on real-world career progression
    /// </summary>
    public static int GetJobIncome(JobLevel job) => job switch
    {
        JobLevel.Farmer => 1000,
        JobLevel.Gardener => 1200,
        JobLevel.Shopkeeper => 2000,
        JobLevel.Artisan => 2500,
        JobLevel.Politician => 4000,
        JobLevel.BusinessLeader => 5000,
        _ => 1000
    };
    
    /// <summary>
    /// Get reputation bonus for different job levels
    /// </summary>
    public static int GetJobReputationBonus(JobLevel job) => job switch
    {
        JobLevel.Farmer => 1,
        JobLevel.Gardener => 2,
        JobLevel.Shopkeeper => 3,
        JobLevel.Artisan => 4,
        JobLevel.Politician => 8,
        JobLevel.BusinessLeader => 10,
        _ => 1
    };
}