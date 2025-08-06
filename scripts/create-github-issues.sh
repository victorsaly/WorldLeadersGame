#!/bin/bash
# Create GitHub Issues for Week 5 Production Security Issues
# Enhanced with .NET 8, UK South, Per-User Cost Tracking, and Documentation Pipeline

# Configuration
REPOSITORY="victorsaly/WorldLeadersGame"
MILESTONE_TITLE="Milestone 5: Production Security & Authentication"
MILESTONE_DUE_DATE="2025-08-20T23:59:59Z"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
MAGENTA='\033[0;35m'
CYAN='\033[0;36m'
WHITE='\033[1;37m'
NC='\033[0m' # No Color

echo -e "${CYAN}🔧 Setting up GitHub CLI for repository: $REPOSITORY${NC}"

# Check if GitHub CLI is authenticated
if ! gh auth status --hostname github.com &> /dev/null; then
    echo -e "${RED}❌ GitHub CLI not authenticated. Please run:${NC}"
    echo -e "${YELLOW}   gh auth login${NC}"
    exit 1
fi

echo -e "${GREEN}✅ GitHub CLI configured and authenticated${NC}"

# Function to create GitHub issue
create_github_issue() {
    local title="$1"
    local body="$2"
    local labels="$3"
    local milestone="$4"
    
    echo -e "${YELLOW}Creating GitHub issue: $title${NC}"
    
    # Create the issue
    if gh issue create \
        --repo "$REPOSITORY" \
        --title "$title" \
        --body "$body" \
        --label "$labels" \
        --milestone "$milestone"; then
        echo -e "${GREEN}✅ Created: $title${NC}"
        return 0
    else
        echo -e "${RED}❌ Failed to create: $title${NC}"
        return 1
    fi
}

# Create Milestone 5 first
echo -e "\n${MAGENTA}🎯 Creating Milestone 5: Production Security & Authentication${NC}"

milestone_description="## Week 5: Production Security & Authentication Platform

Complete production security implementation for UK educational platform with .NET 8 optimizations and per-user cost tracking.

### Success Criteria
- [ ] All 5 production security issues completed (5.1-5.5)
- [ ] .NET 8 optimizations implemented across platform
- [ ] UK South region deployment operational
- [ ] Per-user cost tracking achieving £0.08/user/day
- [ ] COPPA/GDPR compliance validated
- [ ] 1000+ concurrent user capacity verified
- [ ] Zero-downtime deployment pipeline operational
- [ ] Child safety validation 100% operational
- [ ] Documentation pipeline generating LinkedIn/Medium articles

### Educational Impact
- Enhanced platform security protecting 1000+ children
- UK GDPR compliance for educational institutions
- Cost-effective scaling within educational budgets
- Professional knowledge sharing through documentation

### Technical Achievements
- .NET 8 performance improvements (25% faster)
- Azure UK South regional compliance
- Automated security monitoring
- Per-user cost attribution system

**Due Date:** August 20, 2025
**Estimated Effort:** 40 hours
**Business Value:** Critical for production readiness"

# Create milestone using GitHub CLI
if milestone_result=$(gh api --method POST \
    -H "Accept: application/vnd.github+json" \
    "/repos/$REPOSITORY/milestones" \
    -f title="$MILESTONE_TITLE" \
    -f description="$milestone_description" \
    -f due_on="$MILESTONE_DUE_DATE" 2>/dev/null); then
    milestone_number=$(echo "$milestone_result" | jq -r '.number')
    echo -e "${GREEN}✅ Created Milestone 5 with number: $milestone_number${NC}"
else
    echo -e "${RED}❌ Failed to create milestone (it might already exist)${NC}"
    # Try to get existing milestone
    if existing_milestone=$(gh api "/repos/$REPOSITORY/milestones" | jq -r '.[] | select(.title=="'"$MILESTONE_TITLE"'") | .number' 2>/dev/null); then
        milestone_number="$existing_milestone"
        echo -e "${YELLOW}⚠️  Using existing milestone: $milestone_number${NC}"
    else
        milestone_number=""
        echo -e "${YELLOW}⚠️  Continuing without milestone assignment${NC}"
    fi
