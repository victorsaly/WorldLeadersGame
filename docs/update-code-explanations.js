#!/usr/bin/env node

/**
 * Simple Code Explanation Updater for Blog Posts
 * Adds simple, hidden explanations to code blocks that can be toggled
 * Target audience: Adult developers on dev.to/medium.com
 */

const fs = require('fs');
const path = require('path');

class SimpleCodeExplainer {
    constructor() {
        this.postsDir = path.join(__dirname, '_posts');
        this.explanations = {
            // C# patterns
            'public class': 'This defines a public class that can be accessed from other parts of the application.',
            'private readonly': 'This creates a private field that can only be set during initialization.',
            'async Task': 'This defines an asynchronous method that returns a Task, allowing non-blocking execution.',
            'await ': 'This waits for an asynchronous operation to complete before continuing.',
            'IServiceCollection': 'This is the dependency injection container where services are registered.',
            'services.AddScoped': 'This registers a service with scoped lifetime (one instance per request).',
            'services.AddSingleton': 'This registers a service with singleton lifetime (one instance for the entire application).',
            '[Parameter]': 'This Blazor attribute marks a property as a component parameter that can be set from parent components.',
            'ComponentBase': 'This is the base class for Blazor components providing lifecycle methods and rendering capabilities.',
            'StateHasChanged()': 'This tells Blazor to re-render the component because its state has changed.',
            'OnInitializedAsync': 'This Blazor lifecycle method runs when the component is first initialized.',
            'RenderFragment': 'This represents a piece of UI content that can be rendered by Blazor components.',
            'DbContext': 'This Entity Framework class represents a session with the database and provides access to entities.',
            'Entity Framework': 'This is Microsoft\'s ORM (Object-Relational Mapping) framework for data access.',

            // JavaScript patterns
            'async function': 'This defines an asynchronous function that can use await and returns a Promise.',
            'const ': 'This declares a constant variable that cannot be reassigned after initialization.',
            'addEventListener': 'This attaches an event listener to an element to handle user interactions.',
            'querySelector': 'This finds the first element in the DOM that matches the specified CSS selector.',
            'fetch(': 'This makes an HTTP request to retrieve data from a server.',
            'JSON.parse': 'This converts a JSON string into a JavaScript object.',
            'document.createElement': 'This creates a new HTML element that can be added to the DOM.',

            // Azure and Cloud patterns
            'Azure OpenAI': 'This is Microsoft\'s cloud service providing access to advanced AI models like GPT-4.',
            'App Service': 'This is Azure\'s platform for hosting web applications with built-in scaling and monitoring.',
            'Application Insights': 'This is Azure\'s application performance monitoring service for tracking metrics and errors.',
            'Key Vault': 'This is Azure\'s service for securely storing and managing secrets, keys, and certificates.',

            // Docker patterns
            'FROM ': 'This specifies the base image that this Docker container will be built upon.',
            'COPY ': 'This copies files from the host system into the Docker container.',
            'RUN ': 'This executes a command during the Docker image build process.',
            'EXPOSE ': 'This documents which port the containerized application will listen on.',

            // General programming patterns
            'try {': 'This starts a try-catch block for handling exceptions that might occur during execution.',
            'catch (': 'This catches and handles exceptions thrown in the corresponding try block.',
            'finally {': 'This block always executes regardless of whether an exception occurred.',
            'if (': 'This conditional statement executes code only when the specified condition is true.',
            'for (': 'This loop executes code repeatedly for a specified number of iterations.',
            'while (': 'This loop continues executing code as long as the specified condition remains true.',
            'return ': 'This exits the function and optionally returns a value to the caller.',
        };
    }

    async updateAllPosts() {
        console.log('🔄 Starting to update code explanations in blog posts...');
        
        try {
            const files = fs.readdirSync(this.postsDir).filter(file => file.endsWith('.md'));
            console.log(`📁 Found ${files.length} markdown files to process`);

            for (const file of files) {
                await this.updatePost(file);
            }

            console.log('✅ All posts updated successfully!');
        } catch (error) {
            console.error('❌ Error updating posts:', error);
        }
    }

    async updatePost(filename) {
        const filePath = path.join(this.postsDir, filename);
        console.log(`📝 Processing: ${filename}`);

        try {
            let content = fs.readFileSync(filePath, 'utf8');
            let updated = false;

            // Find all code blocks (both ``` and indented)
            const codeBlockRegex = /```[\s\S]*?```/g;
            
            content = content.replace(codeBlockRegex, (match) => {
                const explanation = this.generateExplanation(match);
                if (explanation) {
                    updated = true;
                    return this.wrapCodeWithExplanation(match, explanation);
                }
                return match;
            });

            if (updated) {
                fs.writeFileSync(filePath, content, 'utf8');
                console.log(`  ✅ Updated: ${filename}`);
            } else {
                console.log(`  ⏭️  No changes needed: ${filename}`);
            }

        } catch (error) {
            console.error(`  ❌ Error processing ${filename}:`, error);
        }
    }

    generateExplanation(codeBlock) {
        // Extract the actual code content
        const codeContent = codeBlock.replace(/```[\w]*\n?/g, '').replace(/```$/, '').trim();
        
        // Skip very short code blocks
        if (codeContent.length < 20) return null;

        // Find the most relevant explanation based on patterns
        let bestMatch = null;
        let bestPattern = '';

        for (const [pattern, explanation] of Object.entries(this.explanations)) {
            if (codeContent.includes(pattern) && pattern.length > bestPattern.length) {
                bestMatch = explanation;
                bestPattern = pattern;
            }
        }

        // If no specific pattern matched, provide a generic explanation
        if (!bestMatch) {
            if (codeContent.includes('class ')) {
                bestMatch = 'This code defines a class structure that encapsulates data and methods for specific functionality.';
            } else if (codeContent.includes('function ') || codeContent.includes('def ')) {
                bestMatch = 'This code defines a function that performs a specific task and can be called from other parts of the program.';
            } else if (codeContent.includes('var ') || codeContent.includes('let ') || codeContent.includes('const ')) {
                bestMatch = 'This code declares variables to store and manipulate data values.';
            } else {
                bestMatch = 'This code snippet demonstrates a programming pattern commonly used in software development.';
            }
        }

        return bestMatch;
    }

    wrapCodeWithExplanation(codeBlock, explanation) {
        // Check if already has explanation wrapper
        if (codeBlock.includes('<details class="code-explanation">')) {
            return codeBlock;
        }

        return `<details class="code-explanation">
<summary>💡 <strong>Explain Code</strong></summary>
<div class="explanation-content">
<p>${explanation}</p>
</div>
</details>

${codeBlock}`;
    }
}

// Run if called directly
if (require.main === module) {
    const updater = new SimpleCodeExplainer();
    updater.updateAllPosts();
}

module.exports = SimpleCodeExplainer;
