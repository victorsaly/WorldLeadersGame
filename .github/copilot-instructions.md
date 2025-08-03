# 🤖 GitHub Copilot Instructions - World Leaders Game

**IMPORTANT**: This is the condensed master file. For detailed instructions, see the [modular instruction system](./copilot-instructions/README.md).

## 🎯 Quick Reference

### Project Identity
**Educational Strategy Game** for **12-year-old players** learning geography, economics, and languages through AI-assisted gameplay.

### Modular Instructions System
| Task Type | Primary Modules | Use Case |
|-----------|----------------|----------|
| **Any Development** | [Core Principles](./copilot-instructions/core-principles.md) | Always active - fundamental guidelines |
| **Game Features** | [Educational Game Development](./copilot-instructions/educational-game-development.md) | Game mechanics, AI agents, progression |
| **Technical Work** | [Technical Architecture](./copilot-instructions/technical-architecture.md) | .NET Aspire, Blazor, Entity Framework |
| **UI/UX Design** | [UI/UX Guidelines](./copilot-instructions/ui-ux-guidelines.md) | Child-friendly design, accessibility |
| **AI Content** | [AI Safety & Child Protection](./copilot-instructions/ai-safety-and-child-protection.md) | All AI interactions with children |
| **Documentation** | [Documentation Standards](./copilot-instructions/documentation-standards.md) | Creating/updating docs, blogs, journey |
| **New Features** | [Feature Development Process](./copilot-instructions/feature-development-process.md) | Complete feature implementation workflow |

### 🚨 Child Safety Priority
**ALL** AI content MUST be validated through [AI Safety & Child Protection](./copilot-instructions/ai-safety-and-child-protection.md) module.

## 🏗️ Essential Technical Stack

**.NET 8 LTS** | **Blazor Server** | **PostgreSQL** | **Azure OpenAI** | **TailwindCSS**

### Core Architecture
```
src/
├── WorldLeaders.AppHost/      # .NET Aspire orchestration
├── WorldLeaders.Web/          # Blazor Server (child-friendly UI)
├── WorldLeaders.API/          # Game API + SignalR hubs
├── WorldLeaders.Shared/       # Domain models + DTOs
└── WorldLeaders.Infrastructure/ # Data access + external services
```

## 🎮 Core Game Mechanics

**Dice-Based Career Progression** → **Territory Acquisition** → **Language Learning** → **World Leadership**

### Essential Models
```csharp
public class Player {
    public int Income { get; set; }      // Monthly earnings
    public int Reputation { get; set; }  // 0-100% for territory purchases  
    public int Happiness { get; set; }   // Population satisfaction
    public JobLevel CurrentJob { get; set; }
    public List<Territory> OwnedTerritories { get; set; }
}

public enum AgentType {
    CareerGuide, EventNarrator, FortuneTeller, 
    HappinessAdvisor, TerritoryStrategist, LanguageTutor
}
```

## 🛡️ Child Safety Framework

### Content Validation Pipeline
```csharp
public async Task<AgentResponse> GenerateResponseAsync(AgentType type, string input)
{
    var response = await _aiService.GenerateAsync(type, input);
    var isAppropriate = await _contentModerator.ValidateAsync(response);
    return isAppropriate ? response : GetSafeFallbackResponse(type);
}
```

**Critical**: Every AI interaction must include child safety validation.

## 📚 Documentation & Process

**Jekyll Documentation**: Comprehensive educational journey tracking in `/docs`
**Process**: Every feature MUST follow [Feature Development Process](./copilot-instructions/feature-development-process.md)

### Documentation Triggers
- **New Features** → Update `_technical/` + create blog post
- **Weekly Progress** → Update `_journey/week-##-title.md`
- **Major Milestones** → Create `_milestones/milestone-##-title.md`

**File Naming**: `lowercase-with-hyphens.md` | **Special Files**: `UPPERCASE.md`

## 💻 Essential Development Patterns

### LTS-First Development (CRITICAL)
```xml
<TargetFramework>net8.0</TargetFramework>
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.8" />
<!-- Use .NET 8 LTS versions only -->
```

### Child-Friendly Component Template
```razor
@* Context: Educational game component for 12-year-old players *@
<div class="btn-child-friendly">
    <!-- Large buttons, clear feedback, encouraging messages -->
</div>

@code {
    // Educational objective and child safety validation required
}
```

### Educational Feature Pattern
```csharp
// Context: Educational game component for 12-year-old geography learning
// Educational Objective: Teach country recognition and economic concepts
// Safety Requirements: Age-appropriate content, positive messaging
public class EducationalFeature
{
    // Implementation with safety validation and learning objectives
}
```

## 🔄 Development Workflow Quick Start

### For New Features
1. **Planning**: Use [Educational Game Development](./copilot-instructions/educational-game-development.md) for game mechanics
2. **Implementation**: Follow [Technical Architecture](./copilot-instructions/technical-architecture.md) patterns
3. **UI Design**: Apply [UI/UX Guidelines](./copilot-instructions/ui-ux-guidelines.md) for child-friendly design
4. **AI Content**: Validate through [AI Safety & Child Protection](./copilot-instructions/ai-safety-and-child-protection.md)
5. **Documentation**: Complete [Feature Development Process](./copilot-instructions/feature-development-process.md)

### For Documentation Tasks
- **Creating**: Follow [Documentation Standards](./copilot-instructions/documentation-standards.md)
- **Structure**: Maintain Jekyll collections in `/docs`
- **Naming**: `lowercase-with-hyphens.md` (collections) | `UPPERCASE.md` (special guides)

### Quick Development Patterns
```csharp
// AI Agent Response (with safety validation)
var response = await _aiService.GenerateResponseAsync(agentType, context, userInput);
var isAppropriate = await _contentModerator.ValidateAsync(response);
return isAppropriate ? response : GetFallbackResponse(agentType);

// Educational Component Template
// Context: Educational game component for 12-year-old [learning objective]
// Safety: Age-appropriate content, positive messaging
public class EducationalComponent : ComponentBase { /* implementation */ }
```

## 📋 Critical Reminders

- **Child Safety First**: ALL AI content must be validated for 12-year-old appropriateness
- **Educational Value**: Every feature must teach geography, economics, or language concepts
- **LTS Versions**: Use .NET 8 LTS packages only for stability
- **Documentation**: Every feature triggers automatic documentation updates
- **Real-World Data**: Connect game mechanics to actual GDP and country data

## 📚 Complete Instruction Modules

For detailed implementation guidance, see:

1. **[Core Principles](./copilot-instructions/core-principles.md)** - Fundamental educational guidelines (always active)
2. **[Educational Game Development](./copilot-instructions/educational-game-development.md)** - Game mechanics and learning systems
3. **[Technical Architecture](./copilot-instructions/technical-architecture.md)** - .NET Aspire, Blazor, Entity Framework patterns
4. **[UI/UX Guidelines](./copilot-instructions/ui-ux-guidelines.md)** - Child-friendly design and accessibility
5. **[AI Safety & Child Protection](./copilot-instructions/ai-safety-and-child-protection.md)** - Content moderation and child safety
6. **[Documentation Standards](./copilot-instructions/documentation-standards.md)** - Documentation creation and maintenance
7. **[Feature Development Process](./copilot-instructions/feature-development-process.md)** - Complete feature implementation workflow

---

**Remember**: This educational game teaches 12-year-olds about world geography, economics, and languages through safe, AI-assisted gameplay. Every development decision must support this learning mission.

