---
layout: page
title: "Command Scripts Reference"
date: 2025-08-09
category: "reference-guide"
tags: ["scripts", "commands", "automation", "development"]
educational_objective: "Provide comprehensive reference for all project automation scripts"
---

# 🚀 Command Scripts Reference

**Purpose**: Complete reference guide for all automation scripts in the World Leaders Game project  
**Location**: All scripts are located in `/scripts/` directory  
**Usage**: Run from project root directory unless specified otherwise

---

## 📋 Quick Reference

### Most Used Commands
```bash
# Start the game
./scripts/start-game.sh

# Deploy to production
./scripts/fast-deploy.sh

# Generate blog images
./scripts/generate-blog-image.sh

# Validate documentation
./scripts/validate-documentation-completeness.sh
```

---

## 🎮 Game Development Scripts

### Game Startup & Management

#### `start-game.sh`
**Purpose**: Start the complete World Leaders Game using .NET Aspire orchestration  
**Usage**: `./scripts/start-game.sh`  
**Requirements**: .NET 8 SDK installed  
**Ports**: API (7155), Web (7154), Aspire Dashboard (15888)  

```bash
# Start the educational game
./scripts/start-game.sh

# Game will be available at:
# - Web App: https://localhost:7154
# - API: https://localhost:7155
# - Aspire Dashboard: https://localhost:15888
```

### Testing & Validation

#### `simple-test.sh`
**Purpose**: Basic application health checks  
**Usage**: `./scripts/simple-test.sh`  
**Checks**: Build status, basic functionality, endpoint availability

#### `test-docker.sh`
**Purpose**: Docker container testing and validation  
**Usage**: `./scripts/test-docker.sh`  
**Requirements**: Docker Desktop running

#### `verify-structure.sh`
**Purpose**: Validate project file structure and organization  
**Usage**: `./scripts/verify-structure.sh`  
**Checks**: Required directories, configuration files, documentation structure

---

## 🔧 Development & Build Scripts

### Code Quality & Validation

#### `validate-documentation-completeness.sh`
**Purpose**: Comprehensive validation of Week 6 retro transformation documentation  
**Usage**: `./scripts/validate-documentation-completeness.sh`  
**Validates**: 
- GitHub issues readiness
- Technical documentation completeness
- Educational objective preservation
- Child safety compliance

#### `validate-retro.sh`
**Purpose**: Validate retro 32-bit design compliance  
**Usage**: `./scripts/validate-retro.sh`  
**Checks**: 
- Green theme implementation
- Pixel art rendering settings
- Retro typography usage
- Component styling standards

#### `validate-pwa.sh`
**Purpose**: Progressive Web App standards validation  
**Usage**: `./scripts/validate-pwa.sh`  
**Validates**:
- Manifest.json structure
- Required icon sizes (72x72 to 512x512)
- Service worker implementation
- Educational branding consistency

#### `validate-education.sh`
**Purpose**: Educational value and child safety validation  
**Usage**: `./scripts/validate-education.sh`  
**Checks**:
- Age-appropriate content (12-year-olds)
- Learning objective implementation
- Cultural sensitivity compliance
- Child safety standards

---

## 🚀 Deployment Scripts

### Production Deployment

#### `fast-deploy.sh`
**Purpose**: Rapid deployment to Azure production environment  
**Usage**: `./scripts/fast-deploy.sh`  
**Environment**: Production (worldleadersgame.com)  
**Requirements**: Azure CLI authenticated, proper environment variables

```bash
# Deploy latest changes to production
./scripts/fast-deploy.sh

# Production URLs:
# - Main Site: https://worldleadersgame.com
# - API: https://api.worldleadersgame.com
```

#### `quick-deploy.sh`
**Purpose**: Quick deployment with minimal checks  
**Usage**: `./scripts/quick-deploy.sh`  
**Warning**: Use only for hotfixes, bypasses some validation

