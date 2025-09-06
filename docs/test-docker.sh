#!/bin/bash

# test-docker.sh - Docker-based Jekyll testing for World Leaders Game docs
# Uses exact GitHub Pages environment (Jekyll 3.9.3)

echo "🐳 Starting Jekyll documentation server with Docker..."
echo "📁 Working directory: $(pwd)"

# Check if Docker is available
if ! command -v docker &> /dev/null; then
    echo "❌ Docker not found. Please install Docker Desktop from docker.com"
    exit 1
fi

# Check if Docker daemon is running
if ! docker info &> /dev/null; then
    echo "❌ Docker daemon not running. Please start Docker Desktop"
    exit 1
fi

echo "✅ Docker is available and running"

# Stop any existing Jekyll containers on port 4000
echo "🔄 Cleaning up any existing Jekyll containers..."
docker ps -q --filter "publish=4000" | xargs -r docker stop

# Start Jekyll development server
echo "🚀 Starting Jekyll server..."
echo "📖 Documentation will be available at: http://localhost:4000/"
echo "⏹️  Press Ctrl+C to stop the server"
echo ""

docker run --rm \
  --name jekyll-docs \
  --volume="$PWD:/srv/jekyll:Z" \
  --publish 4000:4000 \
  jekyll/jekyll:3.9 \
  jekyll serve --watch --force_polling --host 0.0.0.0 --baseurl ""
