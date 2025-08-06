#!/bin/bash
# Create missing labels for Week 5 issues

REPOSITORY="victorsaly/WorldLeadersGame"

echo "🏷️ Creating missing labels for Week 5 issues..."

create_label() {
    local name="$1"
    local color="$2"
    local description="$3"
    
    echo "Creating label: $name"
    if gh label create "$name" \
        --color "$color" \
        --description "$description" \
        --repo "$REPOSITORY" 2>/dev/null; then
        echo "✅ Created: $name"
    else
        echo "⚠️  Label $name might already exist or failed to create"
    fi
}

# Create all required labels
create_label "week-5" "1f77b4" "Week 5 production security tasks"
create_label "security" "d73a49" "Security and authentication features"
create_label "authentication" "e74c3c" "Authentication and authorization"
create_label "dotnet8" "512bd4" ".NET 8 specific features and optimizations"
create_label "uk-south" "0052cc" "UK South Azure region deployment"
create_label "performance" "2ecc71" "Performance optimization and scalability"
create_label "scalability" "27ae60" "Scalability improvements"
create_label "optimization" "f39c12" "Code and performance optimizations"
create_label "caching" "3498db" "Caching strategies and implementation"
create_label "cost-management" "e67e22" "Azure cost management and monitoring"
create_label "budget-control" "d35400" "Budget controls and alerts"
create_label "per-user-tracking" "8e44ad" "Per-user cost attribution"
create_label "gbp" "16a085" "UK Pound Sterling financial tracking"
create_label "analytics" "34495e" "Analytics and reporting"
create_label "security-hardening" "c0392b" "Production security hardening"
create_label "compliance" "8b4513" "COPPA/GDPR compliance"
create_label "gdpr" "2c3e50" "GDPR compliance specific"
create_label "uk-region" "0052cc" "UK regional compliance"
create_label "infrastructure" "7f8c8d" "Infrastructure and DevOps"
create_label "devops" "95a5a6" "DevOps and deployment"
create_label "deployment" "6c757d" "Deployment automation"
create_label "automation" "17a2b8" "Process automation"
create_label "reliability" "28a745" "System reliability and uptime"

echo ""
echo "🎯 All labels created! You can now run the issue creation script."
