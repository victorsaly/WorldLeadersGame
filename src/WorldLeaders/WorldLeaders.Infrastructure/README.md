# World Leaders Game - Database Setup

This document describes the Entity Framework setup for the World Leaders educational game database.

## 📋 Overview

The database is designed for educational gameplay targeting 12-year-old players. It includes:

- **Child Safety**: Soft deletes, audit trails, and data protection
- **Educational Content**: Real GDP data, geography, and language learning
- **Game Mechanics**: Player progression, territories, and AI interactions

## 🏗️ Database Structure

### Core Tables

1. **Players** - Child player accounts with progress tracking
2. **Territories** - Countries with real World Bank GDP data
3. **GameEvents** - Educational events teaching decision-making
4. **AIInteractions** - Safe AI conversation history
5. **LanguageProgress** - Speech learning advancement

## 🚀 Quick Start

### Prerequisites

1. PostgreSQL server running locally
2. .NET 8 SDK installed
3. Entity Framework tools: `dotnet tool install --global dotnet-ef`

### Database Setup

1. **Create Migration** (already done):
   ```bash
   cd src/WorldLeaders/WorldLeaders.Infrastructure
   dotnet ef migrations add InitialCreate
   ```

2. **Apply Migration**:
   ```bash
   dotnet ef database update
   ```

3. **Verify Setup**:
   ```bash
   dotnet run --project Tools/DatabaseVerificationTool.cs
   ```

### Connection String Configuration

Default connection string in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=worldleadersdb;Username=postgres;Password=postgres;"
  }
}
```

## 🎯 Educational Features

### Territory Data

- **34 Countries** with real GDP rankings
- **3 Difficulty Tiers** for progressive learning
- **Multiple Languages** for speech challenges
- **Economic Data** for mathematics education

### Game Events

- **15+ Educational Scenarios** teaching decision-making
- **Age-Appropriate Content** for 12-year-olds
- **Positive Reinforcement** patterns
- **Cultural Sensitivity** considerations

### Child Safety

- **Soft Delete Protection** - No data loss
- **Audit Trails** - Track all changes
- **Content Moderation** - AI safety filters
- **Session Management** - Time limits

## 🔧 Configuration

### Environment Variables

- `ConnectionStrings__DefaultConnection` - Database connection
- `Logging__EnableSensitiveDataLogging` - Development only
- `GameSettings__EnableChildSafetyMode` - Always true in production

### Deployment

- **Development**: SQLite or local PostgreSQL
- **Production**: Azure PostgreSQL with encryption
- **Testing**: In-memory database

## 📊 Data Seeding

### Automatic Seeding

The database automatically seeds:

- **Territories**: Based on real World Bank GDP data
- **Game Events**: Educational scenarios for learning
- **Default Configuration**: Child-safe settings

### Custom Seeding

To add custom educational content:

1. Update seed files in `Data/Seed/`
2. Create new migration: `dotnet ef migrations add YourMigrationName`
3. Apply: `dotnet ef database update`

## 🧪 Testing

### Database Verification Tool

Run the verification tool to ensure setup is correct:

```bash
cd src/WorldLeaders/WorldLeaders.Infrastructure
dotnet run Tools/DatabaseVerificationTool.cs
```

Expected output:
- ✅ Database Connection
- ✅ All Tables Created
- ✅ Seed Data Loaded
- ✅ Child Safety Features Active

### Manual Testing

```sql
-- Check territory data
SELECT COUNT(*) FROM "Territories";

-- Verify educational events
SELECT "Title", "Type", "IsPositive" FROM "GameEvents";

-- Check soft delete protection
SELECT COUNT(*) FROM "Territories" WHERE "IsDeleted" = false;
```

## 🛡️ Security Considerations

### Child Privacy (COPPA Compliance)

- Minimal data collection
- Secure password handling
- Session timeouts
- Parent/guardian oversight

### Data Protection

- Encrypted connections (SSL)
- Soft deletes prevent data loss
- Audit trails for accountability
- Regular backups

## 📚 Educational Objectives

### Economics Learning

- Real GDP data from World Bank
- Supply and demand concepts
- Resource management
- International trade basics

### Geography Education

- Country locations and capitals
- Cultural awareness
- Language diversity
- Global citizenship

### Decision Making

- Consequence-based learning
- Strategic thinking
- Problem-solving skills
- Leadership development

## 🔄 Maintenance

### Regular Tasks

1. **Database Backups** - Daily automated
2. **Performance Monitoring** - Query optimization
3. **Content Updates** - New educational events
4. **Security Audits** - Child safety compliance

### Troubleshooting

Common issues and solutions:

- **Connection Failed**: Check PostgreSQL service
- **Migration Errors**: Verify Entity configurations
- **Seed Data Missing**: Run database verification tool
- **Performance Issues**: Check query plans and indexes

## 📈 Future Enhancements

### Planned Features

- Real-time GDP data updates
- Enhanced AI personality system
- Multiplayer collaborative learning
- Advanced language challenges
- Progress analytics for educators

### Scalability

- Read replicas for performance
- Caching layers for frequently accessed data
- Microservices architecture
- Cloud-native deployment