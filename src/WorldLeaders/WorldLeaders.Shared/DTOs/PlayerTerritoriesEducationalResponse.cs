using System.Collections.Generic;

namespace WorldLeaders.Shared.DTOs
{
    public class PlayerTerritoriesEducationalResponse
    {
        public required List<TerritoryDto> Territories { get; set; }
        public required string EducationalExplanation { get; set; }
        public required string ProgressTip { get; set; }
    }
}
