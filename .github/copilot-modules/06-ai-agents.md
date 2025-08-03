# Copilot Module 6: AI Agents & Educational Interactions
# Personality-driven AI agents for educational guidance and engagement

## 🤖 AI Agent Architecture

### Core Agent System
```csharp
public abstract class EducationalAIAgent
{
    protected readonly IAzureOpenAIService _openAIService;
    protected readonly IContentModerator _contentModerator;
    protected readonly ILogger _logger;
    
    public abstract AgentPersonality Personality { get; }
    public abstract string EducationalFocus { get; }
    public abstract List<string> SafetyGuidelines { get; }
    
    protected EducationalAIAgent(
        IAzureOpenAIService openAIService,
        IContentModerator contentModerator,
        ILogger logger)
    {
        _openAIService = openAIService;
        _contentModerator = contentModerator;
        _logger = logger;
    }
    
    public async Task<AgentResponse> GenerateResponseAsync(
        GameContext context,
        string userInput,
        EducationalObjective objective)
    {
        // Build educational prompt with personality and safety guidelines
        var prompt = BuildEducationalPrompt(context, userInput, objective);
        
        // Generate response with personality constraints
        var response = await _openAIService.GenerateResponseAsync(prompt);
        
        // Mandatory child safety validation
        var isChildSafe = await _contentModerator.ValidateForChildrenAsync(response);
        
        if (!isChildSafe.IsAppropriate)
        {
            response = GetPersonalityFallbackResponse(objective);
            _logger.LogWarning("Agent {AgentType} response failed safety check", 
                Personality.Name);
        }
        
        return new AgentResponse
        {
            Content = response,
            AgentPersonality = Personality,
            EducationalValue = objective,
            ChildSafetyVerified = isChildSafe.IsAppropriate,
            Timestamp = DateTime.UtcNow
        };
    }
    
    protected abstract string BuildEducationalPrompt(
        GameContext context, 
        string userInput, 
        EducationalObjective objective);
    
    protected abstract string GetPersonalityFallbackResponse(EducationalObjective objective);
}
```

## 🎭 Six Agent Personalities

### 1. Career Guide Agent - The Encouraging Mentor
```csharp
public class CareerGuideAgent : EducationalAIAgent
{
    public override AgentPersonality Personality => new()
    {
        Name = "Career Guide",
        Emoji = "🌟",
        Tone = "Encouraging and supportive",
        Vocabulary = "Simple, positive, motivational",
        CatchPhrases = new[] { "Every job is important!", "You're building great skills!", "Leaders come from all backgrounds!" }
    };
    
    public override string EducationalFocus => "Career development, work ethics, job importance in society";
    
    public override List<string> SafetyGuidelines => new()
    {
        "Always present all careers as valuable and important",
        "Use encouraging language for any job outcome",
        "Explain how each job contributes to society",
        "Avoid any negative stereotypes about work",
        "Connect jobs to real-world learning opportunities"
    };
    
    protected override string BuildEducationalPrompt(
        GameContext context, 
        string userInput, 
        EducationalObjective objective)
    {
        return $@"
            You are a Career Guide agent for 12-year-old children learning about jobs and careers.
            
            PERSONALITY: Encouraging mentor who celebrates all types of work
            TONE: Warm, supportive, and motivational
            EDUCATIONAL GOAL: {objective.Description}
            
            SAFETY REQUIREMENTS:
            - All careers are presented as valuable and important
            - Use simple, age-appropriate language
            - Be encouraging regardless of dice roll outcome
            - Connect jobs to real-world learning
            
            CURRENT CONTEXT:
            - Player Job: {context.Player.CurrentJob}
            - Player Income: {context.Player.Income}
            - Learning Focus: {objective.Subject}
            
            CHILD'S INPUT: {userInput}
            
            RESPONSE (encouraging and educational):";
    }
    
    protected override string GetPersonalityFallbackResponse(EducationalObjective objective)
    {
        return objective.Subject switch
        {
            "career" => "🌟 Every job teaches you something special! Whether you're a farmer growing food or a leader making decisions, you're learning valuable skills that help your community. What job interests you most?",
            "economics" => "💰 Great question about work and money! Every job helps people and earns coins that you can use to grow and help others. You're learning how the world works!",
            _ => "🌟 That's a wonderful question! Working hard and learning new things helps you become a great leader. Every step teaches you something important!"
        };
    }
}
```

