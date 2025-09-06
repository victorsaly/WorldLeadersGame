#!/bin/bash

# simple-test.sh - Auto-detect and run Jekyll testing
# Tries multiple methods to start Jekyll development server

echo "🧪 World Leaders Game Documentation Testing"
echo "🔍 Auto-detecting available Jekyll setup..."

# Function to check if command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to check if port is available
port_available() {
    ! lsof -Pi :4000 -sTCP:LISTEN -t >/dev/null
}

# Check if port 4000 is available
if ! port_available; then
    echo "⚠️  Port 4000 is already in use"
    echo "🔧 Attempting to free port 4000..."
    lsof -ti:4000 | xargs kill -9 2>/dev/null || true
    sleep 2
    if ! port_available; then
        echo "❌ Could not free port 4000. Please stop any running Jekyll servers and try again."
        exit 1
    fi
fi

# Method 1: Try Docker (most reliable)
if command_exists docker && docker info &> /dev/null; then
    echo "✅ Docker available - using Docker method (recommended)"
    exec ./test-docker.sh
fi

# Method 2: Try bundle exec jekyll (if Bundler setup exists)
if command_exists bundle && [ -f "Gemfile" ]; then
    echo "✅ Bundler available - using bundle exec method"
    echo "📦 Installing/updating gems..."
    bundle config set --local path 'vendor/bundle'
    if bundle install; then
        echo "🚀 Starting Jekyll server..."
        echo "📖 Documentation will be available at: http://localhost:4000/"
        echo "⏹️  Press Ctrl+C to stop the server"
        bundle exec jekyll serve --baseurl ""
        exit 0
    else
        echo "❌ Bundle install failed"
    fi
fi

# Method 3: Try system Jekyll
if command_exists jekyll; then
    echo "✅ System Jekyll available - using system method"
    echo "🚀 Starting Jekyll server..."
    echo "📖 Documentation will be available at: http://localhost:4000/"
    echo "⏹️  Press Ctrl+C to stop the server"
    jekyll serve --baseurl ""
    exit 0
fi

# If nothing works
echo "❌ No suitable Jekyll setup found"
echo ""
echo "🔧 Available solutions:"
echo "1. Install Docker Desktop (recommended): https://docker.com"
echo "2. Install Ruby and Bundler: brew install ruby && gem install bundler"
echo "3. Install Jekyll system-wide: gem install jekyll"
echo ""
echo "💡 Docker is the easiest option and matches GitHub Pages exactly"
