# World Leaders Game: AI-Powered Educational Strategy Game

*From a 12-year-old's voice memo to a full-featured educational game built with AI assistance*

---

## 🎯 Project Overview

**World Leaders Game** is an educational strategy game where players progress from peasant to world leader by managing resources, learning languages, and acquiring territories based on real-world GDP data. The game combines strategic thinking with educational content about global economics, geography, and language learning.

### **🌟 What Makes This Special**
- **Real-World Data**: Territory pricing based on actual GDP from World Bank API
- **AI Tutors**: 6 specialized AI agents guide players through each game phase
- **Speech Recognition**: Learn languages of owned territories with pronunciation feedback
- **Child-Safe**: Built with privacy protection and age-appropriate content moderation
- **Educational Focus**: Every game mechanic teaches valuable real-world concepts

---

## 🎮 Core Gameplay

### **The 6-Phase Game Loop**

1. **🎲 Career Roll**: Dice determines job level (farmer → politician → world leader)
2. **🃏 Random Events**: Cards bring unexpected challenges and opportunities
3. **🔮 Fortune Telling**: AI oracle provides strategic insights and predictions
4. **😊 Happiness Management**: Keep your population satisfied to avoid game over
5. **🌍 Territory Acquisition**: Buy countries using income and reputation
6. **🗣️ Language Learning**: Master languages through speech recognition challenges

### **Win Conditions**
- Achieve 100% reputation (diplomatic victory)
- Acquire all territories (conquest victory)

### **Educational Outcomes**
- **Economics**: Learn about GDP, global markets, resource management
- **Geography**: Discover countries, capitals, regions, and cultural diversity
- **Languages**: Practice pronunciation and vocabulary for multiple languages
- **Strategy**: Develop critical thinking and long-term planning skills

---

## 🤖 AI-Powered Development

This project showcases **AI-first development methodology** using:

### **Claude Sonnet 3.5**
- Architecture design and technical planning
- Educational content creation and safety guidelines
- Complex problem-solving and documentation

### **GitHub Copilot**
- Real-time code generation and completion
- Pattern recognition and refactoring assistance
- Test creation and debugging support

### **Development Stats**
- **95% AI-generated** documentation and planning (Week 1)
- **Estimated 70-80% time savings** vs traditional development
- **18-week timeline** from concept to production-ready game

---

## 📁 Project Structure

```
WorldLeadersGame/
├── docs/                              # 📚 All project documentation
│   ├── 01-development-blog.md         # Public development journey blog
│   ├── 02-development-plan.md         # 18-week roadmap and milestones
│   ├── 03-copilot-prompts.md         # Ready-to-use AI prompts
│   ├── 04-technical-guide.md         # Implementation details and architecture
│   └── README.md                      # Documentation index
├── src/                               # 💻 Source code (coming in Week 2)
│   ├── WorldLeaders.AppHost/       # .NET Aspire orchestration
│   ├── WorldLeaders.Web/           # Blazor Server application
│   ├── WorldLeaders.API/           # Game API services
│   ├── WorldLeaders.Shared/        # Shared models and contracts
│   └── WorldLeaders.Infrastructure/ # Data access and external services
├── .github/
│   └── copilot-instructions.md        # GitHub Copilot context configuration
└── README.md                          # This file
```

---

## 🏗️ Technology Stack

### **Backend**
- **.NET 8** with **ASP.NET Core**
- **.NET Aspire** for cloud-native orchestration
- **Entity Framework Core** with **PostgreSQL**
- **SignalR** for real-time game updates

### **Frontend**
- **Blazor Server** for interactive C# web UI
- **TailwindCSS** for child-friendly responsive design
- **JavaScript** for speech recognition and animations

### **AI & External Services**
- **Azure OpenAI Service** (GPT-4) for 6 specialized AI agents
- **Azure Speech Services** for pronunciation assessment
- **World Bank API** for real GDP data
- **REST Countries API** for country information

---

## 🚀 Quick Start

### Running the Application
```bash
# Clone the repository
git clone https://github.com/victorsaly/WorldLeadersGame.git
cd WorldLeadersGame/src/WorldLeaders

# Install Aspire workload (required for orchestration)
dotnet workload install aspire

# Install dependencies
dotnet restore

# Run the complete application stack (.NET Aspire)
cd WorldLeaders.AppHost
dotnet run
```

**Access Points:**
- **Game Web App**: `https://localhost:7154`
- **Game API**: `https://localhost:7155`  
- **API Documentation**: `https://localhost:7155/swagger`

