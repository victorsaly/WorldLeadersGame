#!/bin/bash
# Setup GitHub Token for Issue Creation
# This script helps you securely store your GitHub token

echo "🔑 GitHub Token Setup for WorldLeadersGame Issue Creation"
echo "=================================================="

# Check if GitHub CLI is installed
if ! command -v gh &> /dev/null; then
    echo "❌ GitHub CLI (gh) is not installed."
    echo "📦 Installing GitHub CLI..."
    
    # Install GitHub CLI on macOS
    if command -v brew &> /dev/null; then
        brew install gh
    else
        echo "❌ Homebrew not found. Please install GitHub CLI manually:"
        echo "   https://cli.github.com/"
        exit 1
    fi
fi

echo "✅ GitHub CLI is available"

# Option 1: Use GitHub CLI authentication (Recommended)
echo ""
echo "🚀 RECOMMENDED: Use GitHub CLI built-in authentication"
echo "This is the most secure method:"
echo ""
echo "1. Run: gh auth login"
echo "2. Choose: GitHub.com"
echo "3. Choose: HTTPS"
echo "4. Choose: Login with a web browser"
echo "5. Copy the one-time code and press Enter"
echo "6. Complete authentication in browser"
echo ""

read -p "Do you want to use GitHub CLI authentication? (y/n): " use_gh_auth

if [[ $use_gh_auth == "y" || $use_gh_auth == "Y" ]]; then
    echo "🔐 Starting GitHub CLI authentication..."
    gh auth login --hostname github.com --protocol https --web
    
    # Test authentication
    if gh auth status &> /dev/null; then
        echo "✅ GitHub CLI authentication successful!"
        echo "🎯 You can now run the PowerShell script without a token:"
        echo "   pwsh ./create-github-issues.ps1 -GitHubToken \$(gh auth token)"
        echo ""
        echo "Or convert to bash script for easier execution."
        exit 0
    else
        echo "❌ GitHub CLI authentication failed"
        echo "Falling back to manual token setup..."
    fi
fi

# Option 2: Manual token setup
echo ""
echo "🔑 ALTERNATIVE: Manual GitHub Token Setup"
echo "============================================"
echo ""
echo "1. Go to: https://github.com/settings/tokens"
echo "2. Click 'Generate new token (classic)'"
echo "3. Note: 'WorldLeadersGame Issue Creation'"
echo "4. Select scopes: 'repo' (full repository access)"
echo "5. Copy the generated token"
echo ""

read -p "Enter your GitHub Personal Access Token: " -s github_token
echo ""

if [[ -z "$github_token" ]]; then
    echo "❌ No token provided. Exiting..."
    exit 1
fi

# Validate token format (should start with ghp_ for personal access tokens)
if [[ ! $github_token =~ ^gh[ps]_ ]]; then
    echo "⚠️  Warning: Token doesn't start with 'ghp_' or 'ghs_'. Please verify it's correct."
fi

# Create .env file in scripts directory
ENV_FILE="$(dirname "$0")/.env"
echo "GITHUB_TOKEN=$github_token" > "$ENV_FILE"

# Secure the .env file
chmod 600 "$ENV_FILE"

echo "✅ Token saved to $ENV_FILE"
echo "🔒 File permissions set to 600 (owner read/write only)"

# Add .env to .gitignore if not already there
GITIGNORE_FILE="$(dirname "$0")/../.gitignore"
if ! grep -q "scripts/\.env" "$GITIGNORE_FILE" 2>/dev/null; then
    echo "scripts/.env" >> "$GITIGNORE_FILE"
    echo "✅ Added .env to .gitignore"
fi

# Test the token
echo ""
echo "🧪 Testing GitHub token..."
export GITHUB_TOKEN="$github_token"

if gh auth status --hostname github.com &> /dev/null; then
    echo "✅ Token validation successful!"
    echo ""
    echo "🎯 Ready to create issues! Run:"
    echo "   source ./scripts/.env"
    echo "   pwsh ./scripts/create-github-issues.ps1 -GitHubToken \$GITHUB_TOKEN"
    echo ""
    echo "Or use the bash version (I can create this for you)."
else
    echo "❌ Token validation failed. Please check your token."
    echo "   Make sure you selected 'repo' scope when creating the token."
fi
