using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.API.Controllers;

/// <summary>
/// Child Safety API for the World Leaders educational game
/// Context: Educational game platform for 12-year-old geography and economics learning
/// Safety: Provides dedicated child protection endpoints and validation services
/// Compliance: COPPA, GDPR, UK Educational Standards
/// </summary>
[ApiController]
[Route("api/child-safety")]
[Produces("application/json")]
public class ChildSafetyController : ControllerBase
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ChildSafetyController> _logger;
    private readonly IChildSafetyValidator? _childSafetyValidator;
    private readonly IContentModerationService? _contentModerationService;

    public ChildSafetyController(
        IServiceProvider serviceProvider,
        ILogger<ChildSafetyController> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        
        // Use optional dependency injection to avoid startup failures
        _childSafetyValidator = serviceProvider.GetService<IChildSafetyValidator>();
        _contentModerationService = serviceProvider.GetService<IContentModerationService>();
    }

    /// <summary>
    /// Health check specifically for child safety systems
    /// </summary>
    /// <returns>Child safety health status</returns>
    [HttpGet("health")]
    [ProducesResponseType(typeof(ChildSafetyHealthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ChildSafetyHealthResponse), StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ChildSafetyHealthResponse>> GetHealthStatus()
    {
        try
        {
            var checks = new List<ChildSafetyCheck>();
            var overallHealthy = true;

            // Check content moderation service
            var contentModerationHealthy = _contentModerationService != null;
            checks.Add(new ChildSafetyCheck
            {
                Name = "ContentModeration",
                IsHealthy = contentModerationHealthy,
                Description = contentModerationHealthy 
                    ? "Content moderation service is available and operational"
                    : "Content moderation service is not available",
                Component = "Azure Cognitive Services Content Moderator"
            });

            if (!contentModerationHealthy) overallHealthy = false;

            // Check child safety validator
            var childSafetyValidatorHealthy = _childSafetyValidator != null;
            checks.Add(new ChildSafetyCheck
            {
                Name = "ChildSafetyValidator",
                IsHealthy = childSafetyValidatorHealthy,
                Description = childSafetyValidatorHealthy 
                    ? "Child safety validator is available and operational"
                    : "Child safety validator is not available",
                Component = "Internal Child Safety Validation Service"
            });

            if (!childSafetyValidatorHealthy) overallHealthy = false;

            // Check authentication service (for child user validation)
            var authService = _serviceProvider.GetService<IAuthenticationService>();
            var authServiceHealthy = authService != null;
            checks.Add(new ChildSafetyCheck
            {
                Name = "AuthenticationService",
                IsHealthy = authServiceHealthy,
                Description = authServiceHealthy 
                    ? "Authentication service is available for child user validation"
                    : "Authentication service is not available",
                Component = "JWT Authentication with Child Protection"
            });

            if (!authServiceHealthy) overallHealthy = false;

            // Test content validation if services are available
            if (_contentModerationService != null && _childSafetyValidator != null)
            {
                try
                {
                    // Test with safe, educational content
                    var testContent = "Learning about geography is fun and educational!";
                    var testResult = await _contentModerationService.IsContentSafeAsync(testContent);
                    
                    checks.Add(new ChildSafetyCheck
                    {
                        Name = "ContentValidationTest",
                        IsHealthy = testResult,
                        Description = testResult 
                            ? "Content validation is working correctly with test content"
                            : "Content validation test failed",
                        Component = "Integrated Content Validation Pipeline"
                    });

                    if (!testResult) overallHealthy = false;
                }
                catch (Exception ex)
                {
                    checks.Add(new ChildSafetyCheck
                    {
                        Name = "ContentValidationTest",
                        IsHealthy = false,
                        Description = $"Content validation test failed with error: {ex.Message}",
                        Component = "Integrated Content Validation Pipeline"
                    });
                    overallHealthy = false;
                }
            }

            var response = new ChildSafetyHealthResponse
            {
                Status = overallHealthy ? "Healthy" : "Unhealthy",
                Timestamp = DateTime.UtcNow,
                TargetAge = "12 years",
                ComplianceFrameworks = new[] { "COPPA", "GDPR", "UK Educational Standards" },
                ChildSafetyMode = true,
                OverallScore = (double)checks.Count(c => c.IsHealthy) / checks.Count,
                Checks = checks,
                TotalChecks = checks.Count,
                HealthyChecks = checks.Count(c => c.IsHealthy),
                CriticalSystemsOperational = overallHealthy,
                Message = overallHealthy 
                    ? "All child safety systems are operational and protecting 12-year-old learners"
                    : "Some child safety systems require attention - educational content protection may be compromised"
            };

            var statusCode = overallHealthy ? StatusCodes.Status200OK : StatusCodes.Status503ServiceUnavailable;

            _logger.LogInformation("Child safety health check completed: {Status} ({HealthyChecks}/{TotalChecks} systems healthy)", 
                response.Status, response.HealthyChecks, response.TotalChecks);

            return StatusCode(statusCode, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Child safety health check failed with exception");
            
            var errorResponse = new ChildSafetyHealthResponse
            {
                Status = "Critical",
                Timestamp = DateTime.UtcNow,
                TargetAge = "12 years",
                ComplianceFrameworks = new[] { "COPPA", "GDPR", "UK Educational Standards" },
                ChildSafetyMode = true,
                OverallScore = 0.0,
                Checks = new List<ChildSafetyCheck>
                {
                    new ChildSafetyCheck
                    {
                        Name = "SystemCheck",
                        IsHealthy = false,
                        Description = $"Critical system error: {ex.Message}",
                        Component = "Child Safety Health Check System"
                    }
                },
                TotalChecks = 1,
                HealthyChecks = 0,
                CriticalSystemsOperational = false,
                Message = "Critical child safety system failure - immediate attention required"
            };

            return StatusCode(StatusCodes.Status503ServiceUnavailable, errorResponse);
        }
    }

    /// <summary>
    /// Validate content for child safety compliance
    /// </summary>
    /// <param name="request">Content to validate</param>
    /// <returns>Validation result</returns>
    [HttpPost("validate")]
    [ProducesResponseType(typeof(ChildSafetyContentValidationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<ActionResult<ChildSafetyContentValidationResponse>> ValidateContent([FromBody] ChildSafetyContentValidationRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Content))
        {
            return BadRequest("Content cannot be empty");
        }

        if (_contentModerationService == null)
        {
            _logger.LogWarning("Content moderation service not available for validation request");
            return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                "Content moderation service is not available");
        }

        try
        {
            var validationResult = await _contentModerationService.ValidateContentAsync(request.Content, "Child Safety Validation");
            
            var response = new ChildSafetyContentValidationResponse
            {
                IsValid = validationResult.IsApproved && validationResult.IsSafe && validationResult.IsAgeAppropriate,
                Content = request.Content,
                Timestamp = DateTime.UtcNow,
                TargetAge = "12 years",
                ValidationRules = new[] 
                { 
                    "No inappropriate language",
                    "Age-appropriate content",
                    "Educational value",
                    "Cultural sensitivity",
                    "Child-friendly messaging"
                },
                Message = validationResult.IsApproved 
                    ? "Content is appropriate for 12-year-old learners"
                    : $"Content does not meet child safety standards: {validationResult.Reason}"
            };

            _logger.LogInformation("Content validation completed: {IsValid} for content length {Length} - Reason: {Reason}", 
                response.IsValid, request.Content.Length, validationResult.Reason);

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Content validation failed for content length {Length}", request.Content.Length);
            
            return StatusCode(StatusCodes.Status503ServiceUnavailable, 
                "Content validation service encountered an error");
        }
    }
}

/// <summary>
/// Child safety health check response
/// </summary>
public sealed record ChildSafetyHealthResponse
{
    public string Status { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string TargetAge { get; init; } = string.Empty;
    public string[] ComplianceFrameworks { get; init; } = Array.Empty<string>();
    public bool ChildSafetyMode { get; init; }
    public double OverallScore { get; init; }
    public List<ChildSafetyCheck> Checks { get; init; } = new();
    public int TotalChecks { get; init; }
    public int HealthyChecks { get; init; }
    public bool CriticalSystemsOperational { get; init; }
    public string Message { get; init; } = string.Empty;
}

/// <summary>
/// Individual child safety check result
/// </summary>
public sealed record ChildSafetyCheck
{
    public string Name { get; init; } = string.Empty;
    public bool IsHealthy { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Component { get; init; } = string.Empty;
}

/// <summary>
/// Child safety content validation request
/// </summary>
public sealed record ChildSafetyContentValidationRequest
{
    public string Content { get; init; } = string.Empty;
}

/// <summary>
/// Child safety content validation response
/// </summary>
public sealed record ChildSafetyContentValidationResponse
{
    public bool IsValid { get; init; }
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; }
    public string TargetAge { get; init; } = string.Empty;
    public string[] ValidationRules { get; init; } = Array.Empty<string>();
    public string Message { get; init; } = string.Empty;
}
