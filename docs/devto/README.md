# 📝 Dev.to Publishing System

This folder contains the automated system for converting Jekyll blog posts to dev.to format with proper image hosting and formatting.

## 🎯 Purpose

Transform Jekyll blog posts into dev.to-ready articles with:

- ✅ Proper frontmatter formatting for dev.to
- ✅ Image URLs pointing to `docs.worldleadersgame.co.uk`
- ✅ ASCII/Unicode diagrams compatible with dev.to
- ✅ Community-optimized content structure
- ✅ Deployment readiness validation

## 📁 Structure

```
devto/
├── README.md                           # This documentation
├── convert-to-devto.sh                # Main conversion script
├── publishing-guide.md                # Complete publishing instructions
├── articles/                          # Converted dev.to articles
│   └── 2025-08-02-ai-first-development-methodology.md
└── templates/                         # Dev.to templates and patterns
    ├── frontmatter-template.md
    └── diagram-patterns.md
```

## 🚀 Quick Start

### Convert a Jekyll Post to Dev.to Format

```bash
# Run from docs/ directory
./devto/convert-to-devto.sh _posts/2025-08-02-ai-first-development-methodology-new-paradigm.md
```

### Validate Dev.to Article

```bash
# Check deployment readiness
./devto/validate-devto-article.sh devto/articles/2025-08-02-ai-first-development-methodology.md
```

## 📋 Dev.to Requirements Checklist

- [ ] **Frontmatter**: Proper tags, description, canonical_url
- [ ] **Images**: All URLs point to `docs.worldleadersgame.co.uk`
- [ ] **Diagrams**: ASCII/Unicode format (no Mermaid)
- [ ] **Links**: Internal links converted to external URLs
- [ ] **Formatting**: Dev.to markdown compatibility
- [ ] **Engagement**: Discussion questions and calls-to-action
- [ ] **Series**: Proper series attribution if applicable

## 🔧 Technical Details

### Image URL Pattern

```
Original:  /assets/images/filename.png
Dev.to:    https://docs.worldleadersgame.co.uk/assets/images/filename.png
```

### Frontmatter Format

```yaml
---
title: "Article Title"
published: false # Set to true when ready to publish
description: "Brief description for dev.to feed"
tags: ai, education, gamedev, softwaredevelopment
cover_image: https://docs.worldleadersgame.co.uk/assets/images/cover.png
canonical_url: https://docs.worldleadersgame.co.uk/post/article-slug/
series: "Series Name" # Optional
---
```

### Diagram Conversion

- **Mermaid** → **ASCII/Unicode** diagrams
- **Complex flows** → **Step-by-step text**
- **Charts** → **Progress bars with emojis**
- **Graphs** → **Visual text representations**

## 📚 See Also

- [publishing-guide.md](./publishing-guide.md) - Complete publishing workflow
- [Jekyll to Dev.to Conversion Examples](./templates/) - Templates and patterns
- [Image Hosting Setup](../assets/) - Image management for dev.to
