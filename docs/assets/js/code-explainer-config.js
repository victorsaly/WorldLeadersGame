/**
 * Educational Code Explainer Configuration
 * Context: Educational game development for young learners
 * Educational Objective: Provide customizable, age-appropriate code explanations
 * Safety Requirements: Child-safe content, encouraging messaging
 */

const EducationalCodeExplainerConfig = {
    // Target demographics
    targetAge: 12,
    readingLevel: 6, // 6th grade reading level
    
    // Educational context
    educationalMission: {
        primarySubjects: ['geography', 'economics', 'programming'],
        learningObjectives: [
            'digital-literacy',
            'logical-thinking',
            'world-awareness',
            'economic-understanding',
            'problem-solving'
        ],
        gameContext: 'world-leaders-educational-game'
    },
    
    // Safety and appropriateness
    childSafety: {
        requiresParentalGuidance: true,
        maxSessionDuration: 30, // minutes
        encouragingLanguageOnly: true,
        noComplexJargon: true,
        includeParentNotes: true
    },
    
    // Explanation templates
    explanationTemplates: {
        summary: {
            maxWords: 25,
            requiresEmoji: true,
            tone: 'encouraging',
            analogyRequired: true
        },
        
        stepByStep: {
            maxSteps: 5,
            useSimpleVocabulary: true,
            includeVisualCues: true,
            connectToRealWorld: true
        },
        
        educationalValue: {
            connectToSubjects: ['geography', 'economics'],
            lifeSkillsConnection: true,
            futureApplications: true,
            ageAppropriateExamples: true
        },
        
        realWorldAnalogy: {
            sources: ['games', 'crafts', 'sports', 'cooking', 'school'],
            avoidAbstractConcepts: true,
            useKidFriendlyMetaphors: true,
            maxComplexity: 'elementary'
        },
        
        nextSteps: {
            maxSuggestions: 4,
            includeHandsOnActivities: true,
            suggestParentalInvolvement: true,
            connectToEducationalGame: true
        }
    },
    
    // Programming concept mappings for 12-year-olds
    conceptMappings: {
        'classes': {
            kidFriendlyName: 'blueprints',
            analogy: 'LEGO instruction manual',
            explanation: 'A set of instructions that tells the computer how to build something',
            realWorldExample: 'Like a recipe that tells you how to make cookies'
        },
        
        'functions': {
            kidFriendlyName: 'helpers',
            analogy: 'magic buttons',
            explanation: 'Special helpers that do one job when you ask them to',
            realWorldExample: 'Like a calculator button that always adds numbers'
        },
        
        'conditionals': {
            kidFriendlyName: 'decision makers',
            analogy: 'if-then rules',
            explanation: 'Code that makes decisions based on what\'s happening',
            realWorldExample: 'Like "if it\'s raining, then take an umbrella"'
        },
        
        'loops': {
            kidFriendlyName: 'repeaters',
            analogy: 'doing jumping jacks',
            explanation: 'Code that repeats the same action multiple times',
            realWorldExample: 'Like counting from 1 to 100'
        },
        
        'variables': {
            kidFriendlyName: 'storage boxes',
            analogy: 'labeled containers',
            explanation: 'Special boxes that hold information for the computer',
            realWorldExample: 'Like writing your name on a folder to remember what\'s inside'
        },
        
        'arrays': {
            kidFriendlyName: 'lists',
            analogy: 'shopping lists',
            explanation: 'A way to keep track of multiple things in order',
            realWorldExample: 'Like a list of your favorite movies'
        },
        
        'async-programming': {
            kidFriendlyName: 'multitasking',
            analogy: 'doing homework while music plays',
            explanation: 'Letting the computer do several things at the same time',
            realWorldExample: 'Like listening to music while drawing a picture'
        },
        
        'object-oriented-programming': {
            kidFriendlyName: 'organizing with groups',
            analogy: 'sorting toys into categories',
            explanation: 'A way to organize code by grouping related things together',
            realWorldExample: 'Like organizing your room with everything in its proper place'
        }
    },
    
    // Subject-specific connections
    subjectConnections: {
        geography: {
            keywords: ['Country', 'Territory', 'Map', 'Location', 'Region'],
            connectionPhrases: [
                'helps players learn about different countries',
                'teaches geography through interactive exploration',
                'connects programming to world knowledge'
            ]
        },
        
        economics: {
            keywords: ['GDP', 'Income', 'Cost', 'Budget', 'Economy', 'Money'],
            connectionPhrases: [
                'teaches economic concepts through gameplay',
                'helps understand how countries manage money',
                'connects programming to real-world economics'
            ]
        },
        
        programming: {
            keywords: ['Code', 'Function', 'Class', 'Variable', 'Logic'],
            connectionPhrases: [
                'builds logical thinking skills',
                'teaches problem-solving through code',
                'develops computational thinking'
            ]
        }
    },
    
    // Encouraging phrases for different situations
    encouragingPhrases: {
        success: [
            '🌟 Fantastic job! You\'re becoming a programming expert!',
            '⭐ Amazing work! You really understand this concept!',
            '🎉 Incredible! You\'re thinking like a real programmer!',
            '💫 Outstanding! You\'re connecting the dots perfectly!'
        ],
        
        effort: [
            '💪 Great try! Every expert was once a beginner!',
            '🚀 You\'re doing amazing work exploring programming!',
            '✨ Keep going! Learning takes practice and you\'re doing great!',
            '🌈 Wonderful effort! Programming is all about trying and learning!'
        ],
        
        discovery: [
            '🔍 You\'re discovering how programming works!',
            '🧭 Great exploration! You\'re learning step by step!',
            '🗺️ You\'re mapping out how code creates our educational game!',
            '🧩 Perfect! You\'re putting the programming puzzle together!'
        ]
    },
    
    // Parent and teacher guidance
    educatorGuidance: {
        parentNotes: {
            include: true,
            topics: [
                'how this connects to child\'s education',
                'ways to support learning at home',
                'age-appropriate next steps',
                'screen time recommendations'
            ]
        },
        
        teacherResources: {
            include: true,
            topics: [
                'curriculum connections',
                'assessment opportunities',
                'classroom discussion prompts',
                'cross-curricular extensions'
            ]
        }
    },
    
    // Accessibility and inclusivity
    accessibility: {
        useSimpleLanguage: true,
        includeVisualCues: true,
        supportMultipleLearningStyles: true,
        provideDifferentExplanationLevels: true,
        includeAudioDescriptions: false, // Future feature
        supportRightToLeft: false // Future feature for Arabic
    },
    
    // API integration settings
    apiConfig: {
        endpoint: '/api/explain-code',
        timeout: 10000, // 10 seconds
        retryAttempts: 3,
        cacheExplanations: true,
        fallbackToMockExplanations: true
    },
    
    // Feature flags
    features: {
        interactiveCodePlayground: false, // Coming soon
        studentCodeSharing: false, // Coming soon
        progressTracking: false, // Coming soon
        multiLanguageSupport: false, // Future feature
        voiceExplanations: false, // Future feature
        parentDashboard: false // Coming soon
    }
};

// Export configuration
if (typeof module !== 'undefined' && module.exports) {
    module.exports = EducationalCodeExplainerConfig;
}

// Make available globally for browser use
if (typeof window !== 'undefined') {
    window.EducationalCodeExplainerConfig = EducationalCodeExplainerConfig;
}
