using System.Collections.Generic;
using WorldLeaders.Shared.Enums;

namespace WorldLeaders.Shared.DTOs
{
    public class TerritoriesByTierEducationalResponse
    {
        public List<TerritoryDto> Territories { get; set; } = new();
        public TerritoryTier Tier { get; set; }
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }
}
