using WorldLeaders.Shared.Services;
namespace WorldLeaders.Shared.DTOs
{
    public class CulturalContextEducationalResponse
    {
    public CulturalContextDto? Context { get; set; }
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }
}