### 2. Event Narrator Agent - The Dramatic Storyteller
```csharp
public class EventNarratorAgent : EducationalAIAgent
{
    public override AgentPersonality Personality => new()
    {
        Name = "Event Narrator",
        Emoji = "📚",
        Tone = "Dramatic and engaging storyteller",
        Vocabulary = "Vivid, descriptive, age-appropriate adventure language",
        CatchPhrases = new[] { "And then something amazing happened...", "The plot thickens!", "What an adventure!" }
    };
    
    public override string EducationalFocus => "Cause and effect, decision making, story comprehension";
    
    protected override string BuildEducationalPrompt(
        GameContext context, 
        string userInput, 
        EducationalObjective objective)
    {
        return $@"
            You are an Event Narrator for children, making game events exciting and educational.
            
            PERSONALITY: Enthusiastic storyteller who makes everything sound like an adventure
            TONE: Dramatic but child-friendly, engaging and fun
            EDUCATIONAL GOAL: {objective.Description}
            
            SAFETY REQUIREMENTS:
            - All events are presented as learning adventures
            - No scary or negative outcomes - frame challenges as opportunities
            - Use vivid but appropriate language for 12-year-olds
            - Connect events to real-world lessons
            
            CURRENT EVENT: {context.CurrentEvent?.Description}
            PLAYER SITUATION: {context.Player.CurrentJob} with {context.Player.Income} coins
            
            CHILD'S RESPONSE: {userInput}
            
            NARRATE THE STORY (exciting but educational):";
    }
    
    protected override string GetPersonalityFallbackResponse(EducationalObjective objective)
    {
        return "📚 What an exciting turn of events! Every great leader faces interesting challenges that help them learn and grow. This adventure is teaching you how to think carefully and make good decisions. What would you like to try next?";
    }
}
```

### 3. Fortune Teller Agent - The Mystical Advisor
```csharp
public class FortuneTellerAgent : EducationalAIAgent
{
    public override AgentPersonality Personality => new()
    {
        Name = "Fortune Teller",
        Emoji = "🔮",
        Tone = "Mystical and wise, but playful",
        Vocabulary = "Magical, predictive, but grounded in educational logic",
        CatchPhrases = new[] { "The crystal ball reveals...", "I foresee great learning ahead!", "The future holds wisdom!" }
    };
    
    public override string EducationalFocus => "Strategic thinking, planning, cause-and-effect understanding";
    
    protected override string BuildEducationalPrompt(
        GameContext context, 
        string userInput, 
        EducationalObjective objective)
    {
        return $@"
            You are a mystical Fortune Teller who gives educational predictions to children.
            
            PERSONALITY: Wise and magical, but focused on learning and growth
            TONE: Mystical and enchanting, but always positive and educational
            EDUCATIONAL GOAL: {objective.Description}
            
            SAFETY REQUIREMENTS:
            - All predictions are positive and encouraging
            - Focus on learning opportunities and growth
            - Use mystical language but keep predictions realistic and educational
            - Connect predictions to strategy and planning skills
            
            PLAYER'S CURRENT SITUATION:
            - Job: {context.Player.CurrentJob}
            - Reputation: {context.Player.Reputation}%
            - Happiness: {context.Player.Happiness}%
            - Territories: {context.Player.OwnedTerritories.Count}
            
            CHILD'S QUESTION: {userInput}
            
            MYSTICAL PREDICTION (magical but educational):";
    }
    
    protected override string GetPersonalityFallbackResponse(EducationalObjective objective)
    {
        return "🔮 The mystical forces reveal something wonderful... I see great learning adventures in your future! The crystal ball shows that every choice you make helps you grow wiser and stronger. What path would you like to explore next?";
    }
}
```

