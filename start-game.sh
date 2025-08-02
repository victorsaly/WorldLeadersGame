#!/bin/bash

# 🎮 World Leaders Game - Manual Startup Script
# For users who cannot install .NET Aspire workload due to permissions

echo "🎮 Starting World Leaders Game (Manual Mode)..."
echo "📍 Working directory: $(pwd)"

# Check if we're in the right directory
if [ ! -d "src/WorldLeaders" ]; then
    echo "❌ Error: Please run this script from the ConquerTheWorldGame root directory"
    echo "   Current directory: $(pwd)"
    echo "   Expected structure: src/WorldLeaders/"
    exit 1
fi

# Navigate to source directory
cd src/WorldLeaders

# Check Docker is running
echo "🐳 Checking Docker..."
if ! docker info > /dev/null 2>&1; then
    echo "❌ Error: Docker is not running. Please start Docker Desktop first."
    exit 1
fi

# Stop and remove existing container if it exists
echo "🧹 Cleaning up existing database container..."
docker stop worldleaders-postgres 2>/dev/null || true
docker rm worldleaders-postgres 2>/dev/null || true

# Start PostgreSQL
echo "🐘 Starting PostgreSQL database..."
docker run --name worldleaders-postgres \
  -e POSTGRES_DB=worldleaders \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:15

if [ $? -ne 0 ]; then
    echo "❌ Error: Failed to start PostgreSQL container"
    exit 1
fi

# Wait for database to be ready
echo "⏳ Waiting for database to be ready..."
sleep 8

# Check if database is responding
echo "🔍 Testing database connection..."
for i in {1..10}; do
    if docker exec worldleaders-postgres pg_isready -U postgres -d worldleaders > /dev/null 2>&1; then
        echo "✅ Database is ready!"
        break
    fi
    if [ $i -eq 10 ]; then
        echo "❌ Error: Database did not start properly"
        docker logs worldleaders-postgres
        exit 1
    fi
    echo "   Attempt $i/10..."
    sleep 2
done

# Build the solution
echo "🔨 Building solution..."
dotnet build
if [ $? -ne 0 ]; then
    echo "❌ Error: Build failed"
    exit 1
fi

echo ""
echo "🎯 Database started successfully!"
echo "📍 PostgreSQL: localhost:5432"
echo "🔑 Database: worldleaders"
echo "👤 User: postgres"
echo "🔐 Password: postgres"
echo ""
echo "🚀 Now run these commands in separate terminals:"
echo ""
echo "   Terminal 1 (API):"
echo "   cd $(pwd)/WorldLeaders.API"
echo "   export ConnectionStrings__DefaultConnection=\"Server=localhost;Database=worldleaders;User Id=postgres;Password=postgres;\""
echo "   export ASPNETCORE_ENVIRONMENT=Development"
echo "   dotnet run"
echo ""
echo "   Terminal 2 (Web App):"
echo "   cd $(pwd)/WorldLeaders.Web"
echo "   export ASPNETCORE_ENVIRONMENT=Development"
echo "   dotnet run"
echo ""
echo "🌐 Once running, access:"
echo "   • Game Web App: https://localhost:7154"
echo "   • Game API: https://localhost:7155"
echo "   • API Docs: https://localhost:7155/swagger"
echo ""
echo "🛑 To stop everything:"
echo "   docker stop worldleaders-postgres"
echo "   docker rm worldleaders-postgres"
echo ""
echo "✨ Happy coding!"
