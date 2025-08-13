# Simple Code Explanation Template

Use this template to add explanations to code blocks in blog posts:

## Template

```html
<details class="code-explanation">
<summary>💡 <strong>Explain Code</strong></summary>
<div class="explanation-content">
<p>[Your explanation here - focus on architectural patterns, design decisions, and practical value for adult developers]</p>
</div>
</details>
```

## Examples of Good Explanations

### For C# Services
```html
<details class="code-explanation">
<summary>💡 <strong>Explain Code</strong></summary>
<div class="explanation-content">
<p>This C# service demonstrates enterprise-grade dependency injection with async/await patterns for AI content generation. It uses the strategy pattern for agent personalities, implements comprehensive error handling with try-catch blocks, and follows the single responsibility principle by separating content generation from validation.</p>
</div>
</details>
```

### For CSS/Styling
```html
<details class="code-explanation">
<summary>💡 <strong>Explain Code</strong></summary>
<div class="explanation-content">
<p>This CSS creates a retro 8-bit game aesthetic using multiple techniques: thick borders instead of border-radius for sharp edges, layered box-shadows to create the classic "raised button" effect from old games, CSS custom properties (variables) for consistent theming, and image-rendering: pixelated to maintain sharp pixel art appearance when scaled.</p>
</div>
</details>
```

### For JavaScript/TypeScript
```html
<details class="code-explanation">
<summary>💡 <strong>Explain Code</strong></summary>
<div class="explanation-content">
<p>This TypeScript comment demonstrates the difference between generic task assignment and contextual mission-driven instructions. When you provide AI with specific context about the target audience, the AI automatically incorporates relevant design patterns like larger touch targets, positive reinforcement messaging, and accessibility considerations without explicit instruction.</p>
</div>
</details>
```

### For Bash Scripts
```html
<details class="code-explanation">
<summary>💡 <strong>Explain Code</strong></summary>
<div class="explanation-content">
<p>This bash script implements automated testing for production deployments using curl for HTTP health checks and openssl for SSL certificate validation. It demonstrates defensive programming with error checking, uses conditional statements for validation, and provides clear status reporting. This is a common pattern in DevOps for post-deployment verification.</p>
</div>
</details>
```

## What Makes a Good Explanation

1. **Focus on Architecture**: Explain design patterns, architectural decisions
2. **Practical Value**: Why this code is useful, what problems it solves
3. **Technical Depth**: Assume the reader knows programming but may not know the specific frameworks
4. **Context**: Connect to the broader application or system
5. **Keep it Concise**: One paragraph that adds real value

## Placement

Always place the explanation **immediately before** the code block:

```markdown
Some text leading to the code...

<details class="code-explanation">
<summary>💡 <strong>Explain Code</strong></summary>
<div class="explanation-content">
<p>Your explanation here.</p>
</div>
</details>

```language
// Your code here
```
```

This ensures the explanation is contextually relevant and easy to find.
