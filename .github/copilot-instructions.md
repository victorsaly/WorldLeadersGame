# GitHub Copilot Instructions for World Leaders Game

## 🎯 Project Overview

This is an educational strategy game called "World Leaders Game" designed for 12-year-old players. The game combines strategic thinking, language learning, and real-world geography/economics education. Players progress from peasant to world leader by managing resources, acquiring territories, and learning languages with AI assistance.

## 🏗️ Architecture & Technology Stack

### Core Technologies

- **.NET 8** with **ASP.NET Core**
- **.NET Aspire** for orchestration and service discovery
- **Blazor Server** for interactive web UI
- **TailwindCSS** for styling with child-friendly design
- **Entity Framework Core** with **PostgreSQL**
- **SignalR** for real-time updates

### AI & External Services

- **Azure OpenAI Service** (GPT-4) for AI agents
- **Azure Speech Services** for speech-to-text and pronunciation assessment
- **Azure Cognitive Services** for content moderation
- **World Bank API** for real GDP data
- **REST Countries API** for country information

### Project Structure

```
src/
├── WorldLeaders.AppHost/           # .NET Aspire orchestration
├── WorldLeaders.Web/               # Blazor Server application
│   ├── Components/                    # Blazor components
│   │   ├── Game/                      # Game-specific components
│   │   ├── Shared/                    # Shared UI components
│   │   └── Layout/                    # Layout components
│   ├── Pages/                         # Blazor pages
│   ├── Services/                      # Client-side services
│   └── wwwroot/                       # Static assets
├── WorldLeaders.API/               # Game API services
│   ├── Controllers/                   # API controllers
│   ├── Hubs/                         # SignalR hubs
│   └── Services/                     # Business logic services
├── WorldLeaders.Shared/            # Shared models and contracts
│   ├── Models/                       # Domain models
│   ├── DTOs/                         # Data transfer objects
│   ├── Enums/                        # Shared enumerations
│   └── Constants/                    # Application constants
└── WorldLeaders.Infrastructure/    # Data access and external services
    ├── Data/                         # Entity Framework context
    ├── Entities/                     # Database entities
    ├── Services/                     # External service integrations
    └── Migrations/                   # Database migrations
```

## 🎮 Game Mechanics & Rules

### Core Game Flow

1. **Career Progression**: Dice roll determines job level (1-2: farmer/gardener, 3-4: shopkeeper/artisan, 5-6: politician/business leader)
2. **Random Events**: Card-based system with good/bad events affecting stats
3. **Fortune Telling**: AI predictions about future events and strategy
4. **Happiness Management**: Population satisfaction meter affecting gameplay
5. **Territory Acquisition**: Purchase countries using income and reputation (based on real GDP data)
6. **Language Learning**: Speech recognition challenges for territory languages

### Resource Management

- **Income**: Monthly earnings from job and territories
- **Reputation**: 0-100% scale, required for territory purchases
- **Happiness**: Population satisfaction meter (0-100%)

### Win/Loss Conditions

- **Win**: Achieve 100% reputation OR acquire all territories
- **Loss**: Happiness drops to zero OR failure to recover from setbacks

## 🤖 AI Agent System

### Agent Types & Personalities

1. **Career Guide Agent**: Encouraging mentor for job progression
2. **Event Narrator Agent**: Dramatic storyteller for random events
3. **Fortune Teller Agent**: Mystical advisor for strategic insights
4. **Happiness Advisor Agent**: Caring diplomat for population management
5. **Territory Strategist Agent**: Military strategist for expansion planning
6. **Language Tutor Agent**: Patient teacher for pronunciation practice

### AI Safety & Content Guidelines

- **Age-Appropriate**: All content must be suitable for 12-year-olds
- **Educational Focus**: Promote learning and positive values
- **Safety Filters**: Content moderation for inappropriate responses
- **Encouraging Tone**: Supportive and motivational messaging
- **Cultural Sensitivity**: Respectful representation of countries and cultures

