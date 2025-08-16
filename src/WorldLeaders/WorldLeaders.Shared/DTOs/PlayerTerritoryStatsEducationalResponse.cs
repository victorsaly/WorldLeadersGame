using System;
using System.Collections.Generic;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.DTOs
{
    public class PlayerTerritoryStatsEducationalResponse
    {
        public Guid PlayerId { get; set; }
        public int TotalTerritories { get; set; }
        public int MonthlyTerritoryIncome { get; set; }
        public Dictionary<TerritoryTier, int> TerritoriesByTier { get; set; } = new();
        public int LanguagesExplored { get; set; }
        public int ContinentsRepresented { get; set; }
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }
}
