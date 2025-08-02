# GitHub Issues-Driven Development for World Leaders Game

## 🎯 Methodology Overview

This guide demonstrates using GitHub Issues as detailed specifications for AI-powered development. Each issue contains comprehensive requirements that GitHub Copilot Chat can follow to generate contextually appropriate educational game code.

## 📋 Phase 1 GitHub Issues

### Issue #1: Create .NET Aspire Solution Structure
**URL**: https://github.com/victorsaly/WorldLeadersGame/issues/1

**Objective**: Create the complete .NET Aspire solution structure for World Leaders Game following comprehensive Copilot instructions.

**Key Requirements**:
- Create .NET Aspire solution: `dotnet new aspire -n WorldLeaders`
- Proper folder structure in `src/` directory
- 5 projects: AppHost, Web, API, Shared, Infrastructure
- Project references properly configured
- Solution compiles successfully

**Copilot Prompt**:
```
@workspace I need to solve GitHub Issue #1 for the World Leaders Game educational project. 

Please read the issue details at: https://github.com/victorsaly/WorldLeadersGame/issues/1

Key requirements:
- Follow comprehensive instructions in .github/copilot-instructions.md
- Create .NET Aspire solution structure
- Target audience: 12-year-old educational game players
- Educational game development patterns required

Steps needed:
1. Create src/ directory
2. Run: dotnet new aspire -n WorldLeaders
3. Create all 5 projects as specified in issue
4. Configure project references properly
5. Ensure solution compiles

Please generate the complete solution following the educational game patterns in the Copilot instructions.
```

### Issue #2: Install Required NuGet Packages
**URL**: https://github.com/victorsaly/WorldLeadersGame/issues/2

**Objective**: Install all required NuGet packages across the 5 projects for educational game functionality.

**Package Categories**:
- **Aspire Orchestration**: PostgreSQL and Redis hosting
- **Blazor Server**: SignalR, Components, TailwindCSS integration
- **API Services**: SignalR hubs, Swagger documentation, JWT authentication
- **Infrastructure**: Entity Framework, Azure AI services, HTTP clients
- **Shared Models**: Data annotations, JSON serialization

**Copilot Prompt**:
```
@workspace Now solve GitHub Issue #2 for NuGet package installation.

Issue URL: https://github.com/victorsaly/WorldLeadersGame/issues/2

Requirements:
- Install packages across all 5 projects
- Follow .github/copilot-instructions.md technology stack
- Ensure version compatibility
- Educational game focus

Please install all required packages and ensure the solution builds successfully.
```

### Issue #3: Create Core Domain Models
**URL**: https://github.com/victorsaly/WorldLeadersGame/issues/3

**Objective**: Implement the essential domain models in WorldLeaders.Shared for educational game functionality.

**Models to Implement**:
- **Player**: Game progression tracking with income, reputation, happiness
- **Territory**: Country information with GDP-based pricing
- **JobLevel Enum**: Career progression from farmer to world leader
- **AgentType Enum**: 6 AI agent personalities for educational guidance

**Educational Considerations**:
- Child-appropriate field names and concepts
- Real-world data integration preparation
- Learning progression support (jobs → territories → languages)

**Copilot Prompt**:
```
@workspace Please solve GitHub Issue #3 for core domain models.

Issue URL: https://github.com/victorsaly/WorldLeadersGame/issues/3

Key focus:
- Create educational game domain models
- Follow exact specifications in .github/copilot-instructions.md
- Player, Territory, JobLevel, AgentType models
- Child-appropriate design patterns

Generate all domain models with proper educational game structure.
```

### Issue #4: Setup Entity Framework with PostgreSQL
**URL**: https://github.com/victorsaly/WorldLeadersGame/issues/4

**Objective**: Configure Entity Framework Core with PostgreSQL for educational game data persistence.

**Database Design Requirements**:
- GameDbContext with educational focus
- Entity configurations for all domain models
- Audit fields (CreatedAt, UpdatedAt) for tracking
- Soft delete patterns for data preservation
- Territory seed data with real GDP information

**Educational Database Patterns**:
- Learning progression tracking capabilities
- Safe data handling for 12-year-old users
- Real-world data integration support
- Performance optimization for young users

**Copilot Prompt**:
```
@workspace Solve GitHub Issue #4 for Entity Framework setup.

Issue URL: https://github.com/victorsaly/WorldLeadersGame/issues/4

Requirements:
- Configure EF Core with PostgreSQL
- Educational game database patterns
- Follow .github/copilot-instructions.md database conventions
- Territory seed data with real GDP information

Please create complete database configuration for the educational game.
```