## 🌍 Real-World Data Integration

### Territory Pricing Based on GDP

- **Tier 1**: Small countries (GDP rank 100+) - Easy acquisition
- **Tier 2**: Medium countries (GDP rank 30-100) - Moderate difficulty
- **Tier 3**: Major powers (GDP rank 1-30) - High reputation required

### Examples

- **Nepal**: $5K cost, 10% reputation required
- **Canada**: $50K cost, 40% reputation required
- **USA**: $200K cost, 85% reputation required

## 💻 Development Guidelines

### Coding Standards

- **C# Conventions**: Follow Microsoft C# coding standards
- **Async/Await**: Use async patterns for all I/O operations
- **Dependency Injection**: Use built-in DI container
- **Error Handling**: Comprehensive try-catch with logging
- **Child Safety**: Always validate and sanitize user inputs

### Version Management Guidelines (CRITICAL)

- **LTS-First Development**: Always use LTS (Long Term Support) versions for stability
- **Target Framework**: .NET 8 LTS (net8.0) - DO NOT use net9.0 or bleeding-edge versions
- **Package Versions**: Match package versions to target framework (8.x.x for .NET 8 projects)
- **Stability Over Features**: Prioritize proven stability over cutting-edge features
- **Educational Context**: Version consistency is crucial for learning environments

#### Package Version Examples

```xml
<!-- CORRECT: LTS versions matching .NET 8 target framework -->
<TargetFramework>net8.0</TargetFramework>
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.8" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="8.0.8" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.8" />

<!-- INCORRECT: Latest versions that may conflict with target framework -->
<TargetFramework>net8.0</TargetFramework>
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="9.0.7" />
<PackageReference Include="Microsoft.AspNetCore.SignalR.Client" Version="9.0.7" />
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="9.0.0" />
```

#### Why LTS Versions Matter

- **Production Stability**: Proven track record in enterprise environments
- **Educational Reliability**: Consistent behavior for students and teachers
- **Long-term Support**: Extended support lifecycle and security updates
- **Deployment Confidence**: Reduced risk of version conflicts and breaking changes
- **Team Consistency**: All developers use same stable foundation

### Blazor Component Guidelines

- **Responsive Design**: Mobile-first approach with TailwindCSS
- **Accessibility**: WCAG 2.1 AA compliance for children
- **Performance**: Minimize re-renders and optimize state management
- **Interactive Elements**: Child-friendly animations and feedback
- **Color Scheme**: Bright, engaging colors suitable for young players

### AI Integration Patterns

```csharp
// Always use this pattern for AI agent calls
public async Task<AgentResponse> GetAgentResponseAsync(
    AgentType agentType,
    GameContext context,
    string userInput)
{
    var response = await _aiService.GenerateResponseAsync(agentType, context, userInput);
    var isAppropriate = await _contentModerator.ValidateAsync(response);
    return isAppropriate ? response : GetFallbackResponse(agentType);
}
```

### Database Entity Conventions

- **Guid IDs**: Use Guid for all primary keys
- **Audit Fields**: Include CreatedAt, UpdatedAt for all entities
- **Soft Deletes**: Use IsDeleted flag instead of hard deletes
- **JSON Columns**: Store complex game state as JSON when appropriate

### Essential Domain Models

