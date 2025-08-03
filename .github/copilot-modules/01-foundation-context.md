# Copilot Module 1: Project Foundation & Context
# Educational game development with 95% AI autonomy

## 🎯 Core Project Context

**Project**: World Leaders Game - Educational strategy game for 12-year-olds
**Methodology**: AI-first development with 95% autonomy, 5% human guidance
**Timeline**: 18-week development journey (currently Week 3)
**Origin**: Child's 5-minute voice memo transformed into production-ready game

## 🏗️ Technical Architecture

### Technology Stack
- **.NET 8 LTS** (net8.0 - LTS versions only, never net9.0)
- **ASP.NET Core** with Blazor Server
- **.NET Aspire** for orchestration
- **Entity Framework Core** with PostgreSQL
- **TailwindCSS** for child-friendly UI
- **SignalR** for real-time updates
- **Azure OpenAI** for AI agents
- **Azure Speech Services** for language learning

### Project Structure
```
src/WorldLeaders/
├── WorldLeaders.AppHost/        # Aspire orchestration
├── WorldLeaders.Web/            # Blazor Server UI
├── WorldLeaders.API/            # Game API services
├── WorldLeaders.Shared/         # Shared models/DTOs
└── WorldLeaders.Infrastructure/ # Data access & external services
```

## 🎮 Game Mechanics Foundation

### Core Gameplay
- **Career Progression**: Dice roll determines job level (1-6 scale)
- **Resource Management**: Income, Reputation (0-100%), Happiness (0-100%)
- **Territory Acquisition**: Purchase countries using real GDP data
- **AI Agents**: 6 personality types for educational guidance
- **Language Learning**: Speech recognition for country languages
- **Random Events**: Card-based system affecting player stats

### Educational Objectives
- **Economics**: GDP understanding, resource management
- **Geography**: Country knowledge, cultural awareness
- **Language**: Pronunciation practice, multilingual exposure
- **Strategy**: Decision-making, consequence understanding

## 👶 Child Safety & Educational Standards

### COPPA Compliance
- All content must be appropriate for 12-year-olds
- Positive messaging and encouraging feedback
- Cultural sensitivity and respectful representation
- Content moderation for all AI-generated responses

### Design Principles
- **Large touch targets**: Minimum 44px for mobile interaction
- **Clear visual feedback**: Immediate response to user actions
- **Encouraging tone**: Supportive messaging for all outcomes
- **Educational value**: Every feature teaches something meaningful

## 🤖 AI Development Principles

### When generating code:
1. **Educational context first**: What does this teach?
2. **Child safety verification**: Age-appropriate content only
3. **Mobile-first design**: Responsive and touch-friendly
4. **Performance optimization**: Fast loading for engagement
5. **Accessibility compliance**: WCAG 2.1 AA standards

### Architecture Patterns
- Use established patterns from existing codebase
- Include comprehensive error handling and logging
- Add XML documentation for public methods
- Suggest test cases for new functionality
- Follow .NET 8 LTS conventions consistently

This foundation module should guide all subsequent development decisions and code generation.