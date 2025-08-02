# Phase 1 Implementation Guide for World Leaders Game

## 🎯 Phase 1 Overview

Phase 1 establishes the foundational .NET Aspire solution structure for the World Leaders Game educational project. This phase creates the complete development environment following AI-first development methodology with comprehensive GitHub Copilot integration.

## 📋 Phase 1 Task Breakdown

### Task 1.1: Solution Structure Setup
**Objective**: Create the complete .NET Aspire solution with proper project architecture

**Requirements**:
- Create `src/` directory structure
- Generate .NET Aspire solution: `dotnet new aspire -n WorldLeaders`
- Establish 5 core projects with proper naming conventions
- Configure solution-level dependencies and references

**Expected Output**:
```
src/
├── WorldLeaders.AppHost/           # .NET Aspire orchestration
├── WorldLeaders.Web/               # Blazor Server application
├── WorldLeaders.API/               # Game API services  
├── WorldLeaders.Shared/            # Shared models and contracts
└── WorldLeaders.Infrastructure/    # Data access and external services
```

### Task 1.2: NuGet Package Installation
**Objective**: Install all required packages across the 5 projects for educational game functionality

**Package Requirements by Project**:

**WorldLeaders.AppHost**:
- `Aspire.Hosting.PostgreSQL` - Database orchestration
- `Aspire.Hosting.Redis` - Caching service orchestration

**WorldLeaders.Web**:
- `Microsoft.AspNetCore.SignalR.Client` - Real-time communication
- `Microsoft.AspNetCore.Components.Web` - Blazor Server components
- TailwindCSS integration packages for child-friendly design

**WorldLeaders.API**:
- `Microsoft.AspNetCore.SignalR` - SignalR hub services
- `Swashbuckle.AspNetCore` - API documentation
- `Microsoft.AspNetCore.Authentication.JwtBearer` - Security

**WorldLeaders.Infrastructure**:
- `Microsoft.EntityFrameworkCore` - ORM framework
- `Npgsql.EntityFrameworkCore.PostgreSQL` - PostgreSQL provider
- `Azure.AI.OpenAI` - AI agent services
- `Microsoft.CognitiveServices.Speech` - Speech recognition
- `Microsoft.Extensions.Http` - HTTP client factory

**WorldLeaders.Shared**:
- `System.ComponentModel.DataAnnotations` - Validation attributes
- `Newtonsoft.Json` - JSON serialization

### Task 1.3: Core Domain Models
**Objective**: Implement essential domain models for educational game functionality

**Models to Create**:

**Player Model** (`WorldLeaders.Shared/Models/Player.cs`):
```csharp
public class Player
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public int Income { get; set; }
    public int Reputation { get; set; }      // 0-100%
    public int Happiness { get; set; }       // 0-100%
    public JobLevel CurrentJob { get; set; }
    public List<Territory> OwnedTerritories { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

**Territory Model** (`WorldLeaders.Shared/Models/Territory.cs`):
```csharp
public class Territory
{
    public Guid Id { get; set; }
    public string CountryName { get; set; }
    public string CountryCode { get; set; }
    public decimal GdpInBillions { get; set; }
    public int Cost { get; set; }
    public int ReputationRequired { get; set; }
    public List<string> OfficialLanguages { get; set; }
}
```

**Enumerations** (`WorldLeaders.Shared/Enums/`):
```csharp
public enum JobLevel
{
    Farmer = 1,
    Gardener = 2,
    Shopkeeper = 3,
    Artisan = 4,
    Politician = 5,
    BusinessLeader = 6
}

public enum AgentType
{
    CareerGuide,
    EventNarrator,
    FortuneTeller,
    HappinessAdvisor,
    TerritoryStrategist,
    LanguageTutor
}
```

### Task 1.4: Entity Framework Configuration
**Objective**: Set up Entity Framework Core with PostgreSQL for educational game data persistence

**Database Setup Requirements**:
- Create `GameDbContext` in `WorldLeaders.Infrastructure/Data/`
- Configure entity relationships and constraints
- Implement audit fields (CreatedAt, UpdatedAt) for all entities
- Set up soft delete patterns with IsDeleted flag
- Create initial migration with territory seed data

**Key Configurations**:
- Guid primary keys for all entities
- JSON column support for complex game state
- Territory seed data with real GDP information
- Connection string configuration for development/production

### Task 1.5: .NET Aspire Orchestration
**Objective**: Configure .NET Aspire in AppHost for local development orchestration

**Orchestration Requirements**:
- Register PostgreSQL database service
- Set up Redis for session state and caching
- Configure Web application (Blazor Server)
- Register API application with proper service discovery
- Health checks for all services
- Development environment optimization

## 🎮 Educational Game Considerations

### Child Safety Requirements
- All code must be appropriate for 12-year-old players
- Input validation and sanitization for user-generated content
- Content moderation preparation for AI responses
- Privacy-first data handling patterns

### Educational Objectives
- Territory pricing based on real-world GDP data
- Language learning integration preparation
- Cultural sensitivity in country representation
- Learning progression tracking capabilities

### Performance Targets
- Page load times under 2 seconds
- API response times under 500ms
- Real-time SignalR communication optimization
- Mobile-responsive design for tablet usage

## 🤖 AI-First Development Approach

### GitHub Copilot Integration
All Phase 1 tasks should be completed using GitHub Copilot Chat with comprehensive context from:
- `.github/copilot-instructions.md` - Complete development guidelines
- Domain model specifications above
- Educational game design patterns
- Child safety and performance requirements

### Prompt-Driven Development
Each task includes specific prompts designed for GitHub Copilot Chat to generate contextually appropriate educational game code following established patterns.

### Quality Assurance
- Solution must compile without errors
- All projects must follow naming conventions
- Educational patterns must be consistently applied
- Child safety considerations must be implemented

## ✅ Phase 1 Completion Criteria

**Technical Completion**:
- [ ] Complete .NET Aspire solution structure created
- [ ] All required NuGet packages installed and compatible
- [ ] Core domain models implemented with proper relationships
- [ ] Entity Framework configured with PostgreSQL
- [ ] .NET Aspire orchestration running locally
- [ ] Solution compiles and runs without errors

**Educational Readiness**:
- [ ] Foundation ready for AI agent integration
- [ ] Database prepared for real-world territory data
- [ ] Child-safe development patterns established
- [ ] Performance optimization baselines met

**Documentation**:
- [ ] All code properly documented with XML comments
- [ ] README updated with setup instructions
- [ ] Development environment documented
- [ ] Phase 2 preparation notes completed

## 🚀 Next Phase Preparation

Phase 1 completion prepares the foundation for Phase 2: Core Game Engine development, including:
- 6-phase game loop implementation
- Random event card system
- Resource management mechanics
- AI agent personality framework
- Real-time game state synchronization

The solid foundation established in Phase 1 enables rapid Phase 2 development with continued AI-first methodology.