📖 **For detailed command-line instructions**: See [Command Line Guide](docs/07-command-line-guide.md)

---

## 🚀 Development Timeline

| Phase | Duration | Status | AI Contribution | Focus Area |
|-------|----------|--------|-----------------|------------|
| **Planning & Architecture** | Week 1 | ✅ Complete | 95% | Technical design, documentation |
| **Project Setup** | Week 2-3 | 🟡 Next | 85% | .NET Aspire, database, AI services |
| **Core Game Engine** | Week 4-5 | ⭕ Planned | 80% | Game mechanics, state management |
| **AI Agent Framework** | Week 6-7 | ⭕ Planned | 90% | 6 specialized AI personalities |
| **Real-World Data** | Week 8 | ⭕ Planned | 75% | GDP integration, territory pricing |
| **Speech Recognition** | Week 9-10 | ⭕ Planned | 70% | Language learning, pronunciation |
| **UI/UX Development** | Week 11-12 | ⭕ Planned | 60% | Child-friendly interface, animations |
| **Testing & Deployment** | Week 13-14 | ⭕ Planned | 85% | Azure deployment, monitoring |

---

## 📖 Documentation

### **For Developers**
- **[Development Plan](docs/02-development-plan.md)**: Complete 18-week roadmap
- **[Technical Guide](docs/04-technical-guide.md)**: Architecture and implementation details
- **[Command Line Guide](docs/07-command-line-guide.md)**: Complete guide for running and managing the application
- **[Copilot Prompts](docs/03-copilot-prompts.md)**: AI-assisted development prompts

### **For Community**
- **[Development Blog](docs/01-development-blog.md)**: Public journey from voice memo to game
- **[GitHub Copilot Instructions](.github/copilot-instructions.md)**: AI context configuration

### **For Educators**
- Child-safe AI interaction patterns
- Educational game development best practices
- Real-world data integration for learning

---

## 🎓 Educational Impact

### **Learning Objectives**
- **Global Economics**: Understanding GDP, trade, and economic development
- **Cultural Awareness**: Respectful exploration of world countries and cultures
- **Language Skills**: Pronunciation practice and vocabulary building
- **Strategic Thinking**: Resource management and long-term planning
- **Technology Literacy**: Seeing how AI can assist in creative projects

### **Target Audience**
- **Primary**: 12-year-old students
- **Secondary**: Homeschool families, educational institutions
- **Tertiary**: Developers interested in AI-assisted educational content

---

## 🤝 Contributing & Community

### **Current Status**: Planning & Setup Phase
- ⭐ **Star** this repository to follow our progress
- 🐛 **Issues** for suggestions and feedback welcome
- 📢 **Discussions** for educational game development topics

### **Coming Soon**
- Open source code release (Week 2)
- Beta testing opportunities (Week 12)
- Educational framework extraction (Week 16)
- Community contribution guidelines

---

## 🔒 Child Safety & Privacy

### **Built-in Protections**
- **COPPA/GDPR Compliance**: Minimal data collection, parental controls
- **Content Moderation**: Multi-layer AI safety filters
- **Local Processing**: Speech recognition processed locally when possible
- **Educational Oversight**: All content reviewed for age-appropriateness

### **Safety-First Development**
- AI responses filtered for inappropriate content
- Positive, encouraging tone in all interactions
- Cultural sensitivity in country representation
- No personal data collection beyond game progress

---

## 📊 Project Metrics (Updated Weekly)

### **Week 1 Achievements**
- **📄 Documentation**: 11,500+ words generated with AI assistance
- **⏱️ Time Saved**: 2-3 weeks of traditional planning completed in 1 day
- **🤖 AI Efficiency**: 95% of planning phase automated
- **🎯 Scope Definition**: Complete game mechanics and technical architecture

---

## 🌟 Vision & Impact

This project demonstrates that **children's creativity + AI assistance = rapid educational innovation**. By documenting our journey, we hope to:

- Inspire more parent-child coding collaborations
- Show how AI can accelerate educational content creation
- Provide reusable patterns for educational game development
- Prove that sophisticated learning experiences can be built quickly

**The goal isn't just to build a game — it's to show how AI can democratize the creation of high-quality educational content.**

---

## 📞 Contact & Updates

- **GitHub**: Real-time code and documentation updates
- **Blog**: Weekly progress posts and lessons learned
- **Community**: Educational game development discussions

*Follow our journey from voice memo to production game — updated weekly with progress, insights, and code!*

---

**Tags**: #AI #GameDevelopment #Education #DotNet #Blazor #ChildSafety #OpenSource #ParentChild #EdTech