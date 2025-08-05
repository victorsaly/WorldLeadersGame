using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.DTOs;

/// <summary>
/// DTO for creating a new player
/// </summary>
public record CreatePlayerRequest(string Username);

/// <summary>
/// DTO for player dashboard information
/// </summary>
public record PlayerDashboardDto(
    Guid Id,
    string Username,
    int Income,
    int Reputation,
    int Happiness,
    JobLevel CurrentJob,
    int TerritoriesOwned,
    GameState CurrentGameState,
    DateTime LastActiveAt
);

/// <summary>
/// DTO for territory information display
/// </summary>
public record TerritoryDto(
    Guid Id,
    string CountryName,
    string CountryCode,
    List<string> OfficialLanguages,
    decimal GdpInBillions,
    TerritoryTier Tier,
    int Cost,
    int ReputationRequired,
    int MonthlyIncome,
    bool IsAvailable,
    bool IsOwned
);

/// <summary>
/// DTO for game events
/// </summary>
public record GameEventDto(
    Guid Id,
    string Title,
    string Description,
    EventType Type,
    int IncomeEffect,
    int ReputationEffect,
    int HappinessEffect,
    bool IsPositive,
    string IconEmoji
);

/// <summary>
/// DTO for AI agent interactions
/// </summary>
public record AIAgentRequest(
    AgentType AgentType,
    string PlayerInput,
    string GameContext
);

/// <summary>
/// DTO for AI agent responses
/// </summary>
public record AIAgentResponse(
    AgentType AgentType,
    string Response,
    bool IsAppropriate,
    DateTime GeneratedAt
);

/// <summary>
/// DTO for language learning challenges with speech recognition support
/// </summary>
public record LanguageChallengeDto(
    string LanguageCode,
    string LanguageName,
    string Word,
    string Pronunciation,
    string AudioUrl,
    int RequiredAccuracy,
    bool SupportsSpeechRecognition = true,
    string CulturalContext = "",
    ChallengeType Type = ChallengeType.BasicWord
);

/// <summary>
/// DTO for language challenge results with speech assessment
/// </summary>
public record LanguageChallengeResult(
    string LanguageCode,
    int AccuracyPercentage,
    bool Passed,
    int BonusIncome,
    string Feedback,
    SpeechAssessmentDetails? SpeechAssessment = null
);

/// <summary>
/// Speech assessment details for pronunciation feedback
/// </summary>
public record SpeechAssessmentDetails(
    double PronunciationScore,
    double FluencyScore,
    double CompletenessScore,
    string RecognizedText,
    List<WordScore> WordScores,
    bool UsedSpeechRecognition
);

/// <summary>
/// Individual word pronunciation scoring
/// </summary>
public record WordScore(
    string Word,
    double Score,
    string Feedback
);

/// <summary>
/// Types of language learning challenges
/// </summary>
public enum ChallengeType
{
    BasicWord,
    Greeting,
    CountryName,
    CulturalPhrase,
    CommonExpression
}

/// <summary>
/// DTO for game state updates
/// </summary>
public record GameStateUpdate(
    int IncomeChange,
    int ReputationChange,
    int HappinessChange,
    string Message,
    EventType? EventType = null
);

/// <summary>
/// Record for tracking resource changes for educational feedback
/// </summary>
public record ResourceChangeRecord(
    int IncomeChange,
    int ReputationChange,
    int HappinessChange,
    string Message,
    EventType? EventType,
    DateTime Timestamp
);

/// <summary>
/// DTO for AI agent personality information display
/// </summary>
public record AgentPersonalityInfo(
    AgentType AgentType,
    string Name,
    string Description,
    string Personality,
    string EducationalFocus,
    string IconEmoji,
    List<string> ExampleResponses
);
