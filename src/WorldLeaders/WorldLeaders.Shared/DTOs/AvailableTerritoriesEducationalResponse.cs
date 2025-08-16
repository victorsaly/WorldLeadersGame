using System.Collections.Generic;

namespace WorldLeaders.Shared.DTOs
{
    /// <summary>
    /// Educational response for available territories
    /// </summary>
    public class AvailableTerritoriesEducationalResponse
    {
        public List<TerritoryDto> Territories { get; set; } = new();
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }
}
