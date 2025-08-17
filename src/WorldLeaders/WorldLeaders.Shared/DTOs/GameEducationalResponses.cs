using System.Collections.Generic;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Shared.DTOs
{
    public class GameEventsEducationalResponse
    {
        public List<GameEventDto> Events { get; set; } = new();
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }

    public class DiceRollEducationalResponse
    {
        public int DiceValue { get; set; }
        public JobLevel NewJob { get; set; }
        public int IncomeChange { get; set; }
        public int ReputationChange { get; set; }
        public int HappinessChange { get; set; }
        public string EncouragingMessage { get; set; } = string.Empty;
        public string JobDescription { get; set; } = string.Empty;
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }

    public class GameSessionEducationalResponse
    {
        public GameSession? Session { get; set; }
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }

    public class GameStateEducationalResponse
    {
        public GameStateUpdate? StateUpdate { get; set; }
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }
}
