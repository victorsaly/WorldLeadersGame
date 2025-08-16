using WorldLeaders.Shared.Services;
namespace WorldLeaders.Shared.DTOs
{
    public class TerritoryAcquisitionEducationalResponse
    {
    public TerritoryAcquisitionResult? Result { get; set; }
    public string EducationalExplanation { get; set; } = string.Empty;
    public string ProgressTip { get; set; } = string.Empty;
    }
}
