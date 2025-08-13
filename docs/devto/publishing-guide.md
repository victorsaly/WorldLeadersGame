# 📱 Dev.to Publishing Guide

Complete guide for converting Jekyll blog posts to dev.to format with proper formatting, image hosting, and deployment validation.

## 🎯 Overview

This guide covers the complete workflow for publishing Jekyll blog posts to dev.to while maintaining:
- ✅ **Educational quality** and technical accuracy
- ✅ **Image hosting** on `docs.worldleadersgame.co.uk`
- ✅ **Dev.to compatibility** with ASCII diagrams
- ✅ **Community engagement** optimization
- ✅ **SEO and discoverability** best practices

## 📋 Pre-Publishing Checklist

### Content Validation
- [ ] **Educational Value**: Clear learning objectives for readers
- [ ] **Technical Accuracy**: All code examples tested and functional
- [ ] **Age-Appropriate**: Content suitable for professional development community
- [ ] **Original Content**: No copyright violations or plagiarism
- [ ] **Complete Article**: Introduction, body, conclusion, and call-to-action

### Dev.to Technical Requirements
- [ ] **Frontmatter Format**: Proper YAML frontmatter with required fields
- [ ] **Tag Compliance**: Maximum 4 tags, following dev.to conventions
- [ ] **Image URLs**: All images hosted on `docs.worldleadersgame.co.uk`
- [ ] **Diagram Format**: ASCII/Unicode diagrams (no Mermaid)
- [ ] **Link Validation**: All internal links converted to external URLs
- [ ] **Markdown Compatibility**: Dev.to markdown syntax compliance

### Engagement Optimization
- [ ] **Compelling Title**: Clear value proposition under 50 characters
- [ ] **TL;DR Section**: Engaging summary with key takeaways
- [ ] **Discussion Questions**: 3-5 questions to encourage comments
- [ ] **Call-to-Action**: Clear next steps for readers
- [ ] **Series Attribution**: Proper series linking if applicable

## 🔄 Conversion Workflow

### Step 1: Content Preparation

```bash
# Copy Jekyll post to working directory
cp _posts/YYYY-MM-DD-article-title.md devto/working/
```

### Step 2: Frontmatter Conversion

**From Jekyll:**
```yaml
---
layout: post
title: "Article Title"
date: 2025-08-02
categories: ["development", "ai", "education"]
tags: ["specific", "technical", "tags"]
author: "Victor Saly"
---
```

**To Dev.to:**
```yaml
---
title: "Article Title"
published: false
description: "Brief, engaging description for dev.to feed (under 160 chars)"
tags: ai, education, gamedev, softwaredevelopment
cover_image: https://docs.worldleadersgame.co.uk/assets/linkedin-images/article-cover.png
canonical_url: https://docs.worldleadersgame.co.uk/posts/article-slug/
series: "AI-First Educational Game Development"
---
```

### Step 3: Image URL Conversion

**Pattern Replacement:**
```bash
# Replace relative image URLs
sed -i 's|/assets/images/|https://docs.worldleadersgame.co.uk/assets/images/|g'
sed -i 's|![Image](images/|![Image](https://docs.worldleadersgame.co.uk/assets/images/|g'
```

### Step 4: Diagram Conversion

**Mermaid to ASCII Example:**

**Before (Mermaid):**
```mermaid
graph TD
    A[Start] --> B[Process]
    B --> C[End]
```

**After (ASCII):**
```
┌─────────┐
│  Start  │
└────┬────┘
     │
     ▼
┌─────────┐
│ Process │
└────┬────┘
     │
     ▼
┌─────────┐
│   End   │
└─────────┘
```

### Step 5: Content Optimization

#### Add TL;DR Section
```markdown
> **TL;DR**: Brief, engaging summary with key value proposition and main takeaways that encourage continued reading.
```

#### Add Discussion Questions
```markdown
## 💭 Discussion Questions

I'm curious about your experience with [topic]:

1. **What's your experience with [specific aspect]?**
2. **Have you tried [specific technique/approach]?**
3. **What barriers have you encountered when [doing something]?**
4. **How do you balance [competing concerns]?**

Share your thoughts and experiences in the comments below! 👇
```

#### Add Resource Links
```markdown
## 🔗 Want to Learn More?

This post is part of our **[series name]**.

**📚 Follow the complete journey**: [docs.worldleadersgame.co.uk](https://docs.worldleadersgame.co.uk/)
**💻 Browse the code**: [GitHub repository](https://github.com/victorsaly/WorldLeadersGame)
**🤖 Study the methodology**: [Complete instruction system](https://github.com/victorsaly/WorldLeadersGame/tree/main/.github/copilot-instructions)

**Next week**: [Preview of upcoming content]

---

_Follow me [@victorsaly](https://dev.to/victorsaly) for more insights on [topic area]._
```

## 🎨 Dev.to Formatting Best Practices

### Headers and Structure
```markdown
# Main Title (H1) - Only one per article

## Section Headers (H2) - Primary sections

### Subsection Headers (H3) - Within sections

#### Detail Headers (H4) - Rarely needed
```

