# 🚀 Command Line Guide - World Leaders Game

_Comprehensive guide for running, building, and managing the World Leaders Game from the command line_

---

## 📋 Prerequisites

Before running the application, ensure you have the following installed:

### Required Software

```bash
# Check .NET version (requires .NET 8 SDK)
dotnet --version

# Check Docker version (for PostgreSQL)
docker --version

# Check Node.js version (for TailwindCSS, optional)
node --version
```

### Required Versions

- **.NET 8 SDK** or later
- **Docker Desktop** (for PostgreSQL database)
- **.NET Aspire workload** (required for orchestration)
- **Git** for version control
- **Visual Studio Code** or **Visual Studio 2022** (recommended for development)

### Install .NET Aspire Workload

```bash
# Install the Aspire workload (REQUIRED)
dotnet workload install aspire

# Verify Aspire workload is installed
dotnet workload list

# Update Aspire workload if needed
dotnet workload update aspire
```

---

## 🏗️ Project Setup Commands

### 1. Clone and Navigate

```bash
# Clone the repository
git clone https://github.com/victorsaly/WorldLeadersGame.git
cd WorldLeadersGame

# Navigate to source directory
cd src/WorldLeaders

# Verify project structure
ls -la
```

### 2. Restore Dependencies

```bash
# Restore all project dependencies
dotnet restore

# Verify all projects can build
dotnet build

# Check for any build issues
dotnet build --verbosity normal

# Note: Aspire workload installation is optional if running manually
# dotnet workload install aspire  # Only needed for Option 1 (Aspire orchestration)
```

---

## 🚀 Running the Application

### Option 1: .NET Aspire Orchestration (Recommended - If Available)

The easiest way to run the entire application with all services (requires Aspire workload):

```bash
# Navigate to the AppHost project
cd WorldLeaders.AppHost

# Run the complete application stack
dotnet run

# Alternative: Run with specific environment
dotnet run --environment Development

# Run with verbose logging
dotnet run --verbosity detailed
```

**What this does:**

- Starts PostgreSQL in Docker container
- Launches the Game API on `https://localhost:7155`
- Launches the Blazor Web app on `https://localhost:7154`
- Opens Aspire Dashboard at `https://localhost:15000` (when available)

### Option 2: Manual Service Orchestration (No Aspire Required)

**For users who cannot install Aspire workload due to permissions or other issues:**

#### Step 1: Start Database Manually

```bash
# Start PostgreSQL using Docker
docker run --name worldleaders-postgres \
  -e POSTGRES_DB=worldleaders \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:15

# Verify database is running
docker ps | grep worldleaders-postgres

# Optional: View database logs
docker logs worldleaders-postgres
```

#### Step 2: Run API Service

```bash
# Navigate to API project
cd WorldLeaders.API

# Set connection string for manual database
export ConnectionStrings__DefaultConnection="Server=localhost;Database=worldleaders;User Id=postgres;Password=postgres;"

# Set development environment
export ASPNETCORE_ENVIRONMENT=Development

# Run the API
dotnet run

# The API will be available at: https://localhost:7155
```

#### Step 3: Run Web Application (In New Terminal)

```bash
# Open a new terminal window and navigate to Web project
cd WorldLeaders.Web

# Set API service URL
export ApiSettings__BaseUrl="https://localhost:7155"

# Set development environment
export ASPNETCORE_ENVIRONMENT=Development

# Run the Blazor Server app
dotnet run

# The Web app will be available at: https://localhost:7154
```

### Option 3: Individual Service Commands

For debugging or development of specific services:

#### Start Database Only

```bash
# Start PostgreSQL using Docker
docker run --name worldleaders-postgres \
  -e POSTGRES_DB=worldleaders \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:15

# Verify database is running
docker ps | grep worldleaders-postgres
```

#### Run API Service

```bash
# Navigate to API project
cd WorldLeaders.API

# Set connection string (if not using Aspire)
export ConnectionStrings__DefaultConnection="Server=localhost;Database=worldleaders;User Id=postgres;Password=postgres;"

# Run the API
dotnet run

# Alternative: Run with specific port
dotnet run --urls "https://localhost:7155"
```

