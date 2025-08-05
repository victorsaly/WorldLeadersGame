#!/bin/bash

# Simple Jekyll Testing Script - Docker Version
# This script uses Docker to test Jekyll without Ruby setup issues

set -e  # Exit on any error

echo "🚀 World Leaders Game - Jekyll Testing (Docker)"
echo "==============================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Check if Docker is available
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed or not running."
    echo ""
    echo "Please install Docker from: https://www.docker.com/products/docker-desktop"
    echo ""
    echo "Alternative: Install Ruby and Jekyll manually (see LOCAL-TESTING.md)"
    exit 1
fi

print_success "Docker found!"

# Check if we're in the right directory
if [ ! -f "_config.yml" ]; then
    print_error "Not in docs directory! Please run this script from the docs folder."
    echo "Usage: cd docs && ./test-docker.sh"
    exit 1
fi

print_status "Testing Jekyll site with Docker..."

# Create .gitignore if it doesn't exist
if [ ! -f ".gitignore" ]; then
    echo "Creating .gitignore..."
    cat > .gitignore << EOF
_site/
.sass-cache/
.jekyll-cache/
.jekyll-metadata
vendor/
.bundle/
*.gem
.DS_Store
EOF
fi

print_status "Building and serving Jekyll site..."
echo ""
print_success "🎉 Starting Jekyll with Docker..."
echo ""
echo -e "${YELLOW}📝 Open your browser to: http://localhost:4000/${NC}"
echo -e "${YELLOW}🛑 Press Ctrl+C to stop the server${NC}"
echo ""

# Run Jekyll in Docker
docker run --rm \
  --volume="$PWD:/srv/jekyll:Z" \
  --publish 4000:4000 \
  jekyll/jekyll:3.9 \
  jekyll serve --watch --force_polling --host 0.0.0.0 --baseurl ""
