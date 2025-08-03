---
layout: page
title: "Local Testing Guide"
date: 2025-08-15
category: "development-guide"
tags: ["jekyll", "testing", "local-development"]
---

# 🧪 Local Testing Guide

**Purpose**: Test Jekyll documentation locally  
**Target**: Contributors to World Leaders Game documentation

---

## 🚀 Three Simple Options

### Option 1: One-Click Test

```bash
cd docs
./simple-test.sh
```

Auto-detects your setup and starts Jekyll server.

### Option 2: Docker (Zero Setup)

```bash
cd docs
./test-docker.sh
```

Requires Docker Desktop only.

### Option 3: Manual Jekyll

```bash
cd docs
bundle config set --local path 'vendor/bundle'
bundle install
bundle exec jekyll serve
```

**All methods open**: http://localhost:4000/ConquerTheWorldGame

---

## � What Each Script Does

### simple-test.sh

- ✅ Detects available Ruby/Jekyll
- ✅ Tries system Jekyll first
- ✅ Falls back to Bundler if needed
- ✅ Provides helpful error messages

### test-docker.sh

- ✅ Uses Jekyll 3.9.3 (exact GitHub Pages version)
- ✅ No Ruby installation required
- ✅ Consistent across all systems
- ✅ Zero configuration needed

---

## 🐛 Quick Fixes

**Problem**: "Command not found"  
**Fix**: Make scripts executable

```bash
chmod +x *.sh
```

**Problem**: "Port already in use"  
**Fix**: Kill existing process

```bash
lsof -ti:4000 | xargs kill
```

**Problem**: Ruby permission issues  
**Fix**: Use Docker method instead

```bash
./test-docker.sh
```

---

**That's it!** Pick one method and test your documentation changes before committing.

```bash
cd docs
./test-docker.sh
```

Opens: http://localhost:4000/ConquerTheWorldGame

### Method 2: Ruby + Jekyll (Traditional)

**Prerequisites**: Ruby 2.7+ and Bundler

```bash
cd docs
bundle config set --local path 'vendor/bundle'
bundle install
bundle exec jekyll serve
```

Opens: http://localhost:4000/ConquerTheWorldGame

### Method 3: GitHub Codespaces (Cloud)

1. GitHub repo → Code → Codespaces → Create
2. Terminal: `cd docs && bundle install && bundle exec jekyll serve --host 0.0.0.0`
3. Open forwarded port 4000

---

## 🐳 Docker Details

### Why Docker?

- ✅ No Ruby installation required
- ✅ Exact GitHub Pages environment (Jekyll 3.9.3)
- ✅ Consistent across all systems
- ✅ No permission issues

### Docker Commands

```bash
# Start development server
./test-docker.sh

# Manual Docker run
docker run --rm \
  --volume="$PWD:/srv/jekyll:Z" \
  --publish 4000:4000 \
  jekyll/jekyll:3.9 \
  jekyll serve --watch --force_polling --host 0.0.0.0

# Build only (no server)
docker run --rm \
  --volume="$PWD:/srv/jekyll:Z" \
  jekyll/jekyll:3.9 \
  jekyll build

# Health check
docker run --rm \
  --volume="$PWD:/srv/jekyll:Z" \
  jekyll/jekyll:3.9 \
  jekyll doctor
```

---

## 📝 Content Testing Checklist

### Before Every Commit

- [ ] **All pages load**: Homepage, blog, journey, technical docs
- [ ] **Navigation works**: Menu links functional
- [ ] **New content renders**: Markdown → HTML correctly
- [ ] **Images display**: All assets load properly
- [ ] **Mobile responsive**: Test on different screen sizes

### Quick Test Commands

```bash
# Start server and auto-refresh on changes
./test-docker.sh

# Test specific URLs
open http://localhost:4000/ConquerTheWorldGame/
open http://localhost:4000/ConquerTheWorldGame/blog/
open http://localhost:4000/ConquerTheWorldGame/journey/
open http://localhost:4000/ConquerTheWorldGame/technical/
```

---

## 🔧 Troubleshooting

### Docker Issues

**Problem**: "Docker command not found"
**Solution**: Install Docker Desktop from docker.com

**Problem**: "Permission denied"
**Solution**: Make script executable: `chmod +x test-docker.sh`

**Problem**: "Port 4000 already in use"
**Solution**:

```bash
# Kill existing process
lsof -ti:4000 | xargs kill

# Or use different port
docker run ... --publish 4001:4000 ...
```

### Ruby/Bundle Issues

**Problem**: "Permission denied installing gems"
**Solution**: Use local bundle path:

```bash
bundle config set --local path 'vendor/bundle'
bundle install
```

**Problem**: "Jekyll version conflicts"
**Solution**: Our Gemfile uses GitHub Pages exact versions:

```bash
rm -f Gemfile.lock
bundle install
```

### Content Issues

**Problem**: "Page not found"
**Solution**: Check frontmatter and file naming:

```yaml
---
layout: page
title: "Page Title"
---
```

**Problem**: "Markdown not rendering"
**Solution**: Verify file extension is `.md` and frontmatter is correct

---

## 🎯 Development Workflow

### Creating New Content

1. **Create file** with proper frontmatter
2. **Start Docker server**: `./test-docker.sh`
3. **Edit content** (auto-refreshes in browser)
4. **Test thoroughly**
5. **Commit and push**

### File Structure

```
docs/
├── _posts/          # Blog posts (YYYY-MM-DD-title.md)
├── _journey/        # Weekly development logs
├── _technical/      # Implementation guides
├── _milestones/     # Project milestones
├── index.md         # Homepage
├── blog.md          # Blog index
├── journey.md       # Journey index
└── technical-docs.md # Technical index
```

### Frontmatter Templates

**Blog Post**:

```yaml
---
layout: post
title: "Post Title"
date: 2025-08-15
categories: ["development", "ai"]
tags: ["specific", "tags"]
author: "Victor Saly"
---
```

**Journey Entry**:

```yaml
---
layout: page
title: "Week X: Description"
date: 2025-08-15
week: X
status: "completed"
ai_autonomy: "90%"
---
```

**Technical Guide**:

```yaml
---
layout: page
title: "Technical Guide Title"
date: 2025-08-15
category: "technical-guide"
tags: ["technology", "framework"]
author: "AI-Generated with Human Oversight"
---
```

---

## 🎉 Success Metrics

### What Good Looks Like

- ✅ **Fast Loading**: Pages load in < 2 seconds
- ✅ **Mobile Friendly**: Responsive on all devices
- ✅ **Navigation Works**: All links functional
- ✅ **Content Renders**: Markdown displays correctly
- ✅ **Professional Look**: Clean, educational design

### Common Success URLs

- **Homepage**: http://localhost:4000/ConquerTheWorldGame/
- **Latest Blog**: http://localhost:4000/ConquerTheWorldGame/blog/2025/08/03/week-2-foundation-complete/
- **Current Journey**: http://localhost:4000/ConquerTheWorldGame/journey/week-03-game-mechanics/
- **Tech Guide**: http://localhost:4000/ConquerTheWorldGame/technical/ai-prompt-engineering/

---

_This guide ensures reliable local testing of the World Leaders Game documentation using the simplest, most compatible methods available._
