using System.Collections.Generic;

namespace WorldLeaders.Shared.DTOs
{
    public class GameEventsEducationalResponse
    {
        public List<GameEventDto> Events { get; set; } = new();
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }
}