#### `deploy-azure.sh`
**Purpose**: Full Azure deployment with complete validation  
**Usage**: `./scripts/deploy-azure.sh`  
**Duration**: ~10-15 minutes  
**Includes**: Infrastructure setup, application deployment, DNS configuration

### Deployment Utilities

#### `restart-production.sh`
**Purpose**: Restart production services without redeployment  
**Usage**: `./scripts/restart-production.sh`  
**Use Case**: Clear caches, reload configuration, resolve temporary issues

#### `quick-cors-fix.sh`
**Purpose**: Emergency CORS configuration fix for production  
**Usage**: `./scripts/quick-cors-fix.sh`  
**Emergency**: Use when API access is blocked by CORS issues

---

## 🌐 Infrastructure & Configuration Scripts

### Azure Setup & Management

#### `azure-setup.sh`
**Purpose**: Complete Azure infrastructure setup from scratch  
**Usage**: `./scripts/azure-setup.sh`  
**Duration**: ~20-30 minutes  
**Creates**: Resource groups, App Services, databases, networking

#### `azure-preflight.sh`
**Purpose**: Pre-deployment Azure environment validation  
**Usage**: `./scripts/azure-preflight.sh`  
**Checks**: Authentication, resource availability, configuration validity

#### `azure-domain-setup.sh`
**Purpose**: Custom domain configuration for Azure App Services  
**Usage**: `./scripts/azure-domain-setup.sh`  
**Configures**: SSL certificates, DNS validation, domain binding

#### `test-azure-setup.sh`
**Purpose**: Validate Azure infrastructure post-deployment  
**Usage**: `./scripts/test-azure-setup.sh`  
**Tests**: Service connectivity, database access, external API integration

### Security & Authentication

#### `setup-azure-b2c.sh`
**Purpose**: Azure B2C identity provider setup (future authentication)  
**Usage**: `./scripts/setup-azure-b2c.sh`  
**Status**: Prepared for future user authentication features

#### `fix-azure-oidc-credential.sh`
**Purpose**: Fix Azure OpenID Connect credential issues  
**Usage**: `./scripts/fix-azure-oidc-credential.sh`  
**Use Case**: Resolve authentication pipeline problems

#### `test-azure-ai.sh`
**Purpose**: Test Azure AI service connectivity and authentication  
**Usage**: `./scripts/test-azure-ai.sh`  
**Tests**: OpenAI API, Speech Services, Content Moderation

#### `test-jwt-auth.sh`
**Purpose**: JWT authentication testing (future feature)  
**Usage**: `./scripts/test-jwt-auth.sh`  
**Status**: Prepared for user authentication implementation

---

## 🌍 Domain & DNS Management

### Cloudflare Integration

#### `cloudflare-auto-setup.sh`
**Purpose**: Automated Cloudflare CDN and DNS configuration  
**Usage**: `./scripts/cloudflare-auto-setup.sh`  
**Configures**: CDN caching, SSL, DNS records, security rules

#### `check-cloudflare-dns.sh`
**Purpose**: Validate Cloudflare DNS configuration  
**Usage**: `./scripts/check-cloudflare-dns.sh`  
**Checks**: DNS propagation, SSL status, CDN functionality

#### `configure-worldleadersgame-domain.sh`
**Purpose**: Specific domain configuration for worldleadersgame.com  
**Usage**: `./scripts/configure-worldleadersgame-domain.sh`  
**Configures**: Production domain with all subdomains

#### `setup-custom-domain.sh`
**Purpose**: Generic custom domain setup utility  
**Usage**: `./scripts/setup-custom-domain.sh [domain]`  
**Flexible**: Can configure any custom domain

---

## 🎨 Content & Asset Generation

### Blog & Marketing

#### `generate-blog-image.sh`
**Purpose**: AI-powered blog post image generation  
**Usage**: `./scripts/generate-blog-image.sh [blog-post-slug]`  
**Features**: Automatic image generation, multiple formats, social media optimization