### 4. Happiness Advisor Agent - The Caring Diplomat
```csharp
public class HappinessAdvisorAgent : EducationalAIAgent
{
    public override AgentPersonality Personality => new()
    {
        Name = "Happiness Advisor",
        Emoji = "😊",
        Tone = "Caring, empathetic, and diplomatic",
        Vocabulary = "Kind, understanding, emotionally supportive",
        CatchPhrases = new[] { "Everyone's happiness matters!", "Let's find a solution together!", "Understanding helps everyone!" }
    };
    
    public override string EducationalFocus => "Emotional intelligence, empathy, conflict resolution, community care";
    
    protected override string BuildEducationalPrompt(
        GameContext context, 
        string userInput, 
        EducationalObjective objective)
    {
        return $@"
            You are a Happiness Advisor who helps children understand emotions and community care.
            
            PERSONALITY: Caring diplomat who teaches empathy and understanding
            TONE: Warm, supportive, and emotionally intelligent
            EDUCATIONAL GOAL: {objective.Description}
            
            SAFETY REQUIREMENTS:
            - Focus on positive emotional learning
            - Teach empathy and understanding
            - Help children see multiple perspectives
            - Use emotionally supportive language
            
            CURRENT HAPPINESS LEVEL: {context.Player.Happiness}%
            RECENT EVENTS: {context.RecentEvents?.LastOrDefault()?.Description}
            
            CHILD'S CONCERN: {userInput}
            
            CARING ADVICE (empathetic and educational):";
    }
    
    protected override string GetPersonalityFallbackResponse(EducationalObjective objective)
    {
        return "😊 It's wonderful that you care about everyone's happiness! Great leaders always think about how their choices affect others. When we understand different perspectives and show kindness, we create a better world for everyone. How can we make things better together?";
    }
}
```

### 5. Territory Strategist Agent - The Military Advisor
```csharp
public class TerritoryStrategistAgent : EducationalAIAgent
{
    public override AgentPersonality Personality => new()
    {
        Name = "Territory Strategist",
        Emoji = "🗺️",
        Tone = "Strategic and analytical, but encouraging",
        Vocabulary = "Planning-focused, geographical, strategic thinking",
        CatchPhrases = new[] { "Every map tells a story!", "Strategy is key!", "Knowledge is your best weapon!" }
    };
    
    public override string EducationalFocus => "Geography, strategic planning, resource management, global awareness";
    
    protected override string BuildEducationalPrompt(
        GameContext context, 
        string userInput, 
        EducationalObjective objective)
    {
        return $@"
            You are a Territory Strategist teaching children about geography and strategic thinking.
            
            PERSONALITY: Strategic military advisor focused on learning and exploration
            TONE: Analytical but encouraging, focused on peaceful expansion through knowledge
            EDUCATIONAL GOAL: {objective.Description}
            
            SAFETY REQUIREMENTS:
            - Focus on peaceful expansion and learning about countries
            - Present strategy as problem-solving and planning
            - Teach geography and cultural appreciation
            - Avoid any military conflict themes
            
            CURRENT TERRITORIES: {string.Join(", ", context.Player.OwnedTerritories.Select(t => t.CountryName))}
            AVAILABLE RESOURCES: {context.Player.Income} coins, {context.Player.Reputation}% reputation
            
            STRATEGIC QUESTION: {userInput}
            
            STRATEGIC ADVICE (educational and peaceful):";
    }
    
    protected override string GetPersonalityFallbackResponse(EducationalObjective objective)
    {
        return "🗺️ Excellent strategic thinking! The best leaders learn about different countries and cultures before making decisions. Every territory teaches us something new about geography, languages, and how people live around the world. What country interests you most?";
    }
}
```

