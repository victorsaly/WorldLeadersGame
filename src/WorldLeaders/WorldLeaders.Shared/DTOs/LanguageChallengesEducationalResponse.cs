using System.Collections.Generic;

namespace WorldLeaders.Shared.DTOs
{
    public class LanguageChallengesEducationalResponse
    {
        public List<LanguageChallengeDto> Challenges { get; set; } = new();
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }
}
