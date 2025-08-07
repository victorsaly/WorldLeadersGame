/**
 * Educational Code Explanation API
 * Context: Educational game development for 12-year-old learners
 * Educational Objective: Provide age-appropriate programming explanations
 * Safety Requirements: Child-safe content, encouraging messaging
 */

// Only define the class if it doesn't exist on window
if (typeof window.EducationalCodeExplanationAPI === 'undefined') {

// Production-ready implementation with Azure OpenAI integration
class EducationalCodeExplanationAPI {
    constructor() {
        // Initialize configuration
        this.isProduction = false;
        this.hasValidConfig = false;
        this.useSecureProxy = false;
        this.useServerAPI = false;
        
        // Load server API configuration
        this.loadAzureConfiguration();
        
        // Initialize educational prompts
        this.initializeEducationalPrompts();
    }

    async loadAzureConfiguration() {
        try {
            // Check if secure proxy is available (for backward compatibility)
            if (window.secureAzureProxy && window.secureAzureProxy.isSecureAPIAvailable()) {
                console.log('✅ Secure OpenAI proxy detected but preferring server-side API');
            }

            // Configure for server-side API endpoint
            this.serverApiConfig = {
                baseUrl: this.getServerBaseUrl(),
                endpoint: '/api/ai/explain-code'
            };

            this.hasValidConfig = true;
            this.isProduction = true;
            this.useServerAPI = true;
            
            console.log('✅ Server-side API configuration loaded successfully');
            console.log('📖 Using secure server endpoint for code explanations');
        } catch (error) {
            console.log('ℹ️ Server API configuration failed, falling back to demo mode');
            this.hasValidConfig = false;
        }
    }

    getServerBaseUrl() {
        // Detect the current environment and return appropriate API base URL
        const currentHost = window.location.host;
        
        if (currentHost.includes('localhost') || currentHost.includes('127.0.0.1')) {
            return 'https://localhost:7155'; // Local development API (preferred HTTPS port)
        } else if (currentHost.includes('docs.worldleadersgame.co.uk')) {
            return 'https://api.worldleadersgame.co.uk'; // Production API
        } else {
            return 'https://localhost:7155'; // Default fallback (preferred HTTPS port)
        }
    }

    getConfigValue(key, defaultValue = null) {
        // Try multiple ways to get configuration values
        
        // 1. Check window.AZURE_CONFIG (can be set in Jekyll layouts)
        if (typeof window !== 'undefined' && window.AZURE_CONFIG && window.AZURE_CONFIG[key]) {
            return window.AZURE_CONFIG[key];
        }
        
        // 2. Check environment variables (if available in Node.js context)
        if (typeof process !== 'undefined' && process.env && process.env[key]) {
            return process.env[key];
        }
        
        // 3. Check localStorage for development
        if (typeof localStorage !== 'undefined') {
            const stored = localStorage.getItem(`azure_${key.toLowerCase()}`);
            if (stored && stored !== 'null' && stored !== 'undefined') {
                return stored;
            }
        }
        
        // 4. Return default or placeholder
        return defaultValue;
    }

    initializeEducationalPrompts() {
        this.educationalPrompts = {
            systemPrompt: `You are an encouraging programming teacher for 12-year-old students learning through an educational geography and economics game. 

Your role:
- Explain code concepts in simple, age-appropriate language
- Use analogies from everyday life that 12-year-olds understand
- Always be encouraging and positive
- Connect programming concepts to the educational game's geography/economics lessons
- Include real-world examples that relate to a child's experience
- Suggest next learning steps appropriate for their age

Guidelines:
- Use simple vocabulary (6th-grade reading level)
- Include emojis to make explanations engaging
- Relate programming concepts to familiar activities (games, sports, crafts, etc.)
- Always emphasize that making mistakes is part of learning
- Connect code to the educational mission of learning about world geography and economics

Safety requirements:
- All content must be appropriate for 12-year-olds
- Use only encouraging, positive language
- No complex technical jargon without simple explanations
- Include notes for parents/teachers when helpful`,

            explanationPrompt: `Please explain this code in a way that a 12-year-old can understand. This code is part of our educational World Leaders Game that teaches geography and economics.

Code to explain:
{CODE_CONTENT}

Context: This code is from our educational game that helps children learn about countries, economics, and world geography while having fun.

Please provide:
1. A simple summary of what the code does (1-2 sentences)
2. Step-by-step breakdown of key parts (max 5 steps)
3. Why this matters for learning programming
4. A real-world analogy a 12-year-old would understand
5. How this connects to learning geography/economics
6. Next steps for continued learning

Remember: Be encouraging, use simple language, and make it fun!`
        };
    }

    /**
     * Generate educational explanation for code
     */
    async generateExplanation(codeContent, context = {}) {
        try {
            // Check if we should use server API (preferred method)
            if (this.hasValidConfig && this.isProduction && this.useServerAPI) {
                // Use server-side API for secure explanations
                return await this.generateServerAPIExplanation(codeContent, context);
            } else {
                // Use local analysis for demo/development
                console.log('🎯 Using local analysis (Server API not configured)');
                const analysis = this.analyzeCode(codeContent);
                return this.createEducationalExplanation(analysis, codeContent);
            }
            
        } catch (error) {
            console.error('Code explanation generation failed:', error);
            return this.createFallbackExplanation();
        }
    }

    async generateServerAPIExplanation(codeContent, context = {}) {
        try {
            console.log('🔒 Using secure server-side API for code explanation');
            
            const requestBody = {
                code: codeContent,
                context: context.description || 'Educational programming lesson',
                language: this.detectLanguage(codeContent),
                domain: window.location.host
            };

            const response = await fetch(`${this.serverApiConfig.baseUrl}${this.serverApiConfig.endpoint}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Accept': 'application/json'
                },
                body: JSON.stringify(requestBody)
            });

            if (!response.ok) {
                throw new Error(`Server API error: ${response.status} - ${response.statusText}`);
            }

            const data = await response.json();
            
            if (data.success && data.summary) {
                return this.parseServerAPIResponse(data);
            } else {
                throw new Error('Invalid response from server API');
            }
            
        } catch (error) {
            console.error('Server API explanation failed:', error);
            console.log('⚡ Falling back to local analysis');
            
            // Fallback to local analysis
            const analysis = this.analyzeCode(codeContent);
            return this.createEducationalExplanation(analysis, codeContent);
        }
    }

    parseServerAPIResponse(serverResponse) {
        return {
            summary: serverResponse.summary,
            breakdown: serverResponse.breakdown.map(item => ({
                line: item.line,
                explanation: item.explanation,
                line_number: item.lineNumber
            })),
            educational_value: {
                learning_objective: serverResponse.educationalValue.learningObjective,
                age_appropriate_concepts: serverResponse.educationalValue.ageAppropriateConcepts,
                geography_economics_connection: "This code supports learning by making educational content interactive and engaging! 📚",
                life_skills: serverResponse.educationalValue.lifeSkills
            },
            real_world_application: serverResponse.realWorldApplication,
            next_steps: serverResponse.nextSteps,
            complexity_level: serverResponse.complexityLevel,
            programming_concepts: serverResponse.programmingConcepts,
            child_friendly_tips: serverResponse.childFriendlyTips
        };
    }

    async generateAzureOpenAIExplanation(codeContent, context = {}) {
        try {
            // Prioritize secure proxy if available
            if (this.useSecureProxy && window.secureAzureProxy && window.secureAzureProxy.isSecureAPIAvailable()) {
                console.log('🔒 Using secure OpenAI proxy with domain validation');
                const prompt = this.buildEducationalPrompt(codeContent, context);
                const explanation = await window.secureAzureProxy.makeSecureAPICall(prompt, context);
                return this.parseAzureOpenAIResponse(explanation);
            }
            
            // Fallback to direct Azure OpenAI API call if configured
            if (this.azureOpenAIConfig && this.azureOpenAIConfig.endpoint && this.azureOpenAIConfig.apiKey) {
                console.log('⚠️ Using direct Azure OpenAI API call - consider using secure proxy');
                const prompt = this.buildEducationalPrompt(codeContent, context);
                
                const response = await fetch(`${this.azureOpenAIConfig.endpoint}/openai/deployments/${this.azureOpenAIConfig.deploymentName}/chat/completions?api-version=${this.azureOpenAIConfig.apiVersion}`, {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        'api-key': this.azureOpenAIConfig.apiKey
                    },
                    body: JSON.stringify({
                        messages: [
                            {
                                role: 'system',
                                content: this.educationalPrompts.systemPrompt
                            },
                            {
                                role: 'user', 
                                content: prompt
                            }
                        ],
                        max_tokens: 800,
                        temperature: 0.7,
                        top_p: 0.9,
                        frequency_penalty: 0,
                        presence_penalty: 0
                    })
                });

                if (!response.ok) {
                    throw new Error(`Azure OpenAI API error: ${response.status}`);
                }

                const data = await response.json();
                const explanation = data.choices[0]?.message?.content;
                
                if (explanation) {
                    return this.parseAzureOpenAIResponse(explanation);
                } else {
                    throw new Error('No explanation generated');
                }
            }
            
            // If no valid configuration, throw error to trigger fallback
            throw new Error('No valid OpenAI configuration available');
            
        } catch (error) {
            console.error('OpenAI explanation failed:', error);
            console.log('⚡ Falling back to local analysis');
            
            // Fallback to local analysis
            const analysis = this.analyzeCode(codeContent);
            return this.createEducationalExplanation(analysis, codeContent);
        }
    }

    buildEducationalPrompt(codeContent, context) {
        return `Please explain this code to a 12-year-old student in our educational geography and economics game:

\`\`\`
${codeContent}
\`\`\`

Please provide:
1. A simple summary (max 25 words)
2. Step-by-step breakdown using everyday analogies
3. How this connects to geography or economics learning
4. Encouraging next steps for learning

Use 6th-grade vocabulary and be encouraging and positive!`;
    }

    parseAzureOpenAIResponse(explanation) {
        // Try to parse structured response or create structure from text
        try {
            // If Azure returns JSON structure, parse it
            if (explanation.startsWith('{')) {
                return JSON.parse(explanation);
            }
            
            // Otherwise, create structure from text response
            return {
                summary: this.extractSummaryFromText(explanation),
                breakdown: this.extractBreakdownFromText(explanation),
                educationalValue: this.extractEducationalValueFromText(explanation),
                nextSteps: this.extractNextStepsFromText(explanation)
            };
        } catch (error) {
            console.error('Error parsing Azure response:', error);
            // Return the raw explanation in a basic structure
            return {
                summary: "🤖 Here's what this code does!",
                breakdown: explanation,
                educationalValue: "This code teaches us about programming logic and problem-solving!",
                nextSteps: "Keep exploring and asking questions about how code works!"
            };
        }
    }

    /**
     * Analyze code to understand its educational concepts
     */
    analyzeCode(codeContent) {
        const analysis = {
            language: this.detectLanguage(codeContent),
            concepts: this.extractConcepts(codeContent),
            complexity: this.assessComplexity(codeContent),
            educationalValue: this.assessEducationalValue(codeContent)
        };

        return analysis;
    }

    /**
     * Detect programming language
     */
    detectLanguage(code) {
        if (code.includes('public class') && code.includes('namespace')) return 'csharp';
        if (code.includes('function') && code.includes('const')) return 'javascript';
        if (code.includes('def ') && code.includes('import')) return 'python';
        if (code.includes('razor') || code.includes('@code')) return 'blazor';
        return 'general';
    }

    /**
     * Extract programming concepts from code
     */
    extractConcepts(code) {
        const concepts = [];
        
        // Object-oriented programming
        if (code.includes('class ')) concepts.push('classes');
        if (code.includes('public ') || code.includes('private ')) concepts.push('access-modifiers');
        
        // Control structures
        if (code.includes('if ') || code.includes('else')) concepts.push('conditionals');
        if (code.includes('for ') || code.includes('while ')) concepts.push('loops');
        
        // Functions and methods
        if (code.includes('function ') || code.includes('def ') || code.includes('public async Task')) {
            concepts.push('functions');
        }
        
        // Async programming
        if (code.includes('async ') || code.includes('await ')) concepts.push('async-programming');
        
        // Data structures
        if (code.includes('List<') || code.includes('Array') || code.includes('[]')) concepts.push('data-structures');
        
        // Educational game specific
        if (code.includes('Player') || code.includes('Territory') || code.includes('Game')) {
            concepts.push('game-programming');
        }
        
        // Web development
        if (code.includes('@page') || code.includes('Component') || code.includes('razor')) {
            concepts.push('web-development');
        }
        
        return concepts;
    }

    /**
     * Assess code complexity for 12-year-old understanding
     */
    assessComplexity(code) {
        let complexity = 'beginner';
        
        const lines = code.split('\n').length;
        const concepts = this.extractConcepts(code);
        
        if (lines > 50 || concepts.length > 4) complexity = 'advanced';
        else if (lines > 20 || concepts.length > 2) complexity = 'intermediate';
        
        return complexity;
    }

    /**
     * Assess educational value for geography/economics learning
     */
    assessEducationalValue(code) {
        const educationalKeywords = [
            'Territory', 'Country', 'GDP', 'Player', 'Economics', 'Geography',
            'Population', 'Income', 'Reputation', 'Language', 'Culture'
        ];
        
        const educationalRelevance = educationalKeywords.filter(keyword => 
            code.includes(keyword)
        ).length;
        
        return {
            relevance: educationalRelevance > 0 ? 'high' : 'medium',
            keywords: educationalKeywords.filter(keyword => code.includes(keyword))
        };
    }

    /**
     * Create educational explanation based on analysis
     */
    createEducationalExplanation(analysis, codeContent) {
        const explanations = {
            summary: this.generateSummary(analysis),
            breakdown: this.generateBreakdown(codeContent, analysis),
            educational_value: this.generateEducationalValue(analysis),
            real_world_application: this.generateRealWorldExample(analysis),
            next_steps: this.generateNextSteps(analysis),
            complexity_level: analysis.complexity,
            programming_concepts: analysis.concepts,
            child_friendly_tips: this.generateChildFriendlyTips(analysis)
        };

        return explanations;
    }

    /**
     * Generate child-friendly summary
     */
    generateSummary(analysis) {
        const summaries = {
            'classes': "This code creates a blueprint for something in our game - like having instructions to build a character or country! 🏗️",
            'functions': "This code creates a helper that does a specific job in our game - like a magic spell that makes something happen! ✨",
            'conditionals': "This code helps our game make smart decisions - like 'if the player has enough money, then they can buy a country!' 🤔",
            'loops': "This code makes our game repeat actions - like counting all the countries a player owns! 🔄",
            'async-programming': "This code lets our game do multiple things at once - like talking to other players while updating the map! ⚡",
            'game-programming': "This code brings our World Leaders educational game to life! 🎮",
            'web-development': "This code creates the web pages you see and interact with in our educational game! 🌐"
        };

        const mainConcept = analysis.concepts[0] || 'programming';
        return summaries[mainConcept] || 
            "This code helps make our educational game work so you can learn about geography and economics while having fun! 🌍";
    }

    /**
     * Generate step-by-step breakdown
     */
    generateBreakdown(codeContent, analysis) {
        const lines = codeContent.split('\n').filter(line => line.trim()).slice(0, 5);
        const breakdown = [];

        lines.forEach((line, index) => {
            let explanation = "This line helps our educational game work! ✨";
            
            if (line.includes('public class')) {
                explanation = "This creates a new part of our game - like making a new type of game piece! 🎲";
            } else if (line.includes('public async Task')) {
                explanation = "This creates a task our game can do - like calculating country costs or updating player scores! 📊";
            } else if (line.includes('if (') || line.includes('if(')) {
                explanation = "This helps our game make decisions - like checking if a player can afford a country! 🤔";
            } else if (line.includes('return ')) {
                explanation = "This gives back a result - like telling the player their final score! 🏆";
            } else if (line.includes('await ')) {
                explanation = "This waits for something to finish - like waiting for country data to load! ⏳";
            }

            breakdown.push({
                line: line.trim(),
                explanation: explanation,
                line_number: index + 1
            });
        });

        return breakdown;
    }

    /**
     * Generate educational value explanation
     */
    generateEducationalValue(analysis) {
        return {
            learning_objective: "Learn how programming creates educational games that teach real-world concepts",
            age_appropriate_concepts: [
                "Programming is like giving step-by-step instructions to a computer 📝",
                "Code helps create interactive learning games 🎮",
                "Good programming makes educational games fun and engaging ✨",
                "Each line of code has a specific purpose in making the game work 🔧"
            ],
            geography_economics_connection: this.generateSubjectConnection(analysis),
            life_skills: [
                "Problem-solving by breaking big tasks into smaller steps 🧩",
                "Logical thinking and planning ahead 🧠",
                "Attention to detail and following instructions carefully 🔍",
                "Learning from mistakes and trying again 💪"
            ]
        };
    }

    /**
     * Generate subject area connections
     */
    generateSubjectConnection(analysis) {
        if (analysis.educationalValue.keywords.includes('Territory') || 
            analysis.educationalValue.keywords.includes('Country')) {
            return "This code helps players learn about different countries and their locations around the world! 🌍";
        }
        
        if (analysis.educationalValue.keywords.includes('GDP') || 
            analysis.educationalValue.keywords.includes('Income')) {
            return "This code teaches economic concepts like how countries make money and manage resources! 💰";
        }
        
        if (analysis.educationalValue.keywords.includes('Language') || 
            analysis.educationalValue.keywords.includes('Culture')) {
            return "This code helps players discover different languages and cultures from around the world! 🗣️";
        }
        
        return "This code supports learning by making educational content interactive and engaging! 📚";
    }

    /**
     * Generate real-world examples
     */
    generateRealWorldExample(analysis) {
        const examples = {
            'classes': "Like having a recipe for making different types of cookies - each recipe (class) tells you exactly how to make that type! 🍪",
            'functions': "Like having different buttons on a remote control - each button does one specific thing when you press it! 📺",
            'conditionals': "Like the rules in a board game - 'if you roll a 6, then you get an extra turn!' 🎲",
            'loops': "Like doing jumping jacks - you repeat the same action over and over until you reach your goal! 🤸",
            'data-structures': "Like organizing your school supplies in different folders and binders to keep everything neat! 📁",
            'game-programming': "Like creating the rules for a new board game that teaches players about different countries! 🎯"
        };

        const mainConcept = analysis.concepts[0] || 'programming';
        return examples[mainConcept] || 
            "Like following a recipe to cook something delicious - each step is important and leads to a great result! 👨‍🍳";
    }

    /**
     * Generate next learning steps
     */
    generateNextSteps(analysis) {
        const steps = [
            "Try creating simple code with our interactive coding activities 💻",
            "Explore how different parts of the World Leaders Game work together 🔗",
            "Practice problem-solving with our educational programming puzzles 🧩",
            "Learn more about the countries featured in our game 🌍"
        ];

        // Add concept-specific next steps
        if (analysis.concepts.includes('functions')) {
            steps.push("Create your own simple functions in our coding playground! 🛠️");
        }
        
        if (analysis.concepts.includes('game-programming')) {
            steps.push("Design your own educational mini-game idea! 🎨");
        }

        return steps;
    }

    /**
     * Generate child-friendly tips
     */
    generateChildFriendlyTips(analysis) {
        const tips = [
            "💡 Don't worry if coding seems confusing at first - even expert programmers started as beginners!",
            "🚀 Every programmer makes mistakes - that's how we learn and get better!",
            "🎯 Start with small, simple projects and build up to bigger ones!",
            "👨‍👩‍👧‍👦 Ask parents, teachers, or friends to help when you get stuck!",
            "📚 Reading code is just as important as writing code!"
        ];

        return tips;
    }

    // Helper methods for parsing Azure OpenAI responses
    extractSummaryFromText(text) {
        // Try to extract summary from various formats
        const summaryPatterns = [
            /summary[:\s]*([^\n]+)/i,
            /what this does[:\s]*([^\n]+)/i,
            /in simple terms[:\s]*([^\n]+)/i,
            /^([^.!?]+[.!?])/
        ];

        for (const pattern of summaryPatterns) {
            const match = text.match(pattern);
            if (match && match[1]) {
                return match[1].trim();
            }
        }

        // Fallback: take first sentence
        const sentences = text.split(/[.!?]+/);
        return sentences[0] ? sentences[0].trim() + '!' : "🤖 This code does something interesting!";
    }

    extractBreakdownFromText(text) {
        // Try to extract step-by-step breakdown
        const stepPatterns = [
            /step[s]?\s*(?:by\s*step)?[:\s]*([^]+?)(?:\n\n|\n(?:[A-Z]|$))/i,
            /breakdown[:\s]*([^]+?)(?:\n\n|\n(?:[A-Z]|$))/i,
            /how it works[:\s]*([^]+?)(?:\n\n|\n(?:[A-Z]|$))/i
        ];

        for (const pattern of stepPatterns) {
            const match = text.match(pattern);
            if (match && match[1]) {
                return match[1].trim();
            }
        }

        // Fallback: split into paragraphs
        const paragraphs = text.split('\n\n');
        if (paragraphs.length > 1) {
            return paragraphs.slice(1).join('\n\n');
        }

        return "This code works by following logical steps to solve a problem! 🧩";
    }

    extractEducationalValueFromText(text) {
        // Try to extract educational connections
        const educationalPatterns = [
            /(?:geography|economics|learning|teaches)[:\s]*([^]+?)(?:\n\n|\n(?:[A-Z]|$))/i,
            /educational[:\s]*([^]+?)(?:\n\n|\n(?:[A-Z]|$))/i,
            /connection[:\s]*([^]+?)(?:\n\n|\n(?:[A-Z]|$))/i
        ];

        for (const pattern of educationalPatterns) {
            const match = text.match(pattern);
            if (match && match[1]) {
                return match[1].trim();
            }
        }

        return "This code teaches us about logical thinking and problem-solving - skills that help us understand how the world works! 🌍";
    }

    extractNextStepsFromText(text) {
        // Try to extract next steps or encouragement
        const nextStepPatterns = [
            /next steps?[:\s]*([^]+?)(?:\n\n|$)/i,
            /try[:\s]*([^]+?)(?:\n\n|$)/i,
            /keep[:\s]*([^]+?)(?:\n\n|$)/i
        ];

        for (const pattern of nextStepPatterns) {
            const match = text.match(pattern);
            if (match && match[1]) {
                return match[1].trim();
            }
        }

        return "Keep exploring and asking questions! Every coder started by being curious about how things work. You're doing great! 🌟";
    }

    /**
     * Create fallback explanation for errors
     */
    createFallbackExplanation() {
        return {
            summary: "This code helps our educational game teach you about geography and economics while having fun! 🌍",
            breakdown: [
                {
                    line: "// Code explanation temporarily unavailable",
                    explanation: "Don't worry - learning is a journey with ups and downs! 🌟",
                    line_number: 1
                }
            ],
            educational_value: {
                learning_objective: "Learn that programming helps create educational experiences",
                age_appropriate_concepts: [
                    "Programming is like giving instructions to a computer 💻",
                    "Code helps create fun learning games 🎮"
                ],
                life_skills: [
                    "Problem-solving and persistence 💪",
                    "Learning from challenges 🌟"
                ]
            },
            real_world_application: "Like following directions to get somewhere new - each step helps you reach your goal! 🗺️",
            next_steps: [
                "Ask a parent, teacher, or friend to help explain this code 👨‍👩‍👧‍👦",
                "Try our simpler coding activities to build understanding 📚",
                "Keep playing our educational game to see programming in action! 🎮"
            ],
            complexity_level: "beginner",
            programming_concepts: ["basic-programming"],
            child_friendly_tips: [
                "💡 It's okay when things don't work perfectly - that's how we learn!",
                "🚀 Every expert was once a beginner!",
                "📚 Learning takes time and practice!"
            ]
        };
    }

    // Helper methods for parsing Azure OpenAI responses
    extractSummaryFromText(text) {
        // Look for summary patterns in text
        const summaryMatch = text.match(/summary[:\s]*(.*?)(?:\n|$|\.)/i);
        if (summaryMatch) {
            return summaryMatch[1].trim();
        }
        
        // Fallback: use first sentence
        const firstSentence = text.split('.')[0];
        return firstSentence.length < 100 ? firstSentence + '.' : "🤖 This code does something cool!";
    }

    extractBreakdownFromText(text) {
        // Try to find step-by-step sections
        const steps = [];
        const lines = text.split('\n');
        
        for (let i = 0; i < lines.length; i++) {
            const line = lines[i].trim();
            if (line.match(/^\d+\.|step|first|second|third|then|next/i)) {
                steps.push({
                    line: line,
                    explanation: line,
                    line_number: i + 1
                });
            }
        }
        
        if (steps.length === 0) {
            return [{
                line: "// Code explanation",
                explanation: text.slice(0, 200) + "...",
                line_number: 1
            }];
        }
        
        return steps;
    }

    extractEducationalValueFromText(text) {
        // Look for educational connections
        const educationalMatch = text.match(/(geography|economics|learning|teaches|helps you)/gi);
        
        return {
            learning_objective: "Understand programming concepts through our educational game",
            age_appropriate_concepts: [
                "This code helps create fun learning experiences 🎮",
                "Programming is like giving step-by-step instructions 📝"
            ],
            life_skills: [
                "Logical thinking and problem-solving 🧠",
                "Understanding how technology works 💻"
            ]
        };
    }

    extractNextStepsFromText(text) {
        // Look for next steps or suggestions
        const nextStepsMatch = text.match(/next[:\s]*(.*?)(?:\n|$)/gi);
        
        if (nextStepsMatch && nextStepsMatch.length > 0) {
            return nextStepsMatch.map(step => step.replace(/next[:\s]*/i, '').trim());
        }
        
        return [
            "Try changing small parts of the code to see what happens! 🧪",
            "Ask questions about what each part does 🤔",
            "Keep exploring our educational game to see more examples! 🌍"
        ];
    }
}

// Export for use in Node.js/Express API
if (typeof module !== 'undefined' && module.exports) {
    module.exports = EducationalCodeExplanationAPI;
}

// Example Express.js route handler
/*
const express = require('express');
const router = express.Router();
const EducationalCodeExplanationAPI = require('./educational-code-explanation-api');

const codeExplainer = new EducationalCodeExplanationAPI();

router.post('/api/explain-code', async (req, res) => {
    try {
        const { code, context } = req.body;
        
        // Validate input
        if (!code || typeof code !== 'string') {
            return res.status(400).json({ 
                error: 'Code content is required',
                child_friendly_message: "Oops! We need some code to explain! 😅"
            });
        }
        
        // Generate explanation
        const explanation = await codeExplainer.generateExplanation(code, context);
        
        res.json({
            success: true,
            explanation: explanation,
            child_friendly_message: "Here's your code explanation! 🧑‍🏫"
        });
        
    } catch (error) {
        console.error('Code explanation API error:', error);
        
        res.status(500).json({
            error: 'Explanation generation failed',
            child_friendly_message: "Something went wrong, but that's okay! Learning has ups and downs! 🌟",
            fallback_explanation: codeExplainer.createFallbackExplanation()
        });
    }
});

module.exports = router;
*/

// Always make the class available globally
window.EducationalCodeExplanationAPI = EducationalCodeExplanationAPI;

} // End of class definition guard