### Issue #5: Configure .NET Aspire Orchestration
**URL**: https://github.com/victorsaly/WorldLeadersGame/issues/5

**Objective**: Set up .NET Aspire orchestration in AppHost for local development and service discovery.

**Orchestration Requirements**:
- PostgreSQL database service registration
- Redis for session state and caching
- Web application (Blazor Server) configuration
- API application service discovery
- Health checks for all services
- Development environment optimization

**Educational Game Support**:
- AI service integration preparation
- Real-time SignalR communication setup
- Child-safe service configurations
- Performance optimization for educational content

**Copilot Prompt**:
```
@workspace Complete Phase 1 by solving GitHub Issue #5.

Issue URL: https://github.com/victorsaly/WorldLeadersGame/issues/5

Final requirements:
- .NET Aspire orchestration setup
- Local development environment
- Educational game service configuration
- Ready for Phase 2 development

Please configure Aspire orchestration following educational game patterns.
```

## 🔄 Issues-Driven Workflow

### Step-by-Step Process

1. **Start with Issue #1**: Begin with solution structure using the provided Copilot prompt
2. **Validate Completion**: Ensure solution compiles and meets acceptance criteria
3. **Close Issue**: Use `gh issue close #1` with completion summary
4. **Commit Changes**: Follow educational game commit message patterns
5. **Proceed to Next Issue**: Continue with Issue #2 using specific prompt

### Quality Checkpoints

After each issue resolution:
- [ ] **Build Verification**: Solution compiles without errors
- [ ] **Pattern Compliance**: Follows educational game development patterns
- [ ] **Child Safety**: Implements age-appropriate considerations
- [ ] **Documentation**: Code includes proper XML documentation
- [ ] **Performance**: Meets educational game performance targets

### Issue Management Commands

**List all issues**:
```bash
gh issue list
```

**View specific issue**:
```bash
gh issue view 1
```

**Close completed issue**:
```bash
gh issue close 1 --comment "✅ Solution structure completed successfully"
```

**Create new issue** (for future phases):
```bash
gh issue create --title "Phase 2.1: Core Game Engine" --body "Detailed requirements..."
```

## 🎯 Benefits of Issues-Driven Development

### Granular Control
- **Specific Tasks**: Each issue focuses on a single, manageable component
- **Clear Requirements**: Comprehensive specifications in every issue
- **Quality Gates**: Test and validate after each issue completion

### AI Context Enhancement
- **Rich Specifications**: Issues provide detailed context for Copilot
- **Comprehensive Instructions**: Reference to `.github/copilot-instructions.md`
- **Educational Focus**: Child-appropriate development patterns in every prompt

### Progress Tracking
- **Visual Progress**: GitHub Issues board shows completion status
- **Documentation**: Issue history creates development audit trail
- **Team Collaboration**: Multiple developers can work on different issues

### Quality Assurance
- **Incremental Testing**: Validate after each issue before proceeding
- **Pattern Consistency**: Educational game patterns enforced in every issue
- **Safety Compliance**: Child safety considerations in all requirements

## 🚀 Getting Started

**Begin with Issue #1 now using this exact prompt in VS Code Copilot Chat:**

```
@workspace I need to solve GitHub Issue #1 for the World Leaders Game educational project. 

Please read the issue details at: https://github.com/victorsaly/WorldLeadersGame/issues/1

Key requirements:
- Follow comprehensive instructions in .github/copilot-instructions.md
- Create .NET Aspire solution structure
- Target audience: 12-year-old educational game players
- Educational game development patterns required

Please generate the complete solution following the educational game patterns in the Copilot instructions.
```

## 📈 Expected Outcomes

After completing all 5 GitHub Issues:

**Technical Foundation**:
- ✅ Complete .NET Aspire solution with 5 projects
- ✅ All required packages installed and compatible
- ✅ Educational game domain models implemented
- ✅ Database configured with real-world territory data
- ✅ Local development environment fully functional

**Educational Readiness**:
- ✅ Child-safe development patterns established
- ✅ AI agent integration foundation prepared
- ✅ Real-world data integration capabilities
- ✅ Performance optimization baselines met

**Development Efficiency**:
- ✅ 80-90% code generation through AI assistance
- ✅ Consistent educational game patterns throughout
- ✅ Comprehensive documentation automatically generated
- ✅ Ready for Phase 2: Core Game Engine development

The Issues-Driven approach with GitHub Copilot creates a powerful development workflow that maintains educational focus while leveraging AI automation for rapid, high-quality code generation.