fi

# Issue 5.1: API Security & Authentication
echo -e "\n${CYAN}🔐 Creating Issue 5.1: API Security & Authentication${NC}"

issue_51_body="## 🎯 Enhanced Objectives (.NET 8 + UK South Focus)

**AI-Generated Image Prompt**: \"Secure authentication system for children using tablets, Azure AD B2C interface, UK data protection symbols, child-safe security design, educational platform login screen\"

### Enhanced .NET 8 Implementation
- [ ] JWT authentication with primary constructors for streamlined security service initialization
- [ ] Azure AD B2C integration in UK South region for educational data residency
- [ ] Per-user cost tracking (£0.08/user/day) with granular attribution
- [ ] Record-based security configurations with immutable settings
- [ ] Child data protection validation with automated compliance checking

### Security Requirements
- [ ] COPPA/GDPR compliance for UK education sector with automated monitoring
- [ ] Role-based access with educational context (Student, Teacher, Admin)
- [ ] Child session management with safety controls and timeout protection
- [ ] Automated security monitoring with real-time threat detection

### UK South Regional Compliance
- [ ] Educational data residency requirements for UK schools
- [ ] GDPR Article 8 compliance for child data protection
- [ ] UK education sector security standards adherence
- [ ] Local data processing for reduced latency

## 🔧 Implementation Details

