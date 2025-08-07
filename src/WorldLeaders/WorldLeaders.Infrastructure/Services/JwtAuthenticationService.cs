using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Shared.DTOs;
using WorldLeaders.Shared.Enums;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Services;

/// <summary>
/// Context: Educational game authentication for 12-year-old players
/// Educational Objective: Secure, COPPA/GDPR compliant JWT authentication
/// Safety Requirements: Child data protection, UK South data residency, enhanced session management
/// Enhanced .NET 8 Implementation with primary constructors for streamlined security service initialization
/// </summary>
public class JwtAuthenticationService(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IChildSafetyValidator childSafetyValidator,
    IPerUserCostTracker costTracker,
    IOptions<JwtOptions> jwtOptions,
    IOptions<ChildSafetyOptions> childSafetyOptions,
    WorldLeadersDbContext dbContext,
    ILogger<JwtAuthenticationService> logger) : IAuthenticationService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;
    private readonly ChildSafetyOptions _childSafetyOptions = childSafetyOptions.Value;

    /// <summary>
    /// Azure AD B2C configuration for UK educational deployment
    /// </summary>
    public required AzureAdB2COptions B2CConfig { get; init; }

    /// <summary>
    /// Azure region for data residency compliance
    /// </summary>
    public required string Region { get; init; } = "UK South";

    /// <summary>
    /// Maximum cost per user per day in GBP
    /// </summary>
    public required decimal MaxCostPerUser { get; init; } = 0.08m;

    /// <summary>
    /// Register a new user with comprehensive child safety validation
    /// </summary>
    public async Task<AuthenticationResponse> RegisterAsync(RegisterUserRequest request)
    {
        logger.LogInformation("Starting user registration for username: {Username}", request.Username);

        try
        {
            // Validate child safety compliance first
            var safetyValidation = await childSafetyValidator.ValidateRegistrationAsync(request);
            if (!safetyValidation.IsApproved)
            {
                logger.LogWarning("Registration failed safety validation: {Reason}", safetyValidation.Reason);
                throw new InvalidOperationException($"Registration failed safety validation: {safetyValidation.Reason}");
            }

            // Check if user already exists
            var existingUser = await userManager.FindByNameAsync(request.Username)
                ?? await userManager.FindByEmailAsync(request.Email);

            if (existingUser != null)
            {
                throw new InvalidOperationException("User with this username or email already exists");
            }

            // Create the application user
            var user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                UserName = request.Username,
                Email = request.Email,
                EmailConfirmed = true, // For educational use, skip email confirmation
                DisplayName = request.DisplayName,
                DateOfBirth = request.DateOfBirth,
                Role = request.Role,
                ParentalEmail = request.ParentalEmail,
                SchoolName = request.SchoolName,
                HasGdprConsent = request.HasGdprConsent,
                GdprConsentDate = request.HasGdprConsent ? DateTime.UtcNow : null,
                HasParentalConsent = request.HasParentalConsent,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Create the user account
            var result = await userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogError("User creation failed: {Errors}", errors);
                throw new InvalidOperationException($"User creation failed: {errors}");
            }

            // Log safety event for child account creation
            if (user.IsChild)
            {
                await childSafetyValidator.LogSafetyEventAsync(new ChildSafetyAudit
                {
                    UserId = user.Id,
                    EventType = SafetyEventType.ParentalConsent,
                    Description = "Child account created with parental consent validation",
                    Severity = SafetyEventSeverity.Info,
                    ActionTaken = true,
                    ActionDescription = "Account created successfully with safety validation"
                });
            }

            logger.LogInformation("User registered successfully: {UserId}", user.Id);

            // Generate authentication response
            return await GenerateAuthenticationResponseAsync(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during user registration for username: {Username}", request.Username);
            throw;
        }
    }

    /// <summary>
    /// Authenticate user with enhanced security for children
    /// </summary>
    public async Task<AuthenticationResponse> LoginAsync(LoginRequest request)
    {
        logger.LogInformation("Login attempt for user: {UsernameOrEmail}", request.UsernameOrEmail);

        try
        {
            // Find user by username or email
            var user = await userManager.FindByNameAsync(request.UsernameOrEmail)
                ?? await userManager.FindByEmailAsync(request.UsernameOrEmail);

            if (user == null)
            {
                logger.LogWarning("Login failed: User not found - {UsernameOrEmail}", request.UsernameOrEmail);
                throw new UnauthorizedAccessException("Invalid username/email or password");
            }

            // Check if account is active
            if (!user.IsActive)
            {
                logger.LogWarning("Login failed: Account inactive - {UserId}", user.Id);
                throw new UnauthorizedAccessException("Account is inactive");
            }

            // Verify password
            var signInResult = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
            if (!signInResult.Succeeded)
            {
                logger.LogWarning("Login failed: Invalid password - {UserId}", user.Id);

                if (signInResult.IsLockedOut)
                {
                    throw new UnauthorizedAccessException("Account is locked due to multiple failed attempts");
                }

                throw new UnauthorizedAccessException("Invalid username/email or password");
            }

            // Check daily cost limit for user
            if (await costTracker.IsOverDailyLimitAsync(user.Id))
            {
                logger.LogWarning("Login blocked: Daily cost limit exceeded - {UserId}", user.Id);
                throw new UnauthorizedAccessException("Daily usage limit exceeded. Please try again tomorrow.");
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            logger.LogInformation("User logged in successfully: {UserId}", user.Id);

            // Generate authentication response
            return await GenerateAuthenticationResponseAsync(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during login for user: {UsernameOrEmail}", request.UsernameOrEmail);
            throw;
        }
    }

    /// <summary>
    /// Create a temporary guest session for exploring the system without registration
    /// </summary>
    public Task<GuestAuthenticationResponse> CreateGuestSessionAsync(GuestAccessRequest request)
    {
        try
        {
            logger.LogInformation("Creating guest session for display name: {DisplayName}, age: {Age}", 
                request.DisplayName ?? "Anonymous", request.Age);

            // Generate a unique guest ID
            var guestId = Guid.NewGuid();
            var now = DateTime.UtcNow;
            
            // Determine session duration (capped at 30 minutes for safety)
            var sessionDuration = Math.Min(request.SessionDurationMinutes, 30);
            var expiresAt = now.AddMinutes(sessionDuration);

            // Create guest session claims
            var claims = new List<Claim>
            {
                new("sub", guestId.ToString()),
                new("jti", Guid.NewGuid().ToString()),
                new(ClaimTypes.Role, UserRole.Guest.ToString()),
                new("isGuest", "true"),
                new("iat", new DateTimeOffset(now).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            // Add child safety claims
            claims.Add(new("isChild", "true"));
            claims.Add(new("requiresChildSafety", "true"));

            if (request.Age.HasValue)
            {
                claims.Add(new("age", request.Age.Value.ToString()));
            }

            if (!string.IsNullOrEmpty(request.DisplayName))
            {
                claims.Add(new("displayName", request.DisplayName));
            }

            // Generate JWT token for guest session
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expiresAt,
                Issuer = _jwtOptions.Issuer,
                Audience = _jwtOptions.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Create guest user info
            var guestUserInfo = new GuestUserInfoDto
            {
                GuestId = guestId,
                DisplayName = request.DisplayName ?? "Guest Explorer",
                Age = request.Age,
                SessionStartedAt = now,
                SessionExpiresAt = expiresAt,
                HasParentalAwareness = request.HasParentalAwareness
            };

            // Create guest authentication response
            var response = new GuestAuthenticationResponse
            {
                AccessToken = tokenString,
                ExpiresAt = expiresAt,
                Guest = guestUserInfo,
                SessionTimeoutMinutes = sessionDuration,
                AvailableFeatures = new List<string>
                {
                    "BasicGameplay",
                    "CountryExploration", 
                    "LanguageLearning",
                    "SafeAIChat"
                },
                RegistrationEncouragement = "Create an account to save your progress and unlock more features!"
            };

            logger.LogInformation("Guest session created successfully: {GuestId}, expires: {ExpiresAt}", 
                guestId, expiresAt);

            return Task.FromResult(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating guest session");
            throw;
        }
    }

    /// <summary>
    /// Validate JWT token and return user information
    /// </summary>
    public async Task<UserInfoDto?> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = _jwtOptions.ValidateIssuer,
                ValidIssuer = _jwtOptions.Issuer,
                ValidateAudience = _jwtOptions.ValidateAudience,
                ValidAudience = _jwtOptions.Audience,
                ValidateLifetime = _jwtOptions.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(_jwtOptions.ClockSkewMinutes)
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);

            if (validatedToken is not JwtSecurityToken jwtToken ||
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return null;
            }

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null || !user.IsActive)
            {
                return null;
            }

            return new UserInfoDto
            {
                Id = user.Id,
                Username = user.UserName!,
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Role = user.Role,
                IsChild = user.IsChild,
                Age = user.CalculateAge(),
                IsActive = user.IsActive,
                PlayerId = user.PlayerId,
                LastLoginAt = user.LastLoginAt
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error validating token");
            return null;
        }
    }

    /// <summary>
    /// Refresh an existing token if still valid
    /// </summary>
    public async Task<AuthenticationResponse> RefreshTokenAsync(string token)
    {
        var userInfo = await ValidateTokenAsync(token);
        if (userInfo == null)
        {
            throw new UnauthorizedAccessException("Invalid or expired token");
        }

        var user = await userManager.FindByIdAsync(userInfo.Id.ToString());
        if (user == null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        return await GenerateAuthenticationResponseAsync(user);
    }

    /// <summary>
    /// Logout user and invalidate session
    /// </summary>
    public async Task<bool> LogoutAsync(Guid userId, string sessionToken)
    {
        try
        {
            // Find and invalidate the session
            var session = await dbContext.UserSessions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.SessionToken == sessionToken && s.IsActive);

            if (session != null)
            {
                session.IsActive = false;
                await dbContext.SaveChangesAsync();
            }

            logger.LogInformation("User logged out: {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error during logout for user: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// Change user password with validation
    /// </summary>
    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordRequest request)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return false;
            }

            var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                logger.LogInformation("Password changed successfully for user: {UserId}", userId);
                return true;
            }

            logger.LogWarning("Password change failed for user: {UserId}. Errors: {Errors}",
                userId, string.Join(", ", result.Errors.Select(e => e.Description)));
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error changing password for user: {UserId}", userId);
            return false;
        }
    }

    /// <summary>
    /// Get current session information
    /// </summary>
    public async Task<SessionInfoDto?> GetSessionInfoAsync(Guid userId, string sessionToken)
    {
        try
        {
            var session = await dbContext.UserSessions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.SessionToken == sessionToken && s.IsActive);

            if (session == null)
            {
                return null;
            }

            var remainingTime = session.ExpiresAt - DateTime.UtcNow;
            var warningThreshold = TimeSpan.FromMinutes(_childSafetyOptions.SessionWarningMinutes);

            return new SessionInfoDto
            {
                SessionId = session.Id,
                StartedAt = session.StartedAt,
                ExpiresAt = session.ExpiresAt,
                LastActivityAt = session.LastActivityAt,
                RemainingTime = remainingTime > TimeSpan.Zero ? remainingTime : TimeSpan.Zero,
                IsNearExpiration = remainingTime <= warningThreshold
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting session info for user: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// Extend session if allowed (typically not for child accounts)
    /// </summary>
    public async Task<DateTime?> ExtendSessionAsync(Guid userId, string sessionToken)
    {
        try
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return null;
            }

            // Don't allow session extension for child accounts for safety
            if (user.IsChild)
            {
                logger.LogInformation("Session extension denied for child account: {UserId}", userId);
                return null;
            }

            var session = await dbContext.UserSessions
                .FirstOrDefaultAsync(s => s.UserId == userId && s.SessionToken == sessionToken && s.IsActive);

            if (session == null)
            {
                return null;
            }

            // Extend session by the original timeout duration
            var extensionMinutes = user.RequiresChildSafety
                ? _childSafetyOptions.ChildSessionTimeoutMinutes
                : _childSafetyOptions.AdultSessionTimeoutMinutes;

            session.ExpiresAt = DateTime.UtcNow.AddMinutes(extensionMinutes);
            session.LastActivityAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Session extended for user: {UserId} until {ExpiresAt}", userId, session.ExpiresAt);
            return session.ExpiresAt;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error extending session for user: {UserId}", userId);
            return null;
        }
    }

    /// <summary>
    /// Generate JWT token and authentication response
    /// </summary>
    private async Task<AuthenticationResponse> GenerateAuthenticationResponseAsync(ApplicationUser user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtOptions.SecretKey);

        // Determine session timeout based on user type
        var sessionTimeoutMinutes = user.RequiresChildSafety
            ? _childSafetyOptions.ChildSessionTimeoutMinutes
            : _childSafetyOptions.AdultSessionTimeoutMinutes;

        var expiresAt = DateTime.UtcNow.AddMinutes(sessionTimeoutMinutes);
        var jwtId = Guid.NewGuid().ToString();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("DisplayName", user.DisplayName),
                new Claim("IsChild", user.IsChild.ToString()),
                new Claim("Age", user.CalculateAge().ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, jwtId),
                new Claim(JwtRegisteredClaimNames.Iat,
                    new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString(),
                    ClaimValueTypes.Integer64)
            }),
            Expires = expiresAt,
            Issuer = _jwtOptions.Issuer,
            Audience = _jwtOptions.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Create session record
        var session = new UserSession
        {
            UserId = user.Id,
            SessionToken = jwtId,
            StartedAt = DateTime.UtcNow,
            ExpiresAt = expiresAt,
            LastActivityAt = DateTime.UtcNow,
            IsActive = true
        };

        dbContext.UserSessions.Add(session);
        await dbContext.SaveChangesAsync();

        return new AuthenticationResponse
        {
            AccessToken = tokenString,
            ExpiresAt = expiresAt,
            User = new UserInfoDto
            {
                Id = user.Id,
                Username = user.UserName!,
                DisplayName = user.DisplayName,
                Email = user.Email!,
                Role = user.Role,
                IsChild = user.IsChild,
                Age = user.CalculateAge(),
                IsActive = user.IsActive,
                PlayerId = user.PlayerId,
                LastLoginAt = user.LastLoginAt
            },
            ChildSafetyEnabled = user.RequiresChildSafety,
            SessionTimeoutMinutes = sessionTimeoutMinutes
        };
    }
}