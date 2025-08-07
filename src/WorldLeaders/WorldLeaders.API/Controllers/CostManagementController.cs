using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.API.Controllers;

/// <summary>
/// Context: Educational game Azure cost management for 12-year-old players
/// Educational Objective: Enhanced cost monitoring and forecasting API
/// Safety Requirements: Budget controls, educational compliance, UK South focus
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class CostManagementController(
    IRealTimeCostTracker costTracker,
    IAzureCostManagementClient azureCostClient,
    ILogger<CostManagementController> logger) : ControllerBase
{
    /// <summary>
    /// Get enhanced real-time cost summary for current user
    /// </summary>
    /// <returns>Enhanced cost summary with educational metrics</returns>
    [HttpGet("enhanced-summary")]
    [ProducesResponseType(typeof(EnhancedCostSummaryDto), 200)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> GetEnhancedCostSummary()
    {
        try
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            // Get basic cost summary and enhance it
            var basicSummary = await costTracker.GetDailyCostSummaryAsync(userId);
            
            // Convert to enhanced format with educational metrics
            var enhancedSummary = await costTracker.TrackRealTimeCostAsync(
                userId, "aggregated", 0, new Dictionary<string, object>());

            logger.LogInformation("Enhanced cost summary retrieved for user {UserId}: £{Cost}, Efficiency: {Score}", 
                userId, enhancedSummary.TotalCostGBP, enhancedSummary.AverageEducationalScore);

            return Ok(enhancedSummary);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting enhanced cost summary");
            return StatusCode(500, "An error occurred getting enhanced cost summary.");
        }
    }

    /// <summary>
    /// Get real-time cost data for monitoring dashboard
    /// </summary>
    /// <returns>Real-time cost tracking data</returns>
    [HttpGet("real-time")]
    [ProducesResponseType(typeof(RealTimeCostData), 200)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> GetRealTimeCostData()
    {
        try
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            var realTimeData = await costTracker.GetRealTimeCostDataAsync(userId);
            
            logger.LogInformation("Real-time cost data retrieved for user {UserId}: £{Cost} at {Timestamp}", 
                userId, realTimeData.CostGBP, realTimeData.Timestamp);

            return Ok(realTimeData);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting real-time cost data");
            return StatusCode(500, "An error occurred getting real-time cost data.");
        }
    }

    /// <summary>
    /// Check if user should be throttled due to budget limits
    /// </summary>
    /// <returns>Throttling recommendation with educational context</returns>
    [HttpGet("throttling-check")]
    [ProducesResponseType(typeof(BudgetThrottlingRecommendation), 200)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> GetThrottlingRecommendation()
    {
        try
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            var recommendation = await costTracker.ShouldThrottleUserAsync(userId);
            
            logger.LogInformation("Throttling check for user {UserId}: ShouldThrottle={ShouldThrottle}, Reason={Reason}", 
                userId, recommendation.ShouldThrottle, recommendation.Reason);

            return Ok(recommendation);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking throttling recommendation");
            return StatusCode(500, "An error occurred checking throttling recommendation.");
        }
    }

    /// <summary>
    /// Get cost forecasts for the current user
    /// </summary>
    /// <param name="days">Number of days to forecast (default: 7)</param>
    /// <returns>Cost forecast data with machine learning predictions</returns>
    [HttpGet("forecast")]
    [ProducesResponseType(typeof(List<CostForecastData>), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> GetCostForecasts([FromQuery] int days = 7)
    {
        try
        {
            if (days < 1 || days > 30)
            {
                return BadRequest("Forecast days must be between 1 and 30");
            }

            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            var forecasts = await azureCostClient.GetCostForecastsAsync(userId, days);
            
            logger.LogInformation("Cost forecasts generated for user {UserId}: {ForecastCount} days", 
                userId, forecasts.Count);

            return Ok(forecasts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting cost forecasts");
            return StatusCode(500, "An error occurred getting cost forecasts.");
        }
    }

    /// <summary>
    /// Get educational efficiency score for current user
    /// </summary>
    /// <param name="timeframeDays">Timeframe in days to calculate efficiency (default: 1)</param>
    /// <returns>Educational efficiency score (target: 85+ points/£)</returns>
    [HttpGet("educational-efficiency")]
    [ProducesResponseType(typeof(decimal), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> GetEducationalEfficiency([FromQuery] int timeframeDays = 1)
    {
        try
        {
            if (timeframeDays < 1 || timeframeDays > 30)
            {
                return BadRequest("Timeframe days must be between 1 and 30");
            }

            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            var timeframe = TimeSpan.FromDays(timeframeDays);
            var efficiency = await costTracker.CalculateEducationalEfficiencyScoreAsync(userId, timeframe);
            
            logger.LogInformation("Educational efficiency calculated for user {UserId}: {Efficiency} points/£ over {Days} days", 
                userId, efficiency, timeframeDays);

            return Ok(new { 
                EfficiencyScore = efficiency,
                Target = 85m,
                IsAboveTarget = efficiency >= 85m,
                Timeframe = $"{timeframeDays} days",
                Currency = "GBP"
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error calculating educational efficiency");
            return StatusCode(500, "An error occurred calculating educational efficiency.");
        }
    }

    /// <summary>
    /// Track new cost with educational metrics (for AI agents to call)
    /// </summary>
    /// <param name="request">Cost tracking request with educational context</param>
    /// <returns>Enhanced cost summary</returns>
    [HttpPost("track")]
    [ProducesResponseType(typeof(EnhancedCostSummaryDto), 200)]
    [ProducesResponseType(typeof(string), 400)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> TrackCost([FromBody] TrackCostRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                
                return BadRequest($"Validation failed: {string.Join(", ", errors)}");
            }

            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            var enhancedSummary = await costTracker.TrackRealTimeCostAsync(
                userId, 
                request.ServiceType, 
                request.EstimatedCostGBP, 
                request.EducationalMetrics);

            logger.LogInformation("Cost tracked for user {UserId}: {ServiceType} - £{Cost}, Efficiency: {Score}", 
                userId, request.ServiceType, request.EstimatedCostGBP, enhancedSummary.AverageEducationalScore);

            return Ok(enhancedSummary);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error tracking cost");
            return StatusCode(500, "An error occurred tracking cost.");
        }
    }

    /// <summary>
    /// Get budget alerts for current user (admin endpoint)
    /// </summary>
    /// <returns>Recent budget alerts</returns>
    [HttpGet("alerts")]
    [Authorize(Roles = "Admin,Teacher")]
    [ProducesResponseType(typeof(List<BudgetAlertNotification>), 200)]
    [ProducesResponseType(typeof(string), 401)]
    [ProducesResponseType(typeof(string), 403)]
    [ProducesResponseType(typeof(string), 500)]
    public async Task<IActionResult> GetBudgetAlerts()
    {
        try
        {
            var userIdClaim = User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized("Invalid user session");
            }

            // For now, check current cost and generate alert if needed
            var summary = await costTracker.GetDailyCostSummaryAsync(userId);
            var alert = await costTracker.CheckAndTriggerBudgetAlertAsync(userId, summary.TotalCost);

            var alerts = alert != null ? new List<BudgetAlertNotification> { alert } : new List<BudgetAlertNotification>();
            
            logger.LogInformation("Budget alerts retrieved for user {UserId}: {AlertCount} alerts", 
                userId, alerts.Count);

            return Ok(alerts);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting budget alerts");
            return StatusCode(500, "An error occurred getting budget alerts.");
        }
    }
}

/// <summary>
/// Request model for tracking costs with educational context
/// </summary>
public record TrackCostRequest
{
    /// <summary>
    /// Type of Azure service (AI, Speech, ContentModeration)
    /// </summary>
    public required string ServiceType { get; init; }

    /// <summary>
    /// Estimated cost in GBP
    /// </summary>
    public required decimal EstimatedCostGBP { get; init; }

    /// <summary>
    /// Educational metrics achieved in this interaction
    /// </summary>
    public Dictionary<string, object>? EducationalMetrics { get; init; }
}