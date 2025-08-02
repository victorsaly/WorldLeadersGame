# GitHub Copilot Chat Prompts for World Leaders Game

## 🚀 Project Setup Prompts

### Initial Setup
```
Create a .NET Aspire solution structure for a Blazor Server game called "WorldLeaders" with the following projects: AppHost for orchestration, Web for Blazor UI, API for game services, Shared for models, and Tests for testing. Include proper project references and basic folder structure.
```

```
Set up TailwindCSS integration in a Blazor Server project with proper configuration for CSS purging, custom colors for a children's game theme, and responsive design utilities.
```

```
Create Entity Framework models for a strategy game including Player, GameState, Territory, GameEvent, and Job entities with proper relationships and data annotations.
```

## 🎮 Game Engine Prompts

### Core Game Logic
```
Create a turn-based game engine service in C# that manages game phases, player progression, dice rolling mechanics, and state transitions for a strategy game.
```

```
Build a card-based random event system with different event types (good/bad), probability weights, and consequence calculation for a children's strategy game.
```

```
Design a resource management system that tracks player income, reputation, and happiness with validation rules and automatic calculations.
```

### Game State Management
```
Create a game state service using SignalR that allows real-time updates of player progress, territory acquisition, and happiness changes with persistence to database.
```

## 🤖 AI Agent Development Prompts

### Base AI Framework
```
Create a base AI agent service interface and implementation that integrates with OpenAI API, manages conversation context, and provides personality-driven responses for different agent types.
```

```
Build a prompt engineering system for game AI agents with different personalities: encouraging career guide, dramatic storyteller, mystical fortune teller, caring advisor, military strategist, and patient language teacher.
```

### Specific AI Agents
```
Create a Career Guide AI agent that analyzes current job level, provides encouragement for dice rolls, explains job benefits, and gives strategic career advancement advice for a 12-year-old player.
```

```
Build a Fortune Teller AI agent that analyzes current game state, predicts potential outcomes, provides mystical insights, and gives strategic planning advice in an engaging, mysterious manner.
```

```
Design a Language Tutor AI agent that creates vocabulary challenges, analyzes speech input, provides pronunciation feedback, and tracks language learning progress with age-appropriate content.
```

## 🎤 Speech Recognition Prompts

### Azure Speech Integration
```
Integrate Azure Speech Services for real-time speech recognition and pronunciation assessment in a Blazor Server application with proper error handling and child-friendly feedback.
```

```
Create a speech analysis service that evaluates pronunciation accuracy, provides detailed feedback on specific phonemes, and tracks improvement over time for language learning.
```

```
Build a Blazor component for recording audio, displaying real-time speech recognition results, and showing pronunciation scores with visual feedback for children.
```

## 🎨 UI/UX Development Prompts

### Blazor Components
```
Create a responsive game dashboard Blazor component using TailwindCSS with animated cards showing player stats (income, reputation, happiness), progress bars, and child-friendly icons.
```

```
Build an interactive dice rolling component in Blazor with CSS animations, sound effects integration, and smooth transitions that appeals to 12-year-old players.
```

```
Design a world map component for territory acquisition with clickable regions, hover effects, requirement tooltips, and purchase confirmations using TailwindCSS and Blazor.
```

### Interactive Elements
```
Create an animated card drawing system in Blazor that simulates drawing random event cards with flip animations, sound effects, and dramatic reveal sequences.
```

```
Build a happiness meter component with smooth animations, color transitions from red to green, and interactive elements that show the impact of player decisions.
```

```
Design a fortune teller interface with mystical animations, crystal ball effects, and atmospheric styling using TailwindCSS for an immersive experience.
```

## 🌍 Game Features Prompts

### Territory System
```
Create a territory management system with different territory types, reputation requirements, income benefits, and strategic value calculations for progression-based acquisition.
```

```
Build a reputation system that tracks player actions, calculates reputation changes from decisions and events, and determines territory unlock thresholds.
```

### Language Learning
```
Design a multilingual vocabulary system with word databases, difficulty progression, pronunciation targets, and cultural context for educational value.
```

```
Create language challenge mini-games integrated with speech recognition that test vocabulary, pronunciation, and comprehension with immediate AI feedback.
```

## 🔧 Technical Implementation Prompts

### Performance & Optimization
```
Implement efficient state management for a real-time multiplayer-capable game using SignalR groups, optimized database queries, and proper caching strategies.
```

```
Create a robust error handling and logging system for AI agent interactions, speech recognition failures, and game state corruption with child-appropriate error messages.
```

### Security & Safety
```
Implement content filtering and safety measures for AI-generated responses to ensure age-appropriate content for 12-year-old players, including inappropriate content detection and fallback responses.
```

```
Create a secure speech data handling system that processes audio locally when possible, encrypts transmitted data, and complies with children's privacy regulations.
```

## 🧪 Testing Prompts

### Unit Testing
```
Create comprehensive unit tests for the game engine including turn progression, resource calculations, AI agent responses, and edge cases for a children's strategy game.
```

```
Build integration tests for AI agent services including OpenAI API interactions, speech recognition accuracy, and end-to-end conversation flows.
```

### User Experience Testing
```
Design age-appropriate user testing scenarios for 12-year-old players including tutorial effectiveness, AI agent helpfulness, and overall engagement metrics.
```

## 🚀 Deployment Prompts

### Azure Deployment
```
Create Azure Container Apps deployment configuration for a .NET Aspire application with proper environment variables, scaling rules, and health checks for a children's game.
```

```
Set up CI/CD pipeline using GitHub Actions for automated testing, security scanning, and deployment of a Blazor Server application with AI services integration.
```

## 📊 Analytics & Monitoring Prompts

### Game Analytics
```
Implement game analytics tracking for player progression, AI agent effectiveness, learning outcomes, and engagement metrics with privacy-compliant data collection for children.
```

```
Create a parent dashboard that shows child's learning progress, time spent, skills developed, and achievements unlocked while maintaining appropriate privacy controls.
```

## 🎯 Advanced Features Prompts

### Multiplayer Preparation
```
Design a scalable architecture for future multiplayer support including lobby systems, real-time synchronization, and fair play mechanisms for children.
```

### AI Enhancement
```
Create an adaptive AI system that adjusts difficulty, personalizes challenges, and provides increasingly sophisticated guidance based on individual player progress and learning style.
```

## 💡 Creative Content Prompts

### Storytelling
```
Generate age-appropriate historical and cultural context for territories, events, and language learning content that makes the game educational and engaging for 12-year-olds.
```

```
Create engaging character backstories and personalities for each AI agent that make them memorable and relatable to children while maintaining their educational purpose.
```

These prompts are designed to be used with GitHub Copilot Chat to accelerate development while maintaining focus on creating an engaging, educational, and safe gaming experience for children.
