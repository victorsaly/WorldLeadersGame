/**
 * Code Explainer for World Leaders Game Educational Blog
 * Context: Educational game development for 12-year-old learners
 * Educational Objective: Teach programming concepts through interactive explanations
 * Safety Requirements: Age-appropriate content, encouraging messaging
 */

// Only define the class if it doesn't exist
if (typeof window.EducationalCodeExplainer === 'undefined') {

class EducationalCodeExplainer {
    constructor() {
        this.apiEndpoint = '/api/explain-code'; // Will implement this endpoint
        this.explanationCache = new Map();
        this.isEducationalMode = true;
        this.targetAge = 12;
        
        this.initializeExplainerButtons();
        this.setupStyles();
    }

    /**
     * Initialize explanation buttons on all code blocks
     */
    initializeExplainerButtons() {
        // Find all code blocks in the blog post
        const codeBlocks = document.querySelectorAll('pre code, .highlight code');
        
        codeBlocks.forEach((codeBlock, index) => {
            this.addExplainerButton(codeBlock, index);
        });
    }

    /**
     * Add explain button to a specific code block
     */
    addExplainerButton(codeBlock, index) {
        const codeContainer = codeBlock.closest('pre') || codeBlock.parentElement;
        const codeContent = codeBlock.textContent.trim();
        
        // Skip very short code snippets
        if (codeContent.length < 20) return;
        
        // Create button container
        const buttonContainer = document.createElement('div');
        buttonContainer.className = 'code-explainer-container';
        
        // Create explain button
        const explainButton = document.createElement('button');
        explainButton.className = 'btn-explain-code';
        explainButton.innerHTML = `
            <span class="explain-icon">🧑‍🏫</span>
            <span class="explain-text">Explain code</span>
        `;
        explainButton.setAttribute('data-code-index', index);
        explainButton.onclick = () => this.explainCode(codeContent, index);
        
        // Create explanation area
        const explanationArea = document.createElement('div');
        explanationArea.className = 'code-explanation-area';
        explanationArea.id = `explanation-${index}`;
        explanationArea.style.display = 'none';
        
        buttonContainer.appendChild(explainButton);
        buttonContainer.appendChild(explanationArea);
        
        // Insert after the code block
        codeContainer.insertAdjacentElement('afterend', buttonContainer);
    }

    /**
     * Explain code using our educational AI service
     */
    async explainCode(codeContent, index) {
        const button = document.querySelector(`[data-code-index="${index}"]`);
        const explanationArea = document.getElementById(`explanation-${index}`);
        
        // Check if elements exist
        if (!button || !explanationArea) {
            console.error('Missing DOM elements for code explanation:', { button: !!button, explanationArea: !!explanationArea });
            return;
        }
        
        // Check cache first
        if (this.explanationCache.has(codeContent)) {
            this.displayExplanation(this.explanationCache.get(codeContent), explanationArea);
            this.toggleExplanation(explanationArea, button);
            return;
        }
        
        // Show loading state
        button.innerHTML = `
            <span class="loading-spinner">⏳</span>
            <span class="explain-text">Explaining...</span>
        `;
        button.disabled = true;
        
        try {
            const explanation = await this.fetchExplanation(codeContent);
            this.explanationCache.set(codeContent, explanation);
            this.displayExplanation(explanation, explanationArea);
            this.toggleExplanation(explanationArea, button);
        } catch (error) {
            console.error('Code explanation failed:', error);
            this.displayError(explanationArea);
        } finally {
            // Reset button safely
            if (button) {
                button.innerHTML = `
                    <span class="explain-icon">🧑‍🏫</span>
                    <span class="explain-text">Explain code</span>
                `;
                button.disabled = false;
            }
        }
    }

    /**
     * Fetch explanation from our educational AI service
     */
    async fetchExplanation(codeContent) {
        try {
            // Check if we have the educational API available
            if (window.EducationalCodeExplanationAPI) {
                console.log('🎯 Using Educational AI API for explanation');
                const educationalAPI = new window.EducationalCodeExplanationAPI();
                return await educationalAPI.generateExplanation(codeContent);
            } else {
                console.log('⚠️ Educational API not loaded, using mock explanation');
                return this.generateMockExplanation(codeContent);
            }
        } catch (error) {
            console.error('Educational API failed, falling back to mock:', error);
            return this.generateMockExplanation(codeContent);
        }
    }

    /**
     * Generate mock explanation for demonstration
     * In production, this would use Azure OpenAI API
     */
    generateMockExplanation(codeContent) {
        // Detect code language and concepts
        const concepts = this.analyzeCodeConcepts(codeContent);
        
        return {
            summary: this.generateChildFriendlySummary(concepts),
            breakdown: this.generateStepByStepBreakdown(codeContent),
            educational_value: this.generateEducationalContext(concepts),
            real_world_application: this.generateRealWorldExample(concepts),
            next_steps: this.generateNextSteps(concepts)
        };
    }

    /**
     * Analyze code to identify key programming concepts
     */
    analyzeCodeConcepts(codeContent) {
        const concepts = [];
        
        // Basic concept detection
        if (codeContent.includes('class ')) concepts.push('classes');
        if (codeContent.includes('function ') || codeContent.includes('def ')) concepts.push('functions');
        if (codeContent.includes('if ')) concepts.push('conditionals');
        if (codeContent.includes('for ') || codeContent.includes('while ')) concepts.push('loops');
        if (codeContent.includes('async ') || codeContent.includes('await ')) concepts.push('async-programming');
        if (codeContent.includes('public class')) concepts.push('object-oriented-programming');
        if (codeContent.includes('[Parameter]')) concepts.push('blazor-components');
        if (codeContent.includes('Entity Framework') || codeContent.includes('DbContext')) concepts.push('database-programming');
        
        return concepts;
    }

    /**
     * Generate child-friendly summary
     */
    generateChildFriendlySummary(concepts) {
        const summaries = {
            'classes': "This code creates a blueprint (called a 'class') that defines how something works - like instructions for building a LEGO set! 🏗️",
            'functions': "This code creates a special helper (called a 'function') that does a specific job when you ask it to - like a robot that follows instructions! 🤖",
            'conditionals': "This code makes decisions using 'if-then' thinking - like 'if it's raining, then take an umbrella!' ☔",
            'loops': "This code repeats actions multiple times - like doing jumping jacks or counting to 100! 🔄",
            'blazor-components': "This code creates interactive parts of our educational game that you can click and play with! 🎮",
            'database-programming': "This code helps our game remember information about players and countries - like a digital filing cabinet! 📁"
        };
        
        const mainConcept = concepts[0] || 'programming';
        return summaries[mainConcept] || "This code helps our educational game teach you about geography and economics while having fun! 🌍";
    }

    /**
     * Generate step-by-step breakdown
     */
    generateStepByStepBreakdown(codeContent) {
        // Simple line-by-line analysis for educational purposes
        const lines = codeContent.split('\n').filter(line => line.trim());
        const breakdown = [];
        
        lines.slice(0, 5).forEach((line, index) => {
            if (line.trim().startsWith('//') || line.trim().startsWith('/*')) {
                breakdown.push({
                    line: line.trim(),
                    explanation: "This is a comment - it's like a note to help humans understand the code! 📝"
                });
            } else if (line.includes('public class')) {
                breakdown.push({
                    line: line.trim(),
                    explanation: "This creates a new blueprint for our educational game component! 🏗️"
                });
            } else if (line.includes('async') && line.includes('Task')) {
                breakdown.push({
                    line: line.trim(),
                    explanation: "This creates a task that can work in the background while other things happen! ⚡"
                });
            } else {
                breakdown.push({
                    line: line.trim(),
                    explanation: "This line helps our educational game work properly! ✨"
                });
            }
        });
        
        return breakdown;
    }

    /**
     * Generate educational context
     */
    generateEducationalContext(concepts) {
        return {
            learning_objective: "Understand how programming helps create educational games that teach geography and economics",
            age_appropriate_concepts: [
                "Programming is like giving instructions to a computer",
                "Code helps create interactive learning experiences",
                "Good code makes games fun and educational"
            ],
            real_world_skills: [
                "Problem-solving and logical thinking",
                "Breaking complex tasks into smaller steps",
                "Creating solutions that help others learn"
            ]
        };
    }

    /**
     * Generate real-world example
     */
    generateRealWorldExample(concepts) {
        const examples = {
            'classes': "Just like how a recipe tells you how to make cookies, a class tells the computer how to create game components!",
            'functions': "Like having a special button that always does the same helpful thing when you press it!",
            'conditionals': "Like the rules in a board game - 'if you land on this space, then something special happens!'",
            'database-programming': "Like organizing your trading cards or stickers in albums so you can find them easily!"
        };
        
        const mainConcept = concepts[0] || 'programming';
        return examples[mainConcept] || "This helps create educational games that make learning about the world fun and interactive!";
    }

    /**
     * Generate next learning steps
     */
    generateNextSteps(concepts) {
        return [
            "Try creating your own simple code with our educational coding activities",
            "Explore how different parts of our World Leaders Game work together",
            "Learn more about geography and economics through our interactive features",
            "Ask a parent or teacher to help you explore programming concepts"
        ];
    }

    /**
     * Display explanation in the UI
     */
    displayExplanation(explanation, explanationArea) {
        explanationArea.innerHTML = `
            <div class="explanation-content">
                <div class="explanation-header">
                    <h4>🧑‍🏫 Code Explanation for Young Learners</h4>
                </div>
                
                <div class="explanation-summary">
                    <h5>📖 What This Code Does:</h5>
                    <p>${explanation.summary}</p>
                </div>
                
                <div class="explanation-breakdown">
                    <h5>🔍 Step by Step:</h5>
                    <ul>
                        ${explanation.breakdown.map(item => `
                            <li>
                                <code class="inline-code">${item.line}</code>
                                <p>${item.explanation}</p>
                            </li>
                        `).join('')}
                    </ul>
                </div>
                
                <div class="explanation-educational">
                    <h5>🎯 Why This Matters:</h5>
                    <p>${explanation.educational_value.learning_objective}</p>
                    <ul>
                        ${explanation.educational_value.age_appropriate_concepts.map(concept => 
                            `<li>${concept}</li>`
                        ).join('')}
                    </ul>
                </div>
                
                <div class="explanation-real-world">
                    <h5>🌍 Real World Example:</h5>
                    <p>${explanation.real_world_application}</p>
                </div>
                
                <div class="explanation-next-steps">
                    <h5>🚀 Next Steps for Learning:</h5>
                    <ul>
                        ${explanation.next_steps.map(step => `<li>${step}</li>`).join('')}
                    </ul>
                </div>
                
                <div class="explanation-footer">
                    <p class="educational-note">
                        💡 <strong>For Parents & Teachers:</strong> This explanation is designed to help 12-year-olds 
                        understand programming concepts while learning about geography and economics through our educational game.
                    </p>
                </div>
            </div>
        `;
    }

    /**
     * Display error message
     */
    displayError(explanationArea) {
        explanationArea.innerHTML = `
            <div class="explanation-error">
                <h4>😅 Oops! Something went wrong</h4>
                <p>We couldn't explain this code right now, but that's okay! Learning is a journey with ups and downs.</p>
                <p>💡 Try asking a parent, teacher, or friend to help you understand this code!</p>
            </div>
        `;
    }

    /**
     * Toggle explanation visibility
     */
    toggleExplanation(explanationArea, button) {
        if (!explanationArea || !button) {
            console.warn('Missing elements for toggle explanation');
            return;
        }

        const explainText = button.querySelector('.explain-text');
        const explainIcon = button.querySelector('.explain-icon');

        if (explanationArea.style.display === 'none') {
            explanationArea.style.display = 'block';
            if (explainText) explainText.textContent = 'Hide explanation';
            if (explainIcon) explainIcon.textContent = '📖';
        } else {
            explanationArea.style.display = 'none';
            if (explainText) explainText.textContent = 'Explain code';
            if (explainIcon) explainIcon.textContent = '🧑‍🏫';
        }
    }

    /**
     * Setup CSS styles for the code explainer
     */
    setupStyles() {
        if (document.getElementById('code-explainer-styles')) return;
        
        const styles = document.createElement('style');
        styles.id = 'code-explainer-styles';
        styles.textContent = `
            /* Educational Code Explainer Styles */
            .code-explainer-container {
                margin: 16px 0;
                background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
                border-radius: 12px;
                padding: 16px;
                border: 2px solid #dee2e6;
                box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            }
            
            .btn-explain-code {
                background: linear-gradient(135deg, #4CAF50 0%, #45a049 100%);
                color: white;
                border: none;
                padding: 12px 24px;
                border-radius: 8px;
                font-size: 16px;
                font-weight: bold;
                cursor: pointer;
                display: flex;
                align-items: center;
                gap: 8px;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
                transition: all 0.3s ease;
                font-family: 'Comic Neue', 'Arial Rounded', sans-serif;
            }
            
            .btn-explain-code:hover {
                background: linear-gradient(135deg, #45a049 0%, #4CAF50 100%);
                transform: translateY(-2px);
                box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
            }
            
            .btn-explain-code:disabled {
                opacity: 0.7;
                cursor: not-allowed;
                transform: none;
            }
            
            .explain-icon {
                font-size: 18px;
            }
            
            .loading-spinner {
                animation: spin 1s linear infinite;
            }
            
            @keyframes spin {
                from { transform: rotate(0deg); }
                to { transform: rotate(360deg); }
            }
            
            .code-explanation-area {
                margin-top: 16px;
                animation: slideDown 0.3s ease-out;
            }
            
            @keyframes slideDown {
                from {
                    opacity: 0;
                    transform: translateY(-10px);
                }
                to {
                    opacity: 1;
                    transform: translateY(0);
                }
            }
            
            .explanation-content {
                background: white;
                border-radius: 12px;
                padding: 24px;
                border: 2px solid #e9ecef;
                box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
            }
            
            .explanation-header h4 {
                color: #2c3e50;
                font-family: 'Comic Neue', 'Arial Rounded', sans-serif;
                margin: 0 0 20px 0;
                font-size: 20px;
            }
            
            .explanation-summary,
            .explanation-breakdown,
            .explanation-educational,
            .explanation-real-world,
            .explanation-next-steps {
                margin-bottom: 20px;
                padding-bottom: 16px;
                border-bottom: 1px solid #f1f3f4;
            }
            
            .explanation-summary h5,
            .explanation-breakdown h5,
            .explanation-educational h5,
            .explanation-real-world h5,
            .explanation-next-steps h5 {
                color: #495057;
                font-family: 'Comic Neue', 'Arial Rounded', sans-serif;
                margin: 0 0 12px 0;
                font-size: 16px;
            }
            
            .explanation-summary p,
            .explanation-educational p,
            .explanation-real-world p {
                font-size: 16px;
                line-height: 1.6;
                color: #495057;
                margin: 0;
            }
            
            .explanation-breakdown ul,
            .explanation-educational ul,
            .explanation-next-steps ul {
                list-style: none;
                padding: 0;
                margin: 0;
            }
            
            .explanation-breakdown li {
                margin-bottom: 12px;
                padding: 12px;
                background: #f8f9fa;
                border-radius: 8px;
                border-left: 4px solid #4CAF50;
            }
            
            .explanation-educational li,
            .explanation-next-steps li {
                margin-bottom: 8px;
                padding: 8px 0;
                position: relative;
                padding-left: 24px;
            }
            
            .explanation-educational li:before,
            .explanation-next-steps li:before {
                content: "✨";
                position: absolute;
                left: 0;
                top: 8px;
                color: #4CAF50;
            }
            
            .inline-code {
                background: #e9ecef;
                padding: 2px 6px;
                border-radius: 4px;
                font-family: 'Monaco', 'Menlo', 'Ubuntu Mono', monospace;
                font-size: 14px;
                color: #e83e8c;
                font-weight: bold;
            }
            
            .explanation-footer {
                background: linear-gradient(135deg, #e3f2fd 0%, #f3e5f5 100%);
                padding: 16px;
                border-radius: 8px;
                margin-top: 20px;
                border: 1px solid #e1f5fe;
            }
            
            .educational-note {
                margin: 0;
                font-size: 14px;
                color: #37474f;
                font-style: italic;
            }
            
            .explanation-error {
                background: linear-gradient(135deg, #fff3e0 0%, #ffcc80 100%);
                padding: 20px;
                border-radius: 8px;
                text-align: center;
                border: 2px solid #ffb74d;
            }
            
            .explanation-error h4 {
                color: #e65100;
                margin: 0 0 12px 0;
                font-family: 'Comic Neue', 'Arial Rounded', sans-serif;
            }
            
            .explanation-error p {
                color: #ef6c00;
                margin: 8px 0;
                font-size: 16px;
            }
            
            /* Mobile responsiveness for child-friendly design */
            @media (max-width: 768px) {
                .code-explainer-container {
                    margin: 12px 0;
                    padding: 12px;
                }
                
                .btn-explain-code {
                    width: 100%;
                    justify-content: center;
                    padding: 16px 24px;
                    font-size: 18px;
                }
                
                .explanation-content {
                    padding: 16px;
                }
                
                .explanation-header h4 {
                    font-size: 18px;
                }
            }
        `;
        
        document.head.appendChild(styles);
    }
}

// Export for potential Node.js usage
if (typeof module !== 'undefined' && module.exports) {
    module.exports = EducationalCodeExplainer;
}

// Always make the class available globally
window.EducationalCodeExplainer = EducationalCodeExplainer;

} // End of class definition guard