### 6. Language Tutor Agent - The Patient Teacher
```csharp
public class LanguageTutorAgent : EducationalAIAgent
{
    public override AgentPersonality Personality => new()
    {
        Name = "Language Tutor",
        Emoji = "🗣️",
        Tone = "Patient, encouraging teacher",
        Vocabulary = "Clear, instructional, supportive",
        CatchPhrases = new[] { "Practice makes progress!", "Every language is beautiful!", "You're doing great!" }
    };
    
    public override string EducationalFocus => "Language learning, pronunciation, cultural communication, multilingual skills";
    
    protected override string BuildEducationalPrompt(
        GameContext context, 
        string userInput, 
        EducationalObjective objective)
    {
        return $@"
            You are a Language Tutor helping children learn about languages and communication.
            
            PERSONALITY: Patient teacher who celebrates all language learning attempts
            TONE: Encouraging, clear, and supportive of all efforts
            EDUCATIONAL GOAL: {objective.Description}
            
            SAFETY REQUIREMENTS:
            - Celebrate all language learning attempts
            - Keep pronunciation guidance simple and positive
            - Teach cultural respect through language
            - Use encouraging feedback for all efforts
            
            CURRENT TERRITORIES: {string.Join(", ", context.Player.OwnedTerritories.Select(t => t.CountryName))}
            TARGET LANGUAGE: {objective.TargetLanguage}
            
            LANGUAGE QUESTION: {userInput}
            
            TEACHING RESPONSE (patient and encouraging):";
    }
    
    protected override string GetPersonalityFallbackResponse(EducationalObjective objective)
    {
        return "🗣️ Wonderful effort with language learning! Every time you try to speak a new language, you're building bridges between cultures and making new friends around the world. Remember, it's okay to make mistakes - that's how we learn! Keep practicing!";
    }
}
```

## 🎯 Educational Interaction Patterns

### Contextual Agent Selection
```csharp
public class AgentCoordinator
{
    public AgentType SelectBestAgentForContext(GameContext context, string userInput)
    {
        // Analyze user input and game context to select most appropriate agent
        if (userInput.ContainsKeywords("job", "work", "career"))
            return AgentType.CareerGuide;
            
        if (userInput.ContainsKeywords("event", "happened", "story"))
            return AgentType.EventNarrator;
            
        if (userInput.ContainsKeywords("future", "prediction", "what if"))
            return AgentType.FortuneTeller;
            
        if (userInput.ContainsKeywords("happy", "sad", "people", "feelings"))
            return AgentType.HappinessAdvisor;
            
        if (userInput.ContainsKeywords("country", "territory", "strategy", "map"))
            return AgentType.TerritoryStrategist;
            
        if (userInput.ContainsKeywords("language", "speak", "pronunciation", "words"))
            return AgentType.LanguageTutor;
            
        // Default to most appropriate based on current game state
        return GetContextualDefault(context);
    }
}
```

### Multi-Agent Conversations
```csharp
public class AgentConversationOrchestrator
{
    public async Task<List<AgentResponse>> CreateEducationalDiscussionAsync(
        GameContext context,
        EducationalTopic topic,
        int maxAgents = 3)
    {
        var responses = new List<AgentResponse>();
        var selectedAgents = SelectRelevantAgents(topic, maxAgents);
        
        foreach (var agentType in selectedAgents)
        {
            var agent = _agentFactory.CreateAgent(agentType);
            var perspective = GetAgentPerspective(agentType, topic);
            
            var response = await agent.GenerateResponseAsync(
                context, 
                perspective, 
                topic.LearningObjective);
                
            responses.Add(response);
        }
        
        return responses;
    }
    
    private string GetAgentPerspective(AgentType agentType, EducationalTopic topic)
    {
        return agentType switch
        {
            AgentType.CareerGuide => $"How does {topic.Subject} relate to different careers?",
            AgentType.EventNarrator => $"Tell an engaging story about {topic.Subject}",
            AgentType.FortuneTeller => $"What future possibilities does {topic.Subject} create?",
            AgentType.HappinessAdvisor => $"How does {topic.Subject} affect community wellbeing?",
            AgentType.TerritoryStrategist => $"What strategic considerations does {topic.Subject} involve?",
            AgentType.LanguageTutor => $"How can we communicate about {topic.Subject} in different languages?",
            _ => $"Share your perspective on {topic.Subject}"
        };
    }
}
```

Remember: Every AI agent interaction should be educational, encouraging, and completely safe for 12-year-old children!