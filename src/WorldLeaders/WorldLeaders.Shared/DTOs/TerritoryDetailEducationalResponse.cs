using WorldLeaders.Shared.Services;
using System.Diagnostics.CodeAnalysis;
namespace WorldLeaders.Shared.DTOs
{
    public class TerritoryDetailEducationalResponse
    {
    public TerritoryDetailDto? Details { get; set; }
        public string EducationalExplanation { get; set; } = string.Empty;
        public string ProgressTip { get; set; } = string.Empty;
    }
}
