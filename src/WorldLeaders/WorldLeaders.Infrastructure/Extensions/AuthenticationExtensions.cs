using System.Text;
using Azure.Identity;
using Azure.ResourceManager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using WorldLeaders.Infrastructure.Configuration;
using WorldLeaders.Infrastructure.Data;
using WorldLeaders.Infrastructure.Services;
using WorldLeaders.Shared.Models;
using WorldLeaders.Shared.Services;

namespace WorldLeaders.Infrastructure.Extensions;

/// <summary>
/// Authentication and security extensions for dependency injection
/// Context: Educational game authentication for 12-year-old players
/// Safety Requirements: JWT authentication, Azure AD B2C, child safety validation, UK South compliance
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Add comprehensive authentication services for educational platform
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddEducationalAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure authentication options
        services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));
        services.Configure<AzureAdB2COptions>(configuration.GetSection(AzureAdB2COptions.SectionName));
        services.Configure<ChildSafetyOptions>(configuration.GetSection(ChildSafetyOptions.SectionName));
        services.Configure<CostTrackingOptions>(configuration.GetSection(CostTrackingOptions.SectionName));

        // Add Identity services with enhanced configuration for children
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
        {
            // Password requirements - balanced security for educational use
            options.Password.RequireDigit = true;
            options.Password.RequiredLength = 8;
            options.Password.RequireNonAlphanumeric = false; // Simplified for children
            options.Password.RequireUppercase = true;
            options.Password.RequireLowercase = true;
            options.Password.RequiredUniqueChars = 3;

            // User requirements
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";

            // Sign in requirements
            options.SignIn.RequireConfirmedEmail = false; // Simplified for educational use
            options.SignIn.RequireConfirmedPhoneNumber = false;

            // Lockout settings - protective but not too restrictive for children
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
        .AddEntityFrameworkStores<WorldLeadersDbContext>()
        .AddDefaultTokenProviders();

        // Configure JWT authentication
        var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>();
        if (jwtOptions == null)
        {
            throw new InvalidOperationException("JWT configuration is missing. Please configure the Jwt section in appsettings.json");
        }

        // Generate a development secret key if not provided (for development only)
        var secretKey = jwtOptions.SecretKey;
        if (string.IsNullOrEmpty(secretKey))
        {
            var environment = configuration["ASPNETCORE_ENVIRONMENT"] ?? "Development";
            if (environment == "Development")
            {
                // Generate a secure 256-bit key for development
                secretKey = "WorldLeaders-Educational-Game-Development-Secret-Key-2025-Very-Long-String-For-256-Bit-Security-Child-Safe-Learning-Platform";
            }
            else
            {
                throw new InvalidOperationException("JWT SecretKey is required in production environments. Please configure Jwt:SecretKey in appsettings.json");
            }
        }

        var key = Encoding.ASCII.GetBytes(secretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true; // Always require HTTPS in production
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = jwtOptions.ValidateIssuer,
                ValidIssuer = jwtOptions.Issuer,
                ValidateAudience = jwtOptions.ValidateAudience,
                ValidAudience = jwtOptions.Audience,
                ValidateLifetime = jwtOptions.ValidateLifetime,
                ClockSkew = TimeSpan.FromMinutes(jwtOptions.ClockSkewMinutes),
                RequireExpirationTime = true,
                RequireSignedTokens = true
            };

            // Enhanced token validation for child safety
            options.Events = new JwtBearerEvents
            {
                OnTokenValidated = async context =>
                {
                    // Additional validation for child accounts
                    var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
                    var costTracker = context.HttpContext.RequestServices.GetRequiredService<IPerUserCostTracker>();
                    
                    var userIdClaim = context.Principal?.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                    if (Guid.TryParse(userIdClaim, out var userId))
                    {
                        var user = await userManager.FindByIdAsync(userId.ToString());
                        if (user != null)
                        {
                            // Check if user is still active
                            if (!user.IsActive)
                            {
                                context.Fail("User account is inactive");
                                return;
                            }

                            // Check daily cost limit
                            if (await costTracker.IsOverDailyLimitAsync(userId))
                            {
                                context.Fail("Daily usage limit exceeded");
                                return;
                            }

                            // Update last activity
                            user.LastLoginAt = DateTime.UtcNow;
                            await userManager.UpdateAsync(user);
                        }
                    }
                },
                OnChallenge = context =>
                {
                    // Custom challenge response for educational context
                    context.HandleResponse();
                    context.Response.StatusCode = 401;
                    context.Response.ContentType = "application/json";
                    
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        error = "Authentication required",
                        message = "Please log in to access the educational game",
                        isEducational = true
                    });
                    
                    return context.Response.WriteAsync(result);
                },
                OnForbidden = context =>
                {
                    // Custom forbidden response for educational context
                    context.Response.StatusCode = 403;
                    context.Response.ContentType = "application/json";
                    
                    var result = System.Text.Json.JsonSerializer.Serialize(new
                    {
                        error = "Access forbidden",
                        message = "You don't have permission to access this educational resource",
                        isEducational = true
                    });
                    
                    return context.Response.WriteAsync(result);
                }
            };
        });

        // Add authorization policies for educational roles
        services.AddAuthorization(options =>
        {
            // Student policy - default for educational users
            options.AddPolicy("StudentPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Student");
            });

            // Teacher policy - can oversee students
            options.AddPolicy("TeacherPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Teacher", "Admin");
            });

            // Admin policy - full system access
            options.AddPolicy("AdminPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireRole("Admin");
            });

            // Child safety policy - enhanced protection for children
            options.AddPolicy("ChildSafetyPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim("IsChild", "true");
            });

            // Adult supervision policy - for sensitive operations
            options.AddPolicy("AdultSupervisionPolicy", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireAssertion(context =>
                {
                    var isChild = context.User.FindFirst("IsChild")?.Value == "true";
                    var role = context.User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
                    
                    // Allow if not a child, or if teacher/admin
                    return !isChild || role == "Teacher" || role == "Admin";
                });
            });
        });

        // Register authentication services
        services.AddScoped<IAuthenticationService, JwtAuthenticationService>();
        services.AddScoped<IChildSafetyValidator, ChildSafetyValidator>();
        
        // Register child data protection dependencies with LiteDB persistence
        services.AddScoped<IKeyVaultClient, LiteDbKeyVaultClient>();
        services.AddScoped<IAuditLogger, LiteDbAuditLogger>();
        services.AddScoped<IComplianceValidator, UkEducationalComplianceValidator>();
        services.AddScoped<IChildDataProtectionService, ChildDataProtectionService>();
        
        // Add enhanced cost tracking services
        services.AddScoped<IPerUserCostTracker, PerUserCostTracker>();
        services.AddScoped<IRealTimeCostTracker, PerUserCostTracker>();
        
        // Add Azure Cost Management integration
        services.AddScoped<IAzureCostManagementClient, AzureCostManagementClient>();
        
        // Add Azure Resource Manager client for cost management
        services.AddSingleton<ArmClient>(provider =>
        {
            // For MVP, create a default ArmClient with default credentials
            // In production, this would use Azure CLI or managed identity
            return new ArmClient(new DefaultAzureCredential());
        });
        
        // Configure cost management options
        services.Configure<CostTrackingOptions>(configuration.GetSection(CostTrackingOptions.SectionName));
        services.Configure<BudgetConfig>(configuration.GetSection("BudgetConfig"));
        services.Configure<AzureCostManagementConfig>(configuration.GetSection(AzureCostManagementConfig.SectionName));

        // Register user management service
        services.AddScoped<IUserManagerService, UserManagerService>();

        return services;
    }

    /// <summary>
    /// Add Azure AD B2C authentication (optional for advanced deployments)
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddAzureAdB2CAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var b2cOptions = configuration.GetSection(AzureAdB2COptions.SectionName).Get<AzureAdB2COptions>();
        
        if (b2cOptions?.Enabled == true)
        {
            // For now, just add the configuration options
            // B2C integration can be added later if needed
            services.Configure<AzureAdB2COptions>(configuration.GetSection(AzureAdB2COptions.SectionName));
        }

        return services;
    }

    /// <summary>
    /// Configure session management for child safety
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>Service collection for chaining</returns>
    public static IServiceCollection AddChildSafeSessionManagement(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var childSafetyOptions = configuration.GetSection(ChildSafetyOptions.SectionName).Get<ChildSafetyOptions>();
        
        if (childSafetyOptions?.Enabled == true)
        {
            // Configure session state for additional safety tracking
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(childSafetyOptions.ChildSessionTimeoutMinutes);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.Always;
                options.Cookie.SameSite = Microsoft.AspNetCore.Http.SameSiteMode.Strict;
                options.Cookie.Name = "WorldLeaders.Session";
            });

            // Add background service for session cleanup
            services.AddHostedService<SessionCleanupService>();
        }

        return services;
    }
}