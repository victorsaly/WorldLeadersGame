---
layout: page
title: "File Organization and Documentation Structure"
date: 2025-08-05
category: "technical-guide"
tags: ["organization", "documentation", "structure", "maintainability"]
author: "AI-Generated with Human Oversight"
---

# 📁 File Organization and Documentation Structure

## 🎯 Educational Objective

Demonstrate professional project organization and documentation structure for educational technology development.

## 🌍 Real-World Connection

This organization mirrors industry-standard practices for maintainable, scalable educational software projects.

## 🗂️ Complete Project Structure

### Root Directory (Development Essentials)

```
/
├── README.md                    # Main project documentation
├── .gitignore                   # Version control exclusions
├── .env.production.template     # Environment configuration template
├── start-game.sh               # Quick game startup script
├── calculate-costs.sh          # Azure cost calculation
├── test-azure-ai.sh           # AI service testing
└── azure-ai-config.json       # AI service configuration
```

### Infrastructure (Deployment & Operations)

```
infrastructure/
├── azure-deploy.bicep          # Main deployment template
├── azure-deploy-simple.bicep   # Simplified deployment option
└── azure-appgateway.bicep     # Application Gateway configuration
```

### Scripts (Automation & Deployment)

```
scripts/
├── README.md                   # Scripts documentation
├── azure-setup.sh             # Infrastructure setup
├── deploy-azure.sh            # Application deployment
├── azure-domain-setup.sh      # Domain configuration
├── cloudflare-auto-setup.sh   # DNS automation
├── test-azure-setup.sh        # Deployment validation
└── [other deployment scripts]
```

### Source Code (Application)

```
src/
├── README.md                   # Source code documentation
└── WorldLeaders/              # .NET solution
    ├── WorldLeaders.AppHost/   # .NET Aspire orchestration
    ├── WorldLeaders.Web/       # Blazor Server application
    ├── WorldLeaders.API/       # ASP.NET Core API
    ├── WorldLeaders.Shared/    # Shared models and DTOs
    └── WorldLeaders.Infrastructure/ # Data access layer
```

### Documentation (Jekyll Site)

```
docs/
├── _config.yml                 # Jekyll configuration
├── Gemfile                     # Ruby dependencies
├── index.md                    # Documentation homepage
├── CNAME                       # Custom domain configuration
├── _layouts/                   # Jekyll page templates
├── _includes/                  # Reusable components
├── _data/                      # Structured data files
├── _posts/                     # Blog posts and development updates
├── _journey/                   # Week-by-week development logs
├── _technical/                 # Technical implementation guides
├── _milestones/               # Project milestone documentation
├── _issues/                   # Issue tracking and resolution
├── assets/                    # Images, CSS, JavaScript
└── [additional documentation pages]
```

## 📚 Documentation Categories

### Technical Guides (`_technical/`)

- **azure-deployment-comprehensive-guide.md** - Complete Azure deployment process
- **complete-azure-deployment-guide.md** - Step-by-step deployment instructions
- **azure-cloudflare-dns-configuration.md** - DNS and CDN setup
- **github-pages-cloudflare-setup.md** - Documentation hosting setup
- **cloudflare-dns-comprehensive-setup.md** - Advanced DNS configuration
- **domain-structure-migration-guide.md** - Domain architecture decisions
- **ai-agent-personality-system.md** - AI personality implementation
- **ai-prompt-engineering.md** - Educational AI prompt design
- **production-readiness-summary.md** - Production deployment checklist

### Development Journey (`_journey/`)

- **week-01-planning.md** - Initial project planning and voice memo analysis
- **week-02-foundation.md** - Technical foundation and architecture
- **week-03-game-mechanics.md** - Core gameplay implementation
- **week-04-ai-integration-real-world-learning.md** - AI agents and production deployment

### Blog Posts (`_posts/`)

- Development methodology posts
- Feature implementation explanations
- AI collaboration insights
- Educational technology analysis

### Project Milestones (`_milestones/`)

- **milestone-01-architecture.md** - Technical foundation completion
- **milestone-02-documentation-infrastructure.md** - Documentation system setup
- **milestone-03-core-gameplay.md** - Game mechanics implementation

## 🔄 Migration Summary

### Files Moved from Root to `docs/_technical/`

- `AZURE-DEPLOYMENT.md` → `azure-deployment-comprehensive-guide.md`
- `GITHUB-PAGES-SETUP.md` → `github-pages-cloudflare-setup.md`
- `cloudflare-dns-setup.md` → `cloudflare-dns-comprehensive-setup.md`
- `DOMAIN-STRUCTURE-UPDATE.md` → `domain-structure-migration-guide.md`

### Files Moved to Appropriate Directories

- `_config.yml` → `docs/_config.yml` (Jekyll configuration)
- `Gemfile` → `docs/Gemfile` (Ruby dependencies)
- `assets/` → `docs/assets/` (Documentation assets)
- `azure-*.bicep` → `infrastructure/` (Infrastructure as Code)

### Scripts Organized

- All deployment scripts moved to `scripts/` directory
- Comprehensive documentation added to `scripts/README.md`

## 🎯 Benefits of This Organization

### 🔧 Technical Benefits

- **Clear Separation**: Code, infrastructure, documentation, and scripts separated
- **Maintainability**: Easy to find and update specific components
- **Scalability**: Structure supports project growth
- **Professional Standards**: Industry-standard organization patterns

### 📚 Documentation Benefits

- **Jekyll Integration**: All documentation in proper Jekyll structure
- **Cross-References**: Easy linking between related documentation
- **Mobile-Friendly**: Proper asset organization for responsive design
- **SEO Optimization**: Structured content for search engines

### 🎮 Educational Benefits

- **Learning Path**: Clear progression through development journey
- **Reference Material**: Comprehensive technical guides
- **Methodology Documentation**: AI-first development process
- **Real-World Skills**: Professional project organization

## 🌐 Live Documentation URLs

All documentation is now properly organized and accessible at:

- **📚 Main Documentation**: https://docs.worldleadersgame.co.uk
- **🔧 Technical Guides**: https://docs.worldleadersgame.co.uk/technical-docs
- **📖 Development Journey**: https://docs.worldleadersgame.co.uk/journey
- **📝 Blog Posts**: https://docs.worldleadersgame.co.uk/blog
- **🎯 Milestones**: https://docs.worldleadersgame.co.uk/milestones

## 📋 Naming Conventions Applied

### File Naming Standards

- **Collections**: `lowercase-with-hyphens.md`
- **Root Pages**: `lowercase-with-hyphens.md`
- **Special Files**: `UPPERCASE.md` (README, CNAME, etc.)
- **Scripts**: `lowercase-with-hyphens.sh`

### Directory Structure

- **Source Code**: `PascalCase` for .NET conventions
- **Documentation**: `lowercase` with underscores for Jekyll collections
- **Scripts**: `lowercase` for shell script conventions
- **Infrastructure**: `lowercase` for deployment tools

## 🛡️ Child Safety in Organization

Even our file organization prioritizes child safety:

- **Clear Structure**: Easy navigation for educational content
- **Safe Separation**: Development tools separated from educational content
- **Documented Processes**: Transparent development for educational oversight
- **Professional Standards**: Teaching real-world software organization

---

**Remember**: This organization structure serves as an educational example of how modern software projects should be organized for maintainability, scalability, and educational transparency.