#### Run Web Application

```bash
# Navigate to Web project
cd WorldLeaders.Web

# Run the Blazor Server app
dotnet run

# Alternative: Run with specific port
dotnet run --urls "https://localhost:7154"
```

---

## 🚀 Quick Start Script (No Aspire Required)

Create a simple startup script to run everything manually:

### For macOS/Linux (start-game.sh)

```bash
#!/bin/bash
echo "🎮 Starting World Leaders Game..."

# Start PostgreSQL
echo "🐘 Starting PostgreSQL..."
docker run --name worldleaders-postgres \
  -e POSTGRES_DB=worldleaders \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:15

# Wait for database to be ready
echo "⏳ Waiting for database to be ready..."
sleep 5

# Start API in background
echo "🔧 Starting API..."
cd WorldLeaders.API
export ConnectionStrings__DefaultConnection="Server=localhost;Database=worldleaders;User Id=postgres;Password=postgres;"
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --urls "https://localhost:7155" &
API_PID=$!

# Wait for API to start
sleep 10

# Start Web application
echo "🌐 Starting Web App..."
cd ../WorldLeaders.Web
export ApiSettings__BaseUrl="https://localhost:7155"
export ASPNETCORE_ENVIRONMENT=Development
dotnet run --urls "https://localhost:7154"

# Cleanup on exit
trap "kill $API_PID; docker stop worldleaders-postgres; docker rm worldleaders-postgres" EXIT
```

### For Windows (start-game.bat)

```batch
@echo off
echo 🎮 Starting World Leaders Game...

echo 🐘 Starting PostgreSQL...
docker run --name worldleaders-postgres -e POSTGRES_DB=worldleaders -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15

echo ⏳ Waiting for database to be ready...
timeout /t 5

echo 🔧 Starting API...
cd WorldLeaders.API
set ConnectionStrings__DefaultConnection=Server=localhost;Database=worldleaders;User Id=postgres;Password=postgres;
set ASPNETCORE_ENVIRONMENT=Development
start /B dotnet run --urls "https://localhost:7155"

echo ⏳ Waiting for API to start...
timeout /t 10

echo 🌐 Starting Web App...
cd ..\WorldLeaders.Web
set ApiSettings__BaseUrl=https://localhost:7155
set ASPNETCORE_ENVIRONMENT=Development
dotnet run --urls "https://localhost:7154"
```

---

## 🎯 Visual Studio Code Integration

### VS Code Tasks (Command Palette)

Press `Cmd+Shift+P` (macOS) or `Ctrl+Shift+P` (Windows/Linux) and type "Tasks: Run Task", then choose:

#### Quick Start Options

- **🎮 Start Game (Aspire)** - Single command to start everything with Aspire
- **🎮 Start Game (Manual Mode)** - Single command to start everything without Aspire
- **🐘 Start Database** - Start PostgreSQL container only
- **🔧 Start API (Manual)** - Start API with manual database connection
- **🌐 Start Web App (Manual)** - Start Blazor web application

#### Utility Tasks

- **🔨 Build Solution** - Build entire solution
- **🔄 Restore Packages** - Restore NuGet packages
- **🧪 Test API Endpoints** - Test if services are responding
- **🛑 Stop All Services** - Stop PostgreSQL container
- **🧹 Clean Database** - Remove PostgreSQL container completely

### VS Code Debug Configurations (F5)

Press `F5` or go to Run and Debug panel:

#### Single Service Debugging

- **🎮 Debug Game (Aspire)** - Debug with Aspire orchestration
- **🔧 Debug API Only** - Debug just the API service
- **🌐 Debug Web App Only** - Debug just the web application

#### Multi-Service Debugging

- **🎮 Debug Game (Manual Mode)** - Debug both API and Web simultaneously

### Quick Start from VS Code

#### Option 1: Aspire Mode (One Command)

1. Open VS Code in project root
2. Press `Cmd+Shift+P` → "Tasks: Run Task"
3. Select **"🎮 Start Game (Aspire)"**
4. Wait for services to start
5. Open `https://localhost:7154` for the game

