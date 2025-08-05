---
layout: page
title: "GitHub Pages CloudFlare Setup Guide"
date: 2025-08-05
category: "technical-guide"
tags: ["github-pages", "cloudflare", "dns", "documentation"]
author: "AI-Generated with Human Oversight"
---

# 📚 GitHub Pages + CloudFlare Setup for docs.worldleadersgame.co.uk

## 🎯 Overview

Your project structure now uses:

- **Main Game**: `worldleadersgame.co.uk` → Azure Web App
- **API**: `api.worldleadersgame.co.uk` → Azure API App
- **Documentation**: `docs.worldleadersgame.co.uk` → GitHub Pages

## ✅ GitHub Pages Configuration (COMPLETED)

### 1. CNAME File Updated

- ✅ **File**: `/docs/CNAME`
- ✅ **Content**: `docs.worldleadersgame.co.uk`
- ✅ **Purpose**: Tells GitHub Pages which custom domain to use

### 2. GitHub Repository Settings

Go to your repository settings and verify:

1. **Pages Section**:

   - Source: Deploy from a branch
   - Branch: `main`
   - Folder: `/docs`
   - Custom domain: `docs.worldleadersgame.co.uk`

2. **Domain Verification**:
   - GitHub will automatically verify the domain
   - SSL certificate will be provisioned

## 🌐 CloudFlare DNS Configuration

### Complete DNS Records (4 Records Total)

Add these records in **CloudFlare Dashboard** → **worldleadersgame.co.uk** → **DNS**:

```
Type: CNAME | Name: @    | Target: worldleaders-web-prod.azurewebsites.net | Proxy: ☁️ ON
Type: CNAME | Name: api  | Target: worldleaders-api-prod.azurewebsites.net | Proxy: ☁️ ON
Type: CNAME | Name: www  | Target: worldleaders-web-prod.azurewebsites.net | Proxy: ☁️ ON
Type: CNAME | Name: docs | Target: victorsaly.github.io                    | Proxy: ☁️ ON
```

### CloudFlare Page Rules for Documentation

Create a page rule for better GitHub Pages performance:

1. Go to **Rules** → **Page Rules**
2. Create rule: `docs.worldleadersgame.co.uk/*`
   - **SSL**: Full (strict)
   - **Cache Level**: Standard
   - **Browser Cache TTL**: 4 hours
   - **Edge Cache TTL**: 2 hours

## 🔧 Verification Steps

### 1. GitHub Pages Status

Check your repository → Settings → Pages:

- Should show: "Your site is published at https://docs.worldleadersgame.co.uk"
- SSL certificate status should be "Certificate has been generated"

### 2. DNS Propagation Check

```bash
# Check all subdomains
dig worldleadersgame.co.uk
dig api.worldleadersgame.co.uk
dig docs.worldleadersgame.co.uk
```

### 3. HTTPS Testing

```bash
# Test all endpoints
curl -I https://worldleadersgame.co.uk
curl -I https://api.worldleadersgame.co.uk/health
curl -I https://docs.worldleadersgame.co.uk
```

## 🚀 Complete Site Architecture

### Final URL Structure

```
📁 worldleadersgame.co.uk
├── 🎮 /                     → Azure Web App (Main Game)
├── 🔧 /api/                 → Azure API App (Game Backend)
└── 📚 /docs/                → GitHub Pages (Documentation)
```

### Actual URLs After Setup

- **🌐 Main Game**: https://worldleadersgame.co.uk
- **🔧 API Endpoints**: https://api.worldleadersgame.co.uk
- **📚 Documentation**: https://docs.worldleadersgame.co.uk
- **📊 API Health**: https://api.worldleadersgame.co.uk/health
- **📋 API Docs**: https://api.worldleadersgame.co.uk/swagger

## 🛠️ Troubleshooting

### GitHub Pages Not Loading

1. Check CNAME file contains `docs.worldleadersgame.co.uk`
2. Verify repository settings → Pages → Custom domain
3. Wait 10-15 minutes for GitHub to process changes
4. Check for any GitHub Pages build errors

### CloudFlare SSL Issues

1. Ensure GitHub Pages SSL is enabled
2. Set CloudFlare to "Full (strict)" SSL mode
3. Enable "Always Use HTTPS" in CloudFlare
4. Wait 15-30 minutes for certificate propagation

### DNS Not Resolving

1. Verify all 4 CNAME records are added
2. Ensure Orange Cloud (Proxy) is enabled
3. Clear DNS cache: `sudo dscacheutil -flushcache` (macOS)
4. Test with external DNS: `dig @8.8.8.8 docs.worldleadersgame.co.uk`

## 📊 Performance Optimization

### CloudFlare Settings for Documentation

- **Caching**: Standard level for static content
- **Minification**: HTML, CSS, JS enabled
- **Compression**: Brotli enabled
- **Security**: Medium level with WAF

### GitHub Pages Optimization

- Jekyll caching enabled automatically
- Static asset optimization
- CDN delivery through CloudFlare

## ✅ Setup Checklist

- [ ] GitHub CNAME file updated to `docs.worldleadersgame.co.uk`
- [ ] GitHub repository Pages settings configured
- [ ] CloudFlare DNS record added: `docs` → `victorsaly.github.io`
- [ ] CloudFlare proxy enabled (Orange Cloud ON)
- [ ] SSL/TLS set to "Full (strict)"
- [ ] DNS propagation verified
- [ ] HTTPS certificates working
- [ ] All sites accessible and loading correctly

---

**⏰ Expected Timeline**: GitHub Pages (5-10 min) → DNS propagation (15-30 min) → SSL activation (15-30 min)
