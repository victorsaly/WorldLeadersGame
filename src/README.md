# 🌍 World Leaders Game - Educational Strategy Game

A comprehensive .NET Aspire educational game designed for 12-year-old players to learn about world leadership, geography, and languages through strategic gameplay.

## 🎯 Project Overview

This educational strategy game combines fun gameplay with learning objectives, teaching young players about:
- Strategic decision-making and leadership skills
- World geography and country information
- Language learning with pronunciation practice
- Economic concepts through resource management
- Social responsibility and diplomacy

## 🏗️ Architecture

### Technology Stack
- **.NET 8** with **ASP.NET Core**
- **.NET Aspire** for orchestration and service discovery
- **Blazor Server** for interactive web UI with child-friendly design
- **TailwindCSS** for responsive, accessible styling
- **Entity Framework Core** with **PostgreSQL** for data persistence
- **SignalR** for real-time game updates
- **AI Integration** ready for Azure OpenAI services

### Project Structure
```
src/WorldLeaders/
├── WorldLeaders.AppHost/           # .NET Aspire orchestration
├── WorldLeaders.Web/               # Blazor Server application
│   ├── Components/Game/            # Game-specific components
│   ├── Components/Shared/          # Shared UI components
│   ├── Components/Layout/          # Layout components (generated)
│   ├── Components/Pages/           # Blazor pages
│   └── Services/                   # Client-side services
├── WorldLeaders.API/               # Game API services
│   ├── Controllers/                # API controllers
│   ├── Hubs/                      # SignalR hubs for real-time updates
│   └── Services/                  # Business logic services
├── WorldLeaders.Shared/            # Shared models and contracts
│   ├── Models/                    # Domain models
│   ├── DTOs/                      # Data transfer objects
│   ├── Enums/                     # Shared enumerations
│   └── Constants/                 # Application constants
├── WorldLeaders.Infrastructure/    # Data access and external services
│   ├── Data/                      # Entity Framework context
│   ├── Entities/                  # Database entities
│   ├── Services/                  # External service integrations
│   └── Migrations/                # Database migrations
└── WorldLeaders.ServiceDefaults/   # Aspire service defaults
```

## 🎮 Game Mechanics

### Core Gameplay
1. **Career Progression**: Dice roll determines job level (Farmer → Gardener → Shopkeeper → Artisan → Politician → Business Leader)
2. **Resource Management**: Manage Income, Reputation (0-100%), and Population Happiness (0-100%)
3. **Territory Acquisition**: Purchase countries using income and reputation requirements
4. **Language Learning**: Complete pronunciation challenges to unlock territory bonuses
5. **Random Events**: Educational card-based events affecting player stats
6. **AI Guidance**: Six different AI agent types for educational assistance

### Educational Features
- **Real-world GDP data** for territory pricing and difficulty
- **Age-appropriate content** with built-in safety measures
- **Cultural sensitivity** in country representation
- **Positive reinforcement** for learning achievements
- **Progress tracking** and skill development

## 🚀 Getting Started

### Prerequisites
- .NET 8 SDK
- Docker Desktop (for PostgreSQL)
- Visual Studio 2022 or VS Code with C# extension

### Running the Application

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd WorldLeadersGame/src/WorldLeaders
   ```

2. **Build the solution**
   ```bash
   dotnet build
   ```

3. **Run with .NET Aspire**
   ```bash
   dotnet run --project WorldLeaders.AppHost
   ```

4. **Access the application**
   - Game Web App: `https://localhost:7154`
   - Game API: `https://localhost:7155`
   - Aspire Dashboard: `https://localhost:15000` (when available)

### Database Setup
The application uses PostgreSQL orchestrated by .NET Aspire. The database will be automatically created and configured when running the AppHost project.

## 🎨 UI Design Philosophy

### Child-Friendly Design
- **Large, colorful buttons** with emoji icons for visual appeal
- **High contrast colors** for accessibility
- **Simple navigation** with clear visual hierarchy
- **Engaging animations** to maintain interest
- **Responsive design** for different screen sizes

### Educational UX
- **Clear feedback** for all actions
- **Progress indicators** to show advancement
- **Helpful tooltips** explaining game mechanics
- **Positive reinforcement** messaging
- **Safe, moderated** content throughout

## 📊 Key Features Implemented

### 🎯 Game Controllers & API
- Player creation and dashboard management
- Territory listing and acquisition system
- Random game events for educational content
- Job progression through dice rolling
- RESTful API with comprehensive documentation

### 🎮 Interactive Blazor Components
- Real-time game dashboard with live updates
- Educational home page with game introduction
- SignalR integration for real-time notifications
- Mobile-responsive design with TailwindCSS

### 🗄️ Data Layer
- Entity Framework Core with PostgreSQL
- Comprehensive domain models for game entities
- Audit trails and soft delete functionality
- Performance-optimized queries with proper indexing

### 🔄 Real-time Features
- SignalR hubs for live game updates
- Real-time stat changes and notifications
- Multi-player preparation (group sessions)

## 🛡️ Educational Safety Features

### Content Moderation
- Age-appropriate content validation
- Built-in profanity filtering preparation
- Cultural sensitivity guidelines
- Positive messaging enforcement

### Child Protection
- No personal information collection beyond username
- Safe, educational AI interactions
- Parental guidance recommendations
- Clear educational objectives

## 🔮 Future Enhancements

### AI Integration
- Azure OpenAI integration for educational AI agents
- Speech recognition for language learning
- Personalized learning recommendations
- Intelligent tutoring system

### Advanced Features
- Multiplayer collaborative gameplay
- Achievement and badge systems
- Progress reports for educators/parents
- Curriculum alignment tools

### Scalability
- Cloud deployment with Azure
- Performance monitoring and analytics
- A/B testing for educational effectiveness
- Multi-language support

## 🏆 Educational Value

This game promotes:
- **Critical thinking** through strategic decision-making
- **Global awareness** through geography and cultural learning
- **Language skills** through pronunciation practice
- **Economic literacy** through resource management
- **Social skills** through diplomacy simulation
- **Technology literacy** through interactive digital learning

## 🤝 Contributing

Contributions are welcome! Please ensure all contributions align with educational guidelines and child safety standards.

## 📄 License

This educational project is designed to promote learning and positive values for young learners.

---

**World Leaders Game** - Empowering the next generation of global leaders through strategic, educational gameplay! 🌟