### .NET 8 JWT Service with Primary Constructors
\`\`\`csharp
// Enhanced JWT authentication service with .NET 8 features
public class JwtAuthenticationService(
    IConfiguration configuration,
    IUserManager<ApplicationUser> userManager,
    IChildSafetyValidator childSafetyValidator,
    IPerUserCostTracker costTracker) : IJwtAuthenticationService
{
    public required AzureAdB2CConfig B2CConfig { get; init; } = AzureAdB2CConfig.UKEducationalDefaults;
    public required string Region { get; init; } = \"UK South\";
    public required decimal MaxCostPerUser { get; init; } = 0.08m; // £0.08/user/day
}
\`\`\`

## 📚 Documentation Updates (Mandatory)
- [ ] **README.md**: Add authentication setup guide for UK educational deployment
- [ ] **docs/journey/week-05-production-security.md**: Document authentication implementation
- [ ] **LinkedIn article**: \"Child-Safe Authentication in Azure UK South: Protecting 1000+ Young Learners\"

## 🔗 Related Issues
- Blocks: Issue 5.2 (Performance), Issue 5.3 (Cost Management), Issue 5.4 (Security Hardening)
- Dependencies: Azure AD B2C tenant setup in UK South region

**Estimated Effort**: 8 hours  
**Priority**: Critical  
**Business Value**: 90/100"

create_github_issue \
    "Issue 5.1: API Security & Authentication - UK Educational Platform" \
    "$issue_51_body" \
    "week-5,security,authentication,dotnet8,uk-south,child-safety" \
    "$MILESTONE_TITLE"

# Issue 5.2: Performance & Scalability Optimization
echo -e "\n${CYAN}⚡ Creating Issue 5.2: Performance & Scalability Optimization${NC}"

issue_52_body="## 🎯 Enhanced Objectives (.NET 8 + UK South Focus)

**AI-Generated Image Prompt**: \"High-performance educational platform dashboard showing 1000+ concurrent users, .NET 8 performance metrics, children learning on tablets simultaneously, Azure UK South infrastructure\"

### Enhanced .NET 8 Performance Features
- [ ] Primary constructor optimizations for dependency injection efficiency
- [ ] Native AOT compilation where applicable for faster startup times
- [ ] Record types for lightweight data structures and reduced memory allocation
- [ ] Sub-2 second load times optimized for 12-year-old attention spans

### Scalability Targets (UK South Region)
- [ ] Support 1000+ concurrent users with UK-optimized infrastructure
- [ ] Multi-layer caching strategy (Memory + Redis + CDN) with UK data residency
- [ ] Auto-scaling App Service Plan in UK South with educational usage patterns
- [ ] Performance monitoring with Application Insights UK South instance

### Educational Platform Optimizations
- [ ] Child-friendly UI performance (large buttons, immediate feedback)
- [ ] Game state synchronization for real-time educational interactions
- [ ] Optimized AI agent response times (<3 seconds) for engagement
- [ ] Speech recognition performance tuning for pronunciation learning

## 🔧 Implementation Details

### .NET 8 Performance Service
\`\`\`csharp
// Enhanced performance optimization with .NET 8 primary constructors
public class PerformanceOptimizedGameComponent(
    IMemoryCache memoryCache,
    IDistributedCache distributedCache,
    IApplicationInsights appInsights) : IGameComponent
{
    public required PerformanceConfig Config { get; init; } = PerformanceConfig.UKEducationalDefaults;
    public required string Region { get; init; } = \"UK South\";
}
\`\`\`

## 📚 Documentation Updates (Mandatory)
- [ ] **README.md**: Add performance optimization guide for educational platforms
- [ ] **docs/journey/week-05-production-security.md**: Document scaling achievements
- [ ] **LinkedIn article**: \"Scaling Educational Platforms: .NET 8 Performance in Azure UK South\"

**Estimated Effort**: 6 hours  
**Priority**: High  
**Business Value**: 85/100"

create_github_issue \
    "Issue 5.2: Performance & Scalability Optimization - 1000+ Users" \
    "$issue_52_body" \
    "week-5,performance,scalability,dotnet8,optimization,caching" \
    "$MILESTONE_TITLE"

# Issue 5.3: Azure Cost Management & Monitoring
echo -e "\n${CYAN}💰 Creating Issue 5.3: Azure Cost Management & Monitoring${NC}"

issue_53_body="## 🎯 Enhanced Objectives (.NET 8 + UK South Focus)

**AI-Generated Image Prompt**: \"Azure cost management dashboard with UK educational budget tracking, per-student cost meters, pound sterling symbols, educational efficiency graphs, school administration interface\"

### Per-User Cost Attribution System
- [ ] Real-time cost tracking per student (£0.08/user/day) with granular attribution
- [ ] .NET 8 record types for cost data structures with built-in comparison
- [ ] GBP-based budgeting optimized for UK education sector
- [ ] Educational efficiency scoring per pound spent (target: 85+ points/£)

### Budget Controls & Monitoring
- [ ] Automated alerts at 80% daily budget (£7.50) with educational context
- [ ] Emergency throttling to prevent overruns while maintaining learning continuity
- [ ] Predictive cost forecasting with machine learning for school usage patterns
- [ ] Azure Cost Management integration with UK South regional pricing

### UK Educational Compliance
- [ ] Educational institution budget reporting for transparency
- [ ] GDPR-compliant cost data handling with UK data residency
- [ ] Integration with Azure Education Hub for educational pricing
- [ ] Parent/school cost transparency dashboards

## 🔧 Implementation Details

### .NET 8 Cost Tracking Service
\`\`\`csharp
// Enhanced cost tracking with .NET 8 features
public class RealTimeCostTracker(
    IConfiguration configuration,
    IMemoryCache cache,
    TelemetryClient telemetryClient,
    IAzureCostManagementClient costClient) : IRealTimeCostTracker
{
    public required BudgetConfig BudgetConfig { get; init; } = BudgetConfig.UKEducationalDefaults;
    public required string Currency { get; init; } = \"GBP\";
}
\`\`\`

## 📚 Documentation Updates (Mandatory)
- [ ] **README.md**: Add cost management setup guide and per-user tracking
- [ ] **docs/journey/week-05-production-security.md**: Document cost optimization results
- [ ] **LinkedIn article**: \"Azure Cost Optimization for Educational Platforms: Mastering Per-User Attribution\"

**Estimated Effort**: 6 hours  
**Priority**: High  
**Business Value**: 80/100"

create_github_issue \
    "Issue 5.3: Azure Cost Management & Monitoring - Per-User Attribution" \
    "$issue_53_body" \
    "week-5,cost-management,budget-control,per-user-tracking,gbp,analytics" \
    "$MILESTONE_TITLE"

# Issue 5.4: Production Security Hardening
echo -e "\n${CYAN}🛡️ Creating Issue 5.4: Production Security Hardening & Compliance${NC}"

issue_54_body="## 🎯 Enhanced Objectives (.NET 8 + UK South Focus)

**AI-Generated Image Prompt**: \"Cyber security shield protecting children using tablets, UK flag elements, GDPR compliance symbols, educational safety icons, Azure Security Center dashboard\"

### Child Data Protection (UK Focus)
- [ ] COPPA/GDPR compliance with UK data residency for educational institutions
- [ ] End-to-end encryption using Azure Key Vault UK South for child data protection
- [ ] .NET 8 security optimizations with primary constructors for secure service initialization
- [ ] Automated compliance monitoring and reporting for UK education sector

### Advanced Security Measures
- [ ] Web Application Firewall tuned for educational content and child safety
- [ ] 24/7 security monitoring with child safety alerts and educational context
- [ ] Penetration testing and vulnerability assessment for educational platforms
- [ ] Incident response automation with child safety escalation procedures

### UK Educational Compliance
- [ ] UK education sector security standards (DfE guidelines)
- [ ] Safeguarding requirements for online child protection
- [ ] Data Protection Impact Assessment (DPIA) for educational processing
- [ ] Parent consent management and transparency controls

## 🔧 Implementation Details

### .NET 8 Security Service
\`\`\`csharp
// Enhanced child data protection service
public class ChildDataProtectionService(
    IKeyVaultClient keyVault,
    IAuditLogger auditLogger,
    IComplianceValidator validator) : IChildDataProtectionService
{
    public required ChildPrivacyConfig PrivacyConfig { get; init; } = ChildPrivacyConfig.UKStandards;
    public required string Region { get; init; } = \"UK South\";
}
\`\`\`

## 📚 Documentation Updates (Mandatory)
- [ ] **README.md**: Add production security checklist and compliance guide
- [ ] **docs/journey/week-05-production-security.md**: Document security hardening achievements
- [ ] **LinkedIn article**: \"Securing Educational Platforms: Enterprise-Grade Child Data Protection\"

**Estimated Effort**: 8 hours  
**Priority**: Critical  
**Business Value**: 95/100"

create_github_issue \
    "Issue 5.4: Production Security Hardening & Compliance - UK Educational" \
    "$issue_54_body" \
    "week-5,security-hardening,compliance,child-safety,gdpr,uk-region" \
    "$MILESTONE_TITLE"

# Issue 5.5: Infrastructure as Code & Deployment Pipeline
echo -e "\n${CYAN}🚀 Creating Issue 5.5: Infrastructure as Code & Automated Deployment${NC}"

issue_55_body="## 🎯 Enhanced Objectives (.NET 8 + UK South Focus)

**AI-Generated Image Prompt**: \"DevOps pipeline visualization with UK data centers, automated deployment flows, .NET 8 build processes, educational platform infrastructure, Azure UK South regions\"

### .NET 8 DevOps Pipeline
- [ ] Bicep templates for complete UK South infrastructure with educational compliance
- [ ] .NET 8 native AOT optimizations in build pipeline for faster deployments
- [ ] Zero-downtime blue-green deployments for educational continuity
- [ ] Automated testing with child safety validation and educational content verification

### Production Reliability (UK South)
- [ ] 99.9% uptime target for educational institutions with UK-optimized monitoring
- [ ] Automated rollback within 30 seconds to minimize learning disruption
- [ ] Health monitoring with educational context and child safety validation
- [ ] Disaster recovery with UK data residency and educational continuity planning

### Infrastructure Automation
- [ ] Environment provisioning (dev/staging/production) with UK compliance
- [ ] Security scanning integration with child data protection validation
- [ ] Performance testing with educational workload patterns
- [ ] Cost optimization automation with per-user budget enforcement

## 🔧 Implementation Details

### .NET 8 Deployment Service
\`\`\`csharp
// Enhanced deployment automation for educational platforms
public class EducationalDeploymentService(
    IInfrastructureProvisioner provisioner,
    IChildSafetyValidator safetyValidator,
    IComplianceChecker complianceChecker) : IDeploymentService
{
    public required UKComplianceConfig ComplianceConfig { get; init; } = UKComplianceConfig.Educational;
    public required string Region { get; init; } = \"UK South\";
}
\`\`\`

## 📚 Documentation Updates (Mandatory)
- [ ] **README.md**: Add infrastructure automation setup and deployment guide
- [ ] **docs/journey/week-05-production-security.md**: Document deployment pipeline achievements
- [ ] **LinkedIn article**: \"DevOps for Educational Platforms: Bulletproof .NET 8 Deployments\"

**Estimated Effort**: 6 hours  
**Priority**: High  
**Business Value**: 85/100"

create_github_issue \
    "Issue 5.5: Infrastructure as Code & Deployment Pipeline - UK Platform" \
    "$issue_55_body" \
    "week-5,infrastructure,devops,deployment,automation,reliability" \
    "$MILESTONE_TITLE"

# Summary and Next Steps
echo -e "\n${GREEN}🎉 Successfully created all GitHub issues for Week 5!${NC}"
echo -e "${CYAN}📊 Summary:${NC}"
echo -e "${WHITE}   • Milestone 5: Production Security & Authentication Platform${NC}"
echo -e "${WHITE}   • Issue 5.1: API Security & Authentication - UK Educational Platform${NC}"
echo -e "${WHITE}   • Issue 5.2: Performance & Scalability Optimization - 1000+ Users${NC}"
echo -e "${WHITE}   • Issue 5.3: Azure Cost Management & Monitoring - Per-User Attribution${NC}"
echo -e "${WHITE}   • Issue 5.4: Production Security Hardening & Compliance - UK Educational${NC}"
echo -e "${WHITE}   • Issue 5.5: Infrastructure as Code & Deployment Pipeline - UK Platform${NC}"

echo -e "\n${YELLOW}🚀 Next Steps:${NC}"
echo -e "${WHITE}   1. Review issues in GitHub web interface${NC}"
echo -e "${WHITE}   2. Assign team members to issues${NC}"
echo -e "${WHITE}   3. Set up GitHub Projects board for Week 5 tracking${NC}"
echo -e "${WHITE}   4. Begin implementation starting with Issue 5.1${NC}"
echo -e "${WHITE}   5. Track progress toward Milestone 5 completion${NC}"

echo -e "\n${CYAN}🔗 Repository Link:${NC}"
echo -e "${BLUE}   GitHub Issues: https://github.com/$REPOSITORY/issues${NC}"
if [[ -n "$milestone_number" ]]; then
    echo -e "${BLUE}   Milestone 5: https://github.com/$REPOSITORY/milestone/$milestone_number${NC}"
fi

echo -e "\n${MAGENTA}💡 Enhanced Features Included:${NC}"
echo -e "${WHITE}   • .NET 8 optimizations (primary constructors, record types, native AOT)${NC}"
echo -e "${WHITE}   • UK South region deployment with educational compliance${NC}"
echo -e "${WHITE}   • Per-user cost tracking (£0.08/user/day) with real-time attribution${NC}"
echo -e "${WHITE}   • Comprehensive documentation pipeline for LinkedIn/Medium articles${NC}"
echo -e "${WHITE}   • Child safety validation and COPPA/GDPR compliance${NC}"
echo -e "${WHITE}   • Production-ready infrastructure with 99.9% uptime target${NC}"
