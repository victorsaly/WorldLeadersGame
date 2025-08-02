namespace WorldLeaders.Shared.Constants;

/// <summary>
/// Game configuration constants for the World Leaders educational game
/// </summary>
public static class GameConstants
{
    // Win/Loss Conditions
    public const int MaxReputation = 100;
    public const int MinHappiness = 0;
    public const int WinReputation = 100;
    public const int LoseHappiness = 0;

    // Job Income Levels (monthly)
    public const int FarmerIncome = 1000;
    public const int GardenerIncome = 1200;
    public const int ShopkeeperIncome = 2000;
    public const int ArtisanIncome = 2500;
    public const int PoliticianIncome = 4000;
    public const int BusinessLeaderIncome = 5000;

    // Territory Costs and Requirements
    public const int SmallTerritoryBaseCost = 5000;
    public const int MediumTerritoryBaseCost = 50000;
    public const int MajorTerritoryBaseCost = 200000;

    public const int SmallTerritoryReputationRequired = 10;
    public const int MediumTerritoryReputationRequired = 40;
    public const int MajorTerritoryReputationRequired = 85;

    // Language Learning
    public const int LanguageChallengeBonus = 500; // Income bonus for completing language challenges
    public const int PronunciationAccuracyRequired = 70; // Minimum accuracy percentage

    // AI Agent Limits
    public const int MaxAIResponseLength = 500; // Characters
    public const int MaxDailyAIInteractions = 50; // Per player
    
    // Game Timing
    public const int TurnDurationMinutes = 10;
    public const int MonthlyIncomeInterval = 30; // Seconds in dev, days in production
}

/// <summary>
/// UI and accessibility constants for child-friendly design
/// </summary>
public static class UIConstants
{
    // Colors (TailwindCSS classes)
    public const string PrimaryColor = "bg-blue-500";
    public const string SecondaryColor = "bg-green-500";
    public const string WarningColor = "bg-yellow-500";
    public const string DangerColor = "bg-red-500";
    public const string SuccessColor = "bg-emerald-500";

    // Font Sizes (child-friendly)
    public const string HeaderFontSize = "text-2xl";
    public const string BodyFontSize = "text-lg";
    public const string SmallFontSize = "text-base";

    // Animation Durations
    public const string FastTransition = "transition-all duration-200";
    public const string MediumTransition = "transition-all duration-500";
    public const string SlowTransition = "transition-all duration-1000";
}
