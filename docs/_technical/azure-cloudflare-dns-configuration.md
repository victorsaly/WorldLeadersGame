---
layout: page
title: "Azure CloudFlare DNS Configuration Guide"
date: 2025-08-05
category: "technical-guide"
tags: ["azure", "cloudflare", "dns", "ssl", "production"]
author: "AI-Generated with Human Oversight"
---

# 🚀 Quick CloudFlare Setup Reference - worldleadersgame.co.uk

## 📋 DNS Records (Copy-Paste Ready)

**Go to CloudFlare Dashboard → Your Domain → DNS → Records**

### Main CNAME Records:

```
Type: CNAME | Name: @    | Target: worldleaders-web-prod.azurewebsites.net | Proxy: ON
Type: CNAME | Name: api  | Target: worldleaders-api-prod.azurewebsites.net | Proxy: ON
Type: CNAME | Name: www  | Target: worldleaders-web-prod.azurewebsites.net | Proxy: ON
Type: CNAME | Name: docs | Target: victorsaly.github.io                    | Proxy: ON
```

### Azure Domain Verification TXT Records:

```
Type: TXT | Name: asuid     | Content: D618B57BA7A1EE5C3F47B6CB7388E279FC0FCEE8D2FBD941C677BDE2AA7D6141 | Proxy: OFF
Type: TXT | Name: asuid.api | Content: D618B57BA7A1EE5C3F47B6CB7388E279FC0FCEE8D2FBD941C677BDE2AA7D6141 | Proxy: OFF
```

## ⚡ Essential CloudFlare Settings

**SSL/TLS Tab:**

- Encryption Mode: `Full (strict)`
- Always Use HTTPS: `ON`
- Minimum TLS Version: `1.2`

**Security Tab:**

- Security Level: `Medium`
- WAF: `ON`
- Browser Integrity Check: `ON`

**Speed Tab:**

- Auto Minify: `HTML, CSS, JS`
- Brotli: `ON`
- Early Hints: `ON`

## 🔧 Azure Commands (After DNS Propagates)

```bash
# Add domains to Azure App Services
az webapp config hostname add --resource-group worldleaders-prod-rg --webapp-name worldleaders-web-prod --hostname worldleadersgame.co.uk
az webapp config hostname add --resource-group worldleaders-prod-rg --webapp-name worldleaders-api-prod --hostname api.worldleadersgame.co.uk

# Enable managed SSL certificates (Azure will automatically provision certificates)
az webapp config ssl create --resource-group worldleaders-prod-rg --name worldleaders-web-prod --hostname worldleadersgame.co.uk
az webapp config ssl create --resource-group worldleaders-prod-rg --name worldleaders-api-prod --hostname api.worldleadersgame.co.uk
```

## 🧪 Quick Tests

```bash
# Check DNS
dig worldleadersgame.co.uk
dig api.worldleadersgame.co.uk
dig docs.worldleadersgame.co.uk

# Test HTTPS
curl -I https://worldleadersgame.co.uk
curl -I https://api.worldleadersgame.co.uk/health
curl -I https://docs.worldleadersgame.co.uk
```

## 📊 Your Live URLs (After Setup)

- **🌐 Main Game**: https://worldleadersgame.co.uk
- **🔧 API**: https://api.worldleadersgame.co.uk
- **� Documentation**: https://docs.worldleadersgame.co.uk
- **�📊 Health Check**: https://api.worldleadersgame.co.uk/health
- **📚 API Docs**: https://api.worldleadersgame.co.uk/swagger

---

**⏰ Timeline**: DNS (5-30 min) → Azure config (5 min) → SSL (15-30 min)