#### Option 2: Manual Mode (One Command)

1. Open VS Code in project root
2. Press `Cmd+Shift+P` → "Tasks: Run Task"
3. Select **"🎮 Start Game (Manual Mode)"**
4. This automatically runs:
   - Starts PostgreSQL database
   - Starts API service
   - Starts Web application
5. Open `https://localhost:7154` for the game

#### Option 3: Debug Mode

1. Open VS Code in project root
2. Press `F5` or go to Run and Debug
3. Select **"🎮 Debug Game (Aspire)"** or **"🎮 Debug Game (Manual Mode)"**
4. Set breakpoints in your code
5. Debug with full VS Code integration

### VS Code Terminal Integration

The tasks will open in VS Code's integrated terminal, allowing you to:

- **See all logs** in real-time
- **Stop services** with `Ctrl+C`
- **Monitor multiple services** in separate terminal panels
- **Keep everything within VS Code** - no external terminals needed

---

## 🛠️ Development Commands

### Build Operations

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build WorldLeaders.Web/WorldLeaders.Web.csproj

# Build in Release mode
dotnet build --configuration Release

# Clean and rebuild
dotnet clean
dotnet build
```

### Testing Commands

```bash
# Run all tests (when test projects are added)
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test WorldLeaders.Tests/WorldLeaders.Tests.csproj
```

### Database Management

```bash
# Add new migration (from Infrastructure project)
cd WorldLeaders.Infrastructure
dotnet ef migrations add InitialCreate --startup-project ../WorldLeaders.API

# Update database
dotnet ef database update --startup-project ../WorldLeaders.API

# Drop database (destructive!)
dotnet ef database drop --startup-project ../WorldLeaders.API

# Generate SQL scripts
dotnet ef migrations script --startup-project ../WorldLeaders.API
```

---

## 🔧 Package Management

### Adding NuGet Packages

```bash
# Add package to specific project
dotnet add WorldLeaders.Web package Microsoft.AspNetCore.SignalR

# Add package with specific version
dotnet add WorldLeaders.API package Microsoft.EntityFrameworkCore --version 8.0.0

# Add project reference
dotnet add WorldLeaders.Web reference WorldLeaders.Shared/WorldLeaders.Shared.csproj
```

### Package Updates

```bash
# List outdated packages
dotnet list package --outdated

# Update all packages
dotnet restore --force

# Update specific package
dotnet add package Microsoft.EntityFrameworkCore --version 8.0.1
```

---

## 🐳 Docker Commands

### Database Management

```bash
# Start PostgreSQL container
docker run --name worldleaders-postgres \
  -e POSTGRES_DB=worldleaders \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=postgres \
  -p 5432:5432 \
  -d postgres:15

# Stop database
docker stop worldleaders-postgres

# Remove database container
docker rm worldleaders-postgres

# View database logs
docker logs worldleaders-postgres

# Access database shell
docker exec -it worldleaders-postgres psql -U postgres -d worldleaders
```

### Application Containerization (Future)

```bash
# Build Docker image for API
docker build -t worldleaders-api -f WorldLeaders.API/Dockerfile .

# Build Docker image for Web
docker build -t worldleaders-web -f WorldLeaders.Web/Dockerfile .

# Run containerized application
docker-compose up -d
```

---

## 🔍 Monitoring & Debugging

### Application Logs

```bash
# Run with detailed logging
dotnet run --verbosity detailed

# Enable specific log levels
export DOTNET_ENVIRONMENT=Development
export Logging__LogLevel__Default=Information
dotnet run
```

### Health Checks

```bash
# Check API health
curl https://localhost:7155/health

# Check API status
curl https://localhost:7155/api/health

# Test API endpoints
curl https://localhost:7155/api/game/territories
```

### Performance Monitoring

```bash
# Run with performance counters
dotnet run --property:EnableEventPipe=true

# Profile application startup
dotnet run --property:COMPlus_PerfMapEnabled=1
```

---

## 🎯 Game-Specific Commands

### Database Seeding

```bash
# Navigate to API project
cd WorldLeaders.API

# Run database seeder (when implemented)
dotnet run --seed-data