```csharp
// Core game entities that should guide all development
public class Player
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public int Income { get; set; }
    public int Reputation { get; set; } // 0-100%
    public int Happiness { get; set; } // 0-100%
    public JobLevel CurrentJob { get; set; }
    public List<Territory> OwnedTerritories { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

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

## 🎨 UI/UX Design Principles

### Child-Friendly Design

- **Large Buttons**: Easy to click for young users
- **Clear Typography**: Readable fonts and appropriate sizes
- **Visual Feedback**: Immediate response to user actions
- **Progress Indicators**: Show advancement clearly
- **Emoji Integration**: Use emojis to make interface engaging

### Color Guidelines

- **Primary**: Blue/Purple gradients for main interface
- **Success**: Green for achievements and positive feedback
- **Warning**: Yellow/Orange for alerts and notifications
- **Error**: Red for problems (used sparingly)
- **Neutral**: Gray for secondary information

### Animation Guidelines

- **Smooth Transitions**: 300ms duration for most animations
- **Bounce Effects**: Use for celebratory moments
- **Loading States**: Engaging spinners and progress bars
- **Micro-interactions**: Hover effects and button feedback

## 🔒 Security & Privacy Considerations

### Data Protection

- **Children's Privacy**: Comply with COPPA and GDPR
- **Minimal Data Collection**: Only collect necessary information
- **Secure Storage**: Encrypt sensitive data at rest
- **API Security**: Use HTTPS and proper authentication
- **Speech Data**: Process audio locally when possible

### Content Safety

- **AI Content Filtering**: Multiple layers of content validation
- **Profanity Filters**: Block inappropriate language
- **Cultural Sensitivity**: Respect for all cultures and countries
- **Educational Value**: Ensure all content has learning benefits

## 🧪 Testing Strategy

### Unit Testing Focus Areas

- **Game Logic**: Turn progression, resource calculations
- **AI Responses**: Agent personality consistency
- **Data Validation**: Input sanitization and validation
- **Error Scenarios**: Graceful failure handling

### Integration Testing

- **API Endpoints**: Full request/response cycles
- **Database Operations**: CRUD operations and relationships
- **External Services**: World Bank API, Speech Services
- **SignalR**: Real-time communication

### User Testing

- **Age-Appropriate Design**: Test with target age group
- **Accessibility**: Screen reader and keyboard navigation
- **Performance**: Load times and responsiveness
- **Educational Value**: Learning outcome measurement

## 📦 Deployment & DevOps

### Environment Configuration

- **Development**: Local with Aspire orchestration
- **Staging**: Azure Container Apps for testing
- **Production**: Azure with high availability setup

### CI/CD Pipeline

- **Build**: Restore, build, test all projects
- **Security Scan**: SAST and dependency scanning
- **Deploy**: Automated deployment to Azure
- **Monitoring**: Application insights and health checks

## 🎯 Performance Guidelines

### Optimization Targets

- **Page Load**: < 2 seconds initial load
- **API Response**: < 500ms for most operations
- **AI Response**: < 3 seconds for agent interactions
- **Speech Processing**: < 2 seconds for pronunciation analysis

### Caching Strategy

- **Memory Cache**: Frequently accessed game data
- **Distributed Cache**: Redis for session state
- **CDN**: Static assets and images
- **API Cache**: World Bank GDP data (24-hour TTL)

## 🌟 Code Generation Preferences

When generating code, please:

1. **Follow Patterns**: Use established patterns from existing codebase
2. **Include Logging**: Add appropriate logging statements
3. **Error Handling**: Implement comprehensive error handling
4. **Documentation**: Include XML documentation for public methods
5. **Testing**: Suggest test cases for new functionality
6. **Child-Safe**: Ensure all generated content is appropriate for 12-year-olds
7. **Educational**: Focus on learning opportunities in implementations
8. **Performance**: Consider performance implications of generated code
9. **Accessibility**: Include accessibility attributes in UI components
10. **Real Data**: Use actual GDP/country data when creating examples

### Comment-Driven Development Pattern

Always use this structured comment format to guide code generation:

```csharp
// Context: [Educational game component description]
// Target Audience: 12-year-old players learning [specific concept]
// Educational Objective: [What this teaches - economics, geography, language, etc.]
// Visual Requirements: [Based on child's mockups if applicable]
// Technical Requirements: [Blazor Server, TailwindCSS, SignalR, etc.]
// Safety Requirements: [Age-appropriate, culturally sensitive, positive messaging]
// Pattern: [Follows established architecture - game component, service, etc.]
// Integration: [How it connects with AI agents, real-world data, etc.]
public class ComponentName : ComponentBase
{
    // Copilot generates implementation based on structured guidance
}
```

### Blazor Component Template

Use this template for all game components:

```razor
@*
Context: Educational game component for 12-year-old players
Educational Goal: [Specific learning objective]
Child-UX: Large buttons, clear feedback, encouraging messages
*@