### Code Formatting
```markdown
`Inline code` for short snippets

```language
Multi-line code blocks
with proper syntax highlighting
```

> Quote blocks for important insights
```

### Visual Elements
```markdown
**Bold text** for emphasis
*Italic text* for subtle emphasis
~~Strikethrough~~ for corrections

- Bullet points for lists
1. Numbered lists for sequences

🎯 Emojis for visual appeal and categorization
```

### Links and References
```markdown
[Link text](https://full-url.com) - Always use full URLs
[Internal content](https://docs.worldleadersgame.co.uk/path) - Convert internal links
```

## 🔍 Quality Assurance

### Content Review
- [ ] **Readability**: Clear, engaging writing style
- [ ] **Flow**: Logical progression of ideas
- [ ] **Value**: Actionable insights and takeaways
- [ ] **Examples**: Concrete examples and code snippets
- [ ] **Accuracy**: Technical information verified

### Technical Validation
- [ ] **Markdown Syntax**: Proper formatting throughout
- [ ] **Link Testing**: All links functional and correct
- [ ] **Image Loading**: All images accessible and optimized
- [ ] **Code Testing**: All code examples functional
- [ ] **Mobile Friendly**: Content readable on mobile devices

### SEO and Discoverability
- [ ] **Title Optimization**: Clear, searchable title
- [ ] **Tag Relevance**: Tags match content and dev.to conventions
- [ ] **Description**: Compelling meta description
- [ ] **Canonical URL**: Proper attribution to original source
- [ ] **Series Attribution**: Proper linking to related content

## 📊 Dev.to Tag Guidelines

### Recommended Tags
- **ai** - AI and machine learning content
- **education** - Educational technology and learning
- **gamedev** - Game development content
- **softwaredevelopment** - General software development
- **github** - GitHub and collaboration tools
- **productivity** - Development productivity and tools
- **tutorial** - Step-by-step guides
- **webdev** - Web development content

### Tag Best Practices
- Maximum 4 tags per article
- Use popular, established tags for discoverability
- Combine broad and specific tags
- Check tag popularity before using

## 🚀 Publishing Workflow

### 1. Final Review
```bash
# Run validation script
./devto/validate-devto-article.sh devto/articles/article-name.md
```

### 2. Test Locally
```bash
# Preview in Jekyll (if needed)
bundle exec jekyll serve --baseurl "" --drafts
```

### 3. Pre-Publish
```yaml
# Set in frontmatter
published: false  # For draft mode
```

### 4. Publish
```yaml
# Set in frontmatter
published: true   # When ready to go live
```

### 5. Post-Publish
- [ ] **Share on Social**: Twitter, LinkedIn, etc.
- [ ] **Monitor Engagement**: Respond to comments
- [ ] **Track Analytics**: Views, engagement metrics
- [ ] **Update Series**: Link to new content in series

## 🛠️ Automation Tools

### convert-to-devto.sh
Automated conversion script handling:
- Frontmatter transformation
- Image URL conversion
- Diagram format conversion
- Content optimization

### validate-devto-article.sh
Quality assurance script checking:
- Frontmatter compliance
- Image accessibility
- Link validation
- Formatting compliance

## 📚 Examples and Templates

### Sample Frontmatter
```yaml
---
title: "AI-First Development: Achieving 95% Autonomous Code Generation"
published: false
description: "Two weeks into our AI-first development experiment, we've discovered the specific methodologies that enable true autonomous software creation."
tags: ai, education, gamedev, softwaredevelopment
cover_image: https://docs.worldleadersgame.co.uk/assets/linkedin-images/ai-first-development.png
canonical_url: https://docs.worldleadersgame.co.uk/posts/ai-first-development-methodology/
series: "AI-First Educational Game Development"
---
```

### Sample Diagram Conversion
```
# Progress Bar Visualization
Architecture Design      ████████████████████ 95% ✅
Code Generation         ████████████████████ 92% ✅
Documentation          ████████████████████ 100% 🎯
Educational Content     ████████████████░░░░ 85% ⭐
```

### Sample Discussion Section
```markdown
## 💭 Discussion Questions

I'm curious about your experience with AI-assisted development:

1. **What's the highest level of AI autonomy you've achieved in your projects?**
2. **Have you tried giving AI more strategic decision-making power?**
3. **What barriers have you encountered when increasing AI autonomy?**
4. **For educational/child-focused projects, how do you balance AI efficiency with safety requirements?**

Share your thoughts and experiences in the comments below! 👇
```

## 🔄 Continuous Improvement

### Performance Tracking
- Monitor article performance metrics
- Analyze reader engagement patterns
- Identify successful content formats
- Optimize based on community feedback

### Process Refinement
- Update templates based on dev.to changes
- Improve automation scripts
- Enhance quality assurance procedures
- Streamline conversion workflow

---

**Remember**: The goal is to share valuable educational content while building a community around AI-assisted educational development. Quality and authenticity are more important than speed.