# Reset game data
dotnet run --reset-game-data
```

### Development Utilities

```bash
# Generate test player data
dotnet run --project WorldLeaders.API --generate-test-data

# Export game configuration
dotnet run --project WorldLeaders.API --export-config

# Validate AI agent configurations
dotnet run --project WorldLeaders.API --validate-agents
```

---

## 🌐 Environment Configuration

### Development Environment

```bash
# Set development environment
export ASPNETCORE_ENVIRONMENT=Development

# Set custom configuration
export GameSettings__MaxPlayers=100
export GameSettings__EnableAI=true

# Run with custom settings
dotnet run
```

### Production Environment

```bash
# Set production environment
export ASPNETCORE_ENVIRONMENT=Production

# Set production database connection
export ConnectionStrings__DefaultConnection="Server=prod-server;Database=worldleaders;..."

# Run in production mode
dotnet run --configuration Release
```

---

## 🚨 Troubleshooting Commands

### Common Issues

#### .NET Aspire Missing Workload (Most Common)

```bash
# Error: "Property CliPath: The path to the DCP executable used for Aspire orchestration is required"
# This error occurs when the Aspire workload is not installed

# Step 1: Install Aspire workload
dotnet workload install aspire

# Step 2: Verify installation
dotnet workload list

# Step 3: If still failing, repair the installation
dotnet workload repair

# Step 4: Update all workloads
dotnet workload update

# Step 5: Clean and rebuild
dotnet clean
dotnet build

# Step 6: Try running again
cd WorldLeaders.AppHost
dotnet run
```

#### Port Already in Use

```bash
# Find process using port 7154
lsof -i :7154

# Kill process on port
kill -9 $(lsof -t -i:7154)

# Run on different port
dotnet run --urls "https://localhost:7156"
```

#### Database Connection Issues

```bash
# Verify PostgreSQL is running
docker ps | grep postgres

# Check connection
pg_isready -h localhost -p 5432

# Reset database container
docker stop worldleaders-postgres
docker rm worldleaders-postgres
# Then restart with run command above
```

#### Build Errors

```bash
# Clean solution
dotnet clean

# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore --force

# Rebuild solution
dotnet build
```

#### SSL/HTTPS Issues

```bash
# Trust development certificates
dotnet dev-certs https --trust

# Clean and regenerate certificates
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

#### .NET Aspire Issues

```bash
# Missing Aspire workload error
# Error: "Property CliPath: The path to the DCP executable used for Aspire orchestration is required"
dotnet workload install aspire

# Repair Aspire installation
dotnet workload repair

# Check installed workloads
dotnet workload list

# Update all workloads
dotnet workload update

# If still having issues, restart Docker Desktop and try again
docker restart $(docker ps -q)
```

---

## 📊 Performance & Testing

### Load Testing

```bash
# Install tools
dotnet tool install -g Microsoft.dotnet-httprepl

# Test API endpoints
httprepl https://localhost:7155

# Stress test (when implemented)
dotnet run --project WorldLeaders.LoadTests
```

### Code Quality

```bash
# Run code analysis
dotnet build --verbosity normal

# Format code
dotnet format

# Security scan (future)
dotnet list package --vulnerable
```

---

## 🎓 Educational Development

### AI Agent Testing

```bash
# Test AI agent responses (when implemented)
dotnet run --project WorldLeaders.API --test-agents

# Validate educational content
dotnet run --project WorldLeaders.API --validate-content

# Check child safety filters
dotnet run --project WorldLeaders.API --test-safety
```

### Content Management

```bash
# Export educational content
dotnet run --project WorldLeaders.API --export-content

# Validate territory data
dotnet run --project WorldLeaders.API --validate-territories

# Update GDP data from World Bank API
dotnet run --project WorldLeaders.API --update-gdp-data
```

---

## 📚 Quick Reference

### Essential Commands

