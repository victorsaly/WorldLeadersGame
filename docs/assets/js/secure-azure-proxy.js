/**
 * Secure OpenAI Proxy for Educational Code Explainer
 * Domain-restricted and token-obfuscated API calls
 * Context: Educational game development for 12-year-old learners
 * Security: Only works on authorized domains
 */

// Only define classes if they don't exist on window
if (typeof window.SecureAzureProxy === 'undefined') {
class SecureAzureProxy {
    constructor() {
        this.authorizedDomains = [
            'localhost',
            '127.0.0.1',
            'docs.worldleadersgame.co.uk'
        ];
        
        this.isSecureContext = this.validateDomain();
        this.obfuscatedConfig = null;
        
        if (this.isSecureContext) {
            this.initializeSecureConfig();
        }
    }

    validateDomain() {
        const currentDomain = window.location.hostname;
        const isAuthorized = this.authorizedDomains.some(domain => {
            return currentDomain === domain || currentDomain.endsWith('.' + domain);
        });

        if (!isAuthorized) {
            console.warn('🚫 Domain not authorized for OpenAI access');
            console.log('✅ Authorized domains:', this.authorizedDomains);
            console.log('❌ Current domain:', currentDomain);
        }

        return isAuthorized;
    }

    initializeSecureConfig() {
        // OpenAI API configuration (base64 encoded for basic obfuscation)
        
        // OpenAI API endpoint (base64 encoded)
        const obfuscatedEndpoint = this.deobfuscate(
            'aHR0cHM6Ly9hcGkub3BlbmFpLmNvbS92MS9jaGF0L2NvbXBsZXRpb25z' // https://api.openai.com/v1/chat/completions
        );

        // Your OpenAI API key (base64 encoded)
        const obfuscatedApiKey = this.deobfuscate(
            'c2stcHJvai1jbmJ0RHNjWUEtekFKcEptWGg3ZlFfWHFtNWM0b2RUR0ZnT0ZydTNpSjgtNUlpaGk4b09CalhlSTYtclJVaWZ6Y0EyQy1KMkFCQ1QzQmxia0ZKY0FqQ3lQOFo0Wnktc1V1bWM1eVg3aWM5UEh4R2RoYjZLZWhIYWhtdm5CZ0xmNnlqR0tYQ0E3QTgwQlZqU2E5NFhUZGRjaEE3c0E='
        );

        // Only set config if domain is authorized AND we have real credentials
        if (this.isSecureContext && 
            obfuscatedEndpoint && 
            obfuscatedApiKey) {
            
            this.obfuscatedConfig = {
                endpoint: obfuscatedEndpoint,
                apiKey: obfuscatedApiKey,
                model: 'gpt-4o-mini', // Using cost-effective model for educational content
                isOpenAI: true // Flag to indicate this is OpenAI, not Azure
            };
        }
    }

    deobfuscate(encodedValue) {
        try {
            return atob(encodedValue);
        } catch (error) {
            console.error('🔒 Configuration deobfuscation failed');
            return null;
        }
    }

    async makeSecureAPICall(prompt, context = {}) {
        // Triple security check
        if (!this.isSecureContext) {
            throw new Error('Domain not authorized for API access');
        }

        if (!this.obfuscatedConfig || !this.obfuscatedConfig.apiKey) {
            throw new Error('Secure configuration not available');
        }

        // Additional runtime domain check
        if (!this.validateDomain()) {
            throw new Error('Runtime domain validation failed');
        }

        try {
            const response = await this.executeSecureAPICall(prompt, context);
            return response;
        } catch (error) {
            console.error('🔒 Secure API call failed:', error.message);
            throw error;
        }
    }

    async executeSecureAPICall(prompt, context) {
        const payload = {
            model: this.obfuscatedConfig.model || 'gpt-4o-mini',
            messages: [
                {
                    role: 'system',
                    content: `You are an encouraging programming teacher for 12-year-old students learning through an educational geography and economics game. 

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
- Include notes for parents/teachers when helpful`
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
        };

        // Use OpenAI API format
        const requestUrl = this.obfuscatedConfig.endpoint;
        const headers = {
            'Content-Type': 'application/json',
            'Authorization': `Bearer ${this.obfuscatedConfig.apiKey}`,
            'User-Agent': 'WorldLeadersGame-Educational-Explainer/1.0',
            'X-Educational-Context': 'child-safe-learning'
        };

        const response = await fetch(requestUrl, {
            method: 'POST',
            headers: headers,
            body: JSON.stringify(payload)
        });

        if (!response.ok) {
            const errorText = await response.text();
            throw new Error(`OpenAI API error: ${response.status} - ${errorText}`);
        }

        const data = await response.json();
        
        if (!data.choices || !data.choices[0] || !data.choices[0].message) {
            throw new Error('Invalid response structure from OpenAI API');
        }

        return data.choices[0].message.content;
    }

    // Check if secure API is available
    isSecureAPIAvailable() {
        return this.isSecureContext && 
               this.obfuscatedConfig && 
               this.obfuscatedConfig.apiKey &&
               !this.obfuscatedConfig.apiKey.includes('your-api-key');
    }

    // Get domain authorization status
    getDomainStatus() {
        return {
            currentDomain: window.location.hostname,
            isAuthorized: this.isSecureContext,
            authorizedDomains: this.authorizedDomains,
            apiAvailable: this.isSecureAPIAvailable()
        };
    }
}

// Simple configuration helper
class SecureConfigurationHelper {
    constructor() {
        this.secureProxy = new SecureAzureProxy();
        console.log('🔒 Secure configuration helper initialized (auto-config mode)');
    }

    // Simple method to check if API is ready
    isAPIReady() {
        return this.secureProxy.isSecureAPIAvailable();
    }

    // Get status information
    getStatus() {
        return this.secureProxy.getDomainStatus();
    }
}

// Initialize secure configuration helper
document.addEventListener('DOMContentLoaded', function() {
    window.secureConfigHelper = new SecureConfigurationHelper();
    window.secureAzureProxy = window.secureConfigHelper.secureProxy;
    
    // Log initialization status
    const status = window.secureConfigHelper.getStatus();
    console.log('🔒 Secure proxy status:', status);
});

// Always make the classes available globally
window.SecureAzureProxy = SecureAzureProxy;
window.SecureConfigurationHelper = SecureConfigurationHelper;

} // End of class definition guard