```bash
# Generate image for specific blog post
./scripts/generate-blog-image.sh week-6-retro-transformation

# Generates:
# - LinkedIn format (1200x630)
# - Twitter format (1024x512)
# - Blog header format (1920x1080)
```

#### `create-github-issues.sh`
**Purpose**: Automated GitHub issue creation from templates  
**Usage**: `./scripts/create-github-issues.sh`  
**Creates**: Development issues, bug reports, feature requests

---

## 📊 Analytics & Monitoring

### Cost Management

#### `calculate-costs.sh`
**Purpose**: Azure resource cost calculation and optimization  
**Usage**: `./scripts/calculate-costs.sh`  
**Reports**: Monthly costs, resource utilization, optimization recommendations

```bash
# Get current month Azure costs
./scripts/calculate-costs.sh

# Example Output:
# Total Monthly Cost: $12.34
# App Service: $8.50
# Database: $2.84
# CDN: $1.00
```

### Health Monitoring

#### `check-and-configure.sh`
**Purpose**: System health check and auto-configuration  
**Usage**: `./scripts/check-and-configure.sh`  
**Checks**: Service health, configuration drift, performance metrics

---

## 🛠️ Development Utilities

### GitHub Integration

#### `setup-github-azure-secrets.sh`
**Purpose**: Configure GitHub Actions secrets for Azure deployment  
**Usage**: `./scripts/setup-github-azure-secrets.sh`  
**Configures**: Deployment secrets, API keys, service principal credentials

#### `setup-github-token.sh`
**Purpose**: GitHub API token configuration  
**Usage**: `./scripts/setup-github-token.sh`  
**Use Case**: Automated issue creation, repository management

#### `create-labels.sh`
**Purpose**: Create standardized GitHub issue labels  
**Usage**: `./scripts/create-labels.sh`  
**Creates**: Educational, development, retro, PWA, safety labels

---

## 🎯 Script Categories Summary

### 🟢 Daily Development
- `start-game.sh` - Start local development
- `simple-test.sh` - Quick health check
- `validate-documentation-completeness.sh` - Documentation validation

### 🔵 Deployment & Production
- `fast-deploy.sh` - Quick production deployment
- `deploy-azure.sh` - Full deployment with validation
- `restart-production.sh` - Production service restart

### 🟡 Infrastructure Setup
- `azure-setup.sh` - Complete Azure setup
- `cloudflare-auto-setup.sh` - CDN configuration
- `setup-custom-domain.sh` - Domain configuration

### 🟠 Content Generation
- `generate-blog-image.sh` - AI blog image generation
- `create-github-issues.sh` - Issue template creation

### 🔴 Emergency & Troubleshooting
- `quick-cors-fix.sh` - CORS emergency fix
- `fix-azure-oidc-credential.sh` - Authentication fix
- `check-and-configure.sh` - Health check and repair

---

## 📋 Usage Guidelines

### Running Scripts
```bash
# Make script executable (if needed)
chmod +x scripts/[script-name].sh

# Run from project root
./scripts/[script-name].sh

# Check script help (if available)
./scripts/[script-name].sh --help
```

### Environment Requirements
- **Azure CLI**: Required for deployment scripts
- **.NET 8 SDK**: Required for game development scripts
- **Docker**: Required for container testing scripts
- **Cloudflare CLI**: Required for CDN management scripts

### Error Handling
Most scripts include:
- ✅ Success/failure validation
- 📝 Detailed logging
- 🔄 Rollback capabilities
- 🛡️ Safety checks

---

## 🎓 Educational Context

All scripts support the educational mission of the World Leaders Game:
- **Child Safety**: Scripts validate age-appropriate content
- **Educational Integrity**: Deployment maintains learning objectives
- **Performance**: Scripts ensure <2 second load times for child engagement
- **Accessibility**: Validation includes WCAG 2.1 AA compliance

---

**Remember**: These scripts are part of an educational technology project. Every automation must support safe, effective learning experiences for 12-year-old students while maintaining professional development standards.