```bash
# Option A: Start everything with Aspire (if workload is installed)
cd WorldLeaders.AppHost && dotnet run

# Option B: Start everything manually (no Aspire required)
# Terminal 1: Database
docker run --name worldleaders-postgres -e POSTGRES_DB=worldleaders -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15

# Terminal 2: API
cd WorldLeaders.API
export ConnectionStrings__DefaultConnection="Server=localhost;Database=worldleaders;User Id=postgres;Password=postgres;"
dotnet run

# Terminal 3: Web App
cd WorldLeaders.Web
dotnet run

# Build everything
dotnet build

# Clean everything
dotnet clean
```

### URLs After Starting

- **Game Web App**: `https://localhost:7154`
- **Game API**: `https://localhost:7155`
- **API Documentation**: `https://localhost:7155/swagger`
- **Aspire Dashboard**: `https://localhost:15000` (when available)

### Environment Variables

```bash
export ASPNETCORE_ENVIRONMENT=Development
export ConnectionStrings__DefaultConnection="Server=localhost;Database=worldleaders;User Id=postgres;Password=postgres;"
export GameSettings__EnableAI=true
export Logging__LogLevel__Default=Information
```

---

## 🎮 Game Testing Commands

### Quick Game Tests

```bash
# Create test player
curl -X POST https://localhost:7155/api/game/players \
  -H "Content-Type: application/json" \
  -d '{"username": "TestPlayer"}'

# Get territories
curl https://localhost:7155/api/game/territories

# Roll career dice
curl -X POST https://localhost:7155/api/game/players/{playerId}/career-roll

# Get random event
curl -X POST https://localhost:7155/api/game/players/{playerId}/random-event
```

---

## 💡 Tips for Developers

### Best Practices

1. **Always use .NET Aspire** (`dotnet run --project WorldLeaders.AppHost`) for full stack
2. **Check Docker first** if database connection fails
3. **Use `dotnet clean` followed by `dotnet build`** for build issues
4. **Trust HTTPS certificates** with `dotnet dev-certs https --trust`
5. **Monitor Aspire Dashboard** for service health and logs

### Development Workflow

```bash
# Option A: With Aspire (if available)
git pull origin main
cd src/WorldLeaders
dotnet workload install aspire  # Only if you have permissions
dotnet restore
dotnet build
cd WorldLeaders.AppHost
dotnet run

# Option B: Manual workflow (no Aspire permissions needed)
git pull origin main
cd src/WorldLeaders
dotnet restore
dotnet build

# Start database
docker run --name worldleaders-postgres -e POSTGRES_DB=worldleaders -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15

# In one terminal: Start API
cd WorldLeaders.API
export ConnectionStrings__DefaultConnection="Server=localhost;Database=worldleaders;User Id=postgres;Password=postgres;"
dotnet run

# In another terminal: Start Web
cd WorldLeaders.Web
dotnet run
```

### First-Time Setup Checklist

#### For Users WITH Aspire Permissions:

1. ✅ Install .NET 8 SDK
2. ✅ Install Docker Desktop and start it
3. ✅ Install Aspire workload: `dotnet workload install aspire`
4. ✅ Clone repository and navigate to `src/WorldLeaders`
5. ✅ Run `dotnet restore` and `dotnet build`
6. ✅ Trust HTTPS certificates: `dotnet dev-certs https --trust`
7. ✅ Start application: `cd WorldLeaders.AppHost && dotnet run`

#### For Users WITHOUT Aspire Permissions:

1. ✅ Install .NET 8 SDK
2. ✅ Install Docker Desktop and start it
3. ✅ Clone repository and navigate to `src/WorldLeaders`
4. ✅ Run `dotnet restore` and `dotnet build`
5. ✅ Trust HTTPS certificates: `dotnet dev-certs https --trust`
6. ✅ Start database: `docker run --name worldleaders-postgres -e POSTGRES_DB=worldleaders -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:15`
7. ✅ Start API: `cd WorldLeaders.API && export ConnectionStrings__DefaultConnection="Server=localhost;Database=worldleaders;User Id=postgres;Password=postgres;" && dotnet run`
8. ✅ Start Web: `cd WorldLeaders.Web && dotnet run` (in new terminal)

This workflow ensures you have the latest code, dependencies are updated, everything builds successfully, and the full application stack starts correctly.

---

_This guide will be updated as new features and deployment options are added to the World Leaders Game project._
