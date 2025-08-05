#!/bin/bash

# Simple Local Testing for Jekyll Documentation
# Works with any Ruby installation

echo "🚀 World Leaders Game - Simple Jekyll Test"
echo "========================================="
echo ""

# Check if we're in docs directory
if [ ! -f "_config.yml" ]; then
    echo "❌ Please run this from the docs directory:"
    echo "   cd docs && ./simple-test.sh"
    exit 1
fi

echo "✅ Found Jekyll configuration"

# Try to use system Jekyll first (if available)
if command -v jekyll &> /dev/null; then
    echo "✅ Using system Jekyll"
    echo "🌐 Starting server at http://localhost:4000/"
    echo "🛑 Press Ctrl+C to stop"
    echo ""
    jekyll serve --host 0.0.0.0 --port 4000 --baseurl ""
elif command -v bundle &> /dev/null; then
    echo "✅ Using Bundler"
    echo "📦 Installing dependencies..."
    bundle config set --local path 'vendor/bundle'
    if bundle install; then
        echo "🌐 Starting server at http://localhost:4000/"
        echo "🛑 Press Ctrl+C to stop"
        echo ""
        bundle exec jekyll serve --host 0.0.0.0 --port 4000 --baseurl ""
    else
        echo "❌ Bundle install failed. Try Docker method instead:"
        echo "   ./test-docker.sh"
    fi
else
    echo "❌ No Jekyll or Bundle found. Options:"
    echo ""
    echo "1. Install Ruby and Jekyll:"
    echo "   gem install jekyll bundler"
    echo ""
    echo "2. Use Docker (recommended):"
    echo "   ./test-docker.sh"
    echo ""
    echo "3. Use GitHub Codespaces (cloud-based)"
    echo "   No local setup required"
fi