@using WorldLeaders.Shared.Models
@using WorldLeaders.Shared.Enums
@inherits ComponentBase
@inject IJSRuntime JSRuntime
@inject ILogger<ComponentName> Logger

<div class="game-component p-6 bg-gradient-to-br from-purple-400 to-blue-500 rounded-lg shadow-lg">
    @* Component content with child-friendly design *@
</div>

@code {
    // Component logic with educational focus and safety considerations
}
```

## 🎪 Special Considerations

### Educational Game Development

- **Learning Objectives**: Each feature should have educational value
- **Engagement**: Balance fun with learning outcomes
- **Difficulty Progression**: Gradual increase in complexity
- **Positive Reinforcement**: Celebrate achievements and progress
- **Cultural Awareness**: Promote understanding of different cultures

### Speech Recognition Integration

- **Multiple Languages**: Support various country languages
- **Pronunciation Assessment**: Detailed phoneme-level feedback
- **Progress Tracking**: Monitor improvement over time
- **Child-Friendly Feedback**: Encouraging and constructive

### AI Agent Personality

- **Consistency**: Maintain character personalities across interactions
- **Age-Appropriate**: Language and concepts suitable for 12-year-olds
- **Educational**: Provide learning opportunities in every interaction
- **Encouraging**: Supportive and motivational tone
- **Safe**: Content filtering and fallback responses

This instruction file should guide GitHub Copilot to provide contextually appropriate suggestions that align with the project's educational goals, technical architecture, and child-safety requirements.

## 🚀 Development Workflow & Startup Instructions

### Phase 1: Solution Setup (Current Phase)

When creating the initial solution structure:

1. **Create .NET Aspire Solution**:

   ```bash
   dotnet new aspire -n WorldLeaders
   ```

2. **Add Project References**:

   - WorldLeaders.AppHost (Aspire orchestrator)
   - WorldLeaders.Web (Blazor Server)
   - WorldLeaders.API (Web API)
   - WorldLeaders.Shared (Shared models)
   - WorldLeaders.Infrastructure (Data & external services)

3. **Install Required Packages**:
   - Entity Framework Core with PostgreSQL provider
   - Azure OpenAI SDK
   - Azure Speech Services SDK
   - TailwindCSS integration
   - SignalR packages

### Phase 2: Core Infrastructure

1. **Database Setup**: Create entities based on domain models above
2. **AI Service Integration**: Implement the 6 AI agent personalities
3. **External API Integration**: World Bank API and REST Countries API
4. **Authentication**: Simple user management for child safety

### Phase 3: Game Engine

1. **Game State Management**: Turn-based progression system
2. **Resource Tracking**: Income, reputation, happiness meters
3. **Event System**: Random card-based events
4. **Territory Management**: GDP-based country acquisition

### Development Priorities

1. **Child Safety First**: All features must include safety considerations
2. **Educational Value**: Every component should teach something
3. **Mobile-Responsive**: Design for tablets and phones
4. **Performance**: Keep load times under 2 seconds
5. **Accessibility**: WCAG 2.1 AA compliance

### Code Review Checklist

Before generating any code, ensure:

- [ ] Age-appropriate content (12-year-olds)
- [ ] Educational objective clearly defined
- [ ] Child-friendly UI/UX patterns
- [ ] Proper error handling and logging
- [ ] Security and privacy considerations
- [ ] Performance optimization
- [ ] Accessibility features included
