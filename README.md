# 🎮 World Leaders Game
### An AI-First Father-Son Educational Software Development Experiment

<div align="center">

![Game Logo](https://raw.githubusercontent.com/victorsaly/WorldLeadersGame/main/docs/assets/world-leaders-logo.svg)

**📊 Current Status: Week 3 • 95% AI Autonomy • Educational Focus**

[![GitHub Pages](https://img.shields.io/badge/docs-GitHub%20Pages-blue?logo=github)](https://victorsaly.github.io/WorldLeadersGame)
[![.NET 8 LTS](https://img.shields.io/badge/.NET-8.0%20LTS-purple?logo=dotnet)](https://dotnet.microsoft.com/)
[![Blazor Server](https://img.shields.io/badge/Blazor-Server-red?logo=blazor)](https://blazor.net/)
[![Azure OpenAI](https://img.shields.io/badge/Azure-OpenAI-green?logo=microsoft)](https://azure.microsoft.com/en-us/products/ai-services/openai-service)
[![Educational](https://img.shields.io/badge/Educational-12%20year%20olds-yellow?logo=graduation-cap)](docs/about.md)

</div>

---

## 🚀 The Revolutionary Experiment

> **What happens when you give AI complete control over software architecture, implementation, and documentation while a father and his 12-year-old son act as creative directors?**

This project answers that question through a **live 18-week educational software development experiment**, transforming a passionate 5-minute voice memo from a London schoolchild into a production-ready educational strategy game.

### 📈 Current Achievements (Week 3)

| Metric | Achievement | Traditional Timeline |
|--------|-------------|---------------------|
| **AI Autonomy** | 95% | N/A |
| **Architecture** | Complete .NET Aspire Solution | 6-8 weeks |
| **Game Mechanics** | Core educational gameplay implemented | 4-6 weeks |
| **Documentation** | Comprehensive Jekyll site with 50+ pages | 3-4 weeks |
| **Child Safety** | Multi-layer AI content validation | 2-3 weeks |
| **Educational Value** | Real-world geography, economics, language learning | Ongoing |

**Total Traditional Development Time Saved: 15-21 weeks**

---

## 🎯 What We're Building

### Educational Strategy Game for 12-Year-Olds

**Core Concept**: Progress from peasant to world leader while learning geography, economics, and languages through engaging, AI-assisted gameplay.

#### 🎲 Game Mechanics
- **Career Progression**: Dice-based job advancement (Farmer → Politician → World Leader)
- **Territory Acquisition**: Real countries priced using World Bank GDP data
- **Resource Management**: Income, reputation, and population happiness
- **Language Learning**: Speech recognition and pronunciation training
- **AI Tutors**: Six educational AI agents providing guidance and support

#### 🌍 Real-World Educational Integration
- **Geography**: Country recognition and cultural awareness
- **Economics**: GDP concepts and strategic resource management
- **Languages**: Multi-lingual pronunciation with Azure Speech Services
- **Critical Thinking**: Decision-making with real consequences

---

## 🏗️ Technical Architecture

### Modern .NET Aspire Solution

```
🎮 WorldLeaders.AppHost        # Aspire orchestration
🌐 WorldLeaders.Web           # Blazor Server (child-friendly UI)
🔧 WorldLeaders.API           # Game logic + SignalR hubs
📊 WorldLeaders.Shared        # Domain models + DTOs  
🗄️ WorldLeaders.Infrastructure # EF Core + external services
```

### Technology Stack

| Layer | Technology | Purpose |
|-------|------------|---------|
| **Frontend** | Blazor Server + TailwindCSS | Child-friendly, responsive UI |
| **Backend** | ASP.NET Core 8 LTS | Stable educational platform |
| **Database** | PostgreSQL + Entity Framework Core | Robust data persistence |
| **AI Services** | Azure OpenAI (GPT-4) | Educational AI agents |
| **Speech** | Azure Speech Services | Pronunciation assessment |
| **Real-Time** | SignalR | Live gameplay updates |
| **Data** | World Bank API | Real GDP/country data |
| **Orchestration** | .NET Aspire | Microservices coordination |

### 🛡️ Child Safety Framework

- **Multi-Layer Content Moderation**: Azure + Custom educational validators
- **Privacy Protection**: COPPA/GDPR compliant data handling
- **Age-Appropriate Content**: All interactions validated for 12-year-olds
- **Safe AI Fallbacks**: Pre-approved responses for any validation failures
- **Parental Oversight**: Optional progress sharing and content review

---

## 📚 Comprehensive Documentation

Our documentation system captures the complete development journey, methodology, and educational outcomes:

### 🎯 [**Explore Full Documentation →**](https://victorsaly.github.io/WorldLeadersGame)

#### Key Documentation Sections

| Section | Purpose | Status |
|---------|---------|--------|
| [**Development Journey**](https://victorsaly.github.io/WorldLeadersGame/journey) | Week-by-week AI collaboration insights | 📝 Live Updates |
| [**Technical Guides**](https://victorsaly.github.io/WorldLeadersGame/technical-docs) | Implementation patterns and architecture | ✅ Complete |
| [**Educational Methodology**](https://victorsaly.github.io/WorldLeadersGame/blog) | AI-first development insights | 📊 Analytics |
| [**Project Milestones**](https://victorsaly.github.io/WorldLeadersGame/milestones) | Achievement tracking and metrics | 🎯 Goal Tracking |
| [**GitHub Issues**](https://github.com/victorsaly/WorldLeadersGame/issues) | AI-generated development tasks | 🤖 AI-Driven |

---

## 🎨 Child-Led Creative Process

### The 12-Year-Old Designer

Our young creative director didn't just describe the game—he designed it:

<div align="center">

| Original Sketches | Digital Logo Design |
|-------------------|---------------------|
| ![Game Mockup 1](https://raw.githubusercontent.com/victorsaly/WorldLeadersGame/main/docs/assets/game-mockup-1.png) | ![Game Mockup 2](https://raw.githubusercontent.com/victorsaly/WorldLeadersGame/main/docs/assets/game-mockup-2.png) |
| *Hand-drawn interface mockups* | *Independent Figma logo creation* |

</div>

**Child-Led Design Elements:**
- 🎨 Complete visual identity (logo, colors, layout)
- 🎮 Game mechanics and progression systems  
- 🌍 Educational objectives and learning goals
- 🎭 AI agent personalities and interaction styles

---

## 🤖 AI Collaboration Methodology

### The AI Dream Team

| AI System | Role | Autonomy Level |
|-----------|------|----------------|
| **Claude Sonnet 3.5** | Strategic architect, content creator | 95% |
| **GitHub Copilot** | Real-time implementation, code generation | 90% |
| **Azure OpenAI** | Educational AI agents, content validation | 85% |

### Human Oversight (5%)

- ✅ Educational validation and safety compliance
- ✅ Creative guidance and vision alignment  
- ✅ Compilation error resolution when AI cannot self-correct
- ✅ Real-world data accuracy verification

---

## 🚀 Quick Start

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [PostgreSQL](https://www.postgresql.org/) (or use Docker container)

### Run the Game

```bash
# Clone the repository
git clone https://github.com/victorsaly/WorldLeadersGame.git
cd WorldLeadersGame

# Start with .NET Aspire (recommended)
dotnet run --project src/WorldLeaders/WorldLeaders.AppHost

# Or run manually (see tasks.json for all options)
# 1. Start database: docker run --name worldleaders-postgres -e POSTGRES_DB=worldleaders -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15
# 2. Start API: dotnet run --project src/WorldLeaders/WorldLeaders.API  
# 3. Start Web: dotnet run --project src/WorldLeaders/WorldLeaders.Web
```

### Explore Documentation Locally

```bash
cd docs
bundle install
bundle exec jekyll serve
# Open http://localhost:4000
```

---

## 📊 Project Metrics & Status

### Week 3 Achievements

| Area | Status | AI Autonomy | Educational Impact |
|------|--------|-------------|-------------------|
| **Architecture** | ✅ Complete | 98% | Foundation for scalable learning |
| **Game Mechanics** | ✅ Core Implemented | 95% | Engaging educational gameplay |
| **Documentation** | ✅ Comprehensive | 95% | Methodology preservation |
| **Child Safety** | ✅ Multi-layer Protection | 90% | Safe learning environment |
| **AI Agents** | 🔄 In Progress | 90% | Personalized tutoring |
| **Speech Recognition** | 📋 Planned | TBD | Language learning assessment |

### Next Milestones (Week 4)

- [ ] **AI Agent Personality System** (8 hours estimated)
- [ ] **Territory Management with Real-World Data** (10 hours estimated)  
- [ ] **Speech Recognition Integration** (6 hours estimated)

---

## 🌟 Why This Matters

### For Educational Technology

- **Proof of Concept**: AI can create production-quality educational software with minimal human oversight
- **Methodology Documentation**: Complete process documentation for replication
- **Child-Centric Design**: Demonstrates importance of including learners in design process
- **Safety Framework**: Comprehensive approach to AI content moderation for children

### For Software Development

- **AI-First Development**: Revolutionary approach to software architecture and implementation
- **Documentation Automation**: AI-generated comprehensive project documentation
- **Real-World Integration**: Practical use of external APIs for educational content
- **Modern Stack Demonstration**: .NET Aspire, Blazor, Azure services working together

### For Parent-Child Collaboration

- **Shared Learning**: Father and son learning together through AI collaboration
- **Creative Empowerment**: 12-year-old as lead creative director and designer
- **STEM Education**: Practical software development experience
- **Documentation of Growth**: Complete journey preservation for reflection

---

## 🤝 Contributing

This is primarily a father-son learning experiment, but we welcome:

- **Educational feedback** from teachers and parents
- **Child safety suggestions** from child protection experts  
- **Technical insights** from AI and educational technology researchers
- **Documentation improvements** for better methodology sharing

### How to Contribute

1. 🔍 **Review Documentation**: [Full project documentation](https://victorsaly.github.io/WorldLeadersGame)
2. 📝 **Submit Issues**: Use our [AI-generated issue templates](https://github.com/victorsaly/WorldLeadersGame/issues)
3. 🗣️ **Join Discussions**: Share insights in [GitHub Discussions](https://github.com/victorsaly/WorldLeadersGame/discussions)
4. 📚 **Educational Use**: Adapt our methodology for your own projects

---

## 📞 Connect With Us

### Project Links

- 📖 **[Complete Documentation](https://victorsaly.github.io/WorldLeadersGame)** - Full development journey and methodology
- 🔄 **[Live Development](https://victorsaly.github.io/WorldLeadersGame/journey)** - Week-by-week progress updates  
- 🎯 **[GitHub Issues](https://github.com/victorsaly/WorldLeadersGame/issues)** - AI-generated development tasks
- 🏆 **[Milestones](https://victorsaly.github.io/WorldLeadersGame/milestones)** - Achievement tracking

### Educational Impact

> *"This isn't just building software—it's documenting a methodology for human-AI collaboration in educational technology while creating a genuine learning experience for a 12-year-old."*

**Join us on this journey of discovery, learning, and innovation in educational technology!**

---

<div align="center">

**Built with ❤️ by a father-son team and AI collaboration**

[📚 Documentation](https://victorsaly.github.io/WorldLeadersGame) • [🎮 Play Game](https://github.com/victorsaly/WorldLeadersGame/releases) • [🤖 AI Methodology](https://victorsaly.github.io/WorldLeadersGame/blog) • [📊 Progress](https://victorsaly.github.io/WorldLeadersGame/journey)

</div>
