---
layout: page
title: "CloudFlare DNS Comprehensive Setup"
date: 2025-08-05
category: "technical-guide"
tags: ["cloudflare", "dns", "networking", "production"]
author: "AI-Generated with Human Oversight"
---

# 🌐 CloudFlare DNS Setup for worldleadersgame.co.uk

**Complete guide to configure CloudFlare DNS for your World Leaders Game deployment**

## 📋 Overview

Your Azure deployment creates these services:

- **Web App**: `worldleaders-web-prod.azurewebsites.net`
- **API App**: `worldleaders-api-prod.azurewebsites.net`

We'll configure CloudFlare to route:

- `worldleadersgame.co.uk` → Web App (main game)
- `api.worldleadersgame.co.uk` → API App (game backend)
- Optional: `docs.worldleadersgame.co.uk` → GitHub Pages (documentation)

## 🚀 Step-by-Step CloudFlare Setup

### Step 1: Access CloudFlare Dashboard

1. Go to [CloudFlare Dashboard](https://dash.cloudflare.com/)
2. Log in to your account
3. Select your **worldleadersgame.co.uk** domain
4. Navigate to **DNS** > **Records**

### Step 2: Configure DNS Records

#### Option A: CNAME Records (Recommended)

Add these records in CloudFlare DNS:

```
Type: CNAME
Name: @
Target: worldleaders-web-prod.azurewebsites.net
Proxy status: ☁️ Proxied (Orange Cloud)
TTL: Auto
```

```
Type: CNAME
Name: api
Target: worldleaders-api-prod.azurewebsites.net
Proxy status: ☁️ Proxied (Orange Cloud)
TTL: Auto
```

```
Type: CNAME
Name: www
Target: worldleaders-web-prod.azurewebsites.net
Proxy status: ☁️ Proxied (Orange Cloud)
TTL: Auto
```

#### Option B: A Records (If CNAME doesn't work for root domain)

If CloudFlare doesn't allow CNAME for root domain, use A records:

**First, get Azure IP addresses:**

```bash
az webapp show --resource-group worldleaders-prod-rg --name worldleaders-web-prod --query "outboundIpAddresses" -o tsv
```

Then add A records for each IP:

```
Type: A
Name: @
IPv4 address: [IP from Azure command]
Proxy status: ☁️ Proxied (Orange Cloud)
TTL: Auto
```

### Step 3: Optional - Documentation Subdomain

If you want to host documentation on a subdomain:

```
Type: CNAME
Name: docs
Target: victorsaly.github.io
Proxy status: ☁️ Proxied (Orange Cloud)
TTL: Auto
```

## ⚡ CloudFlare Configuration Recommendations

### SSL/TLS Settings

1. Go to **SSL/TLS** > **Overview**
2. Set encryption mode to: **Full (strict)**
3. Enable **Always Use HTTPS**

### Page Rules for API Routing

1. Go to **Rules** > **Page Rules**
2. Create rule for `api.worldleadersgame.co.uk/*`
   - **SSL**: Full (strict)
   - **Cache Level**: Bypass
   - **Browser Cache TTL**: Respect Existing Headers

### Security Settings

1. **Security** > **WAF**

   - Enable **Web Application Firewall**
   - Set to **Medium** security level

2. **Security** > **DDoS**

   - Ensure DDoS protection is **enabled**

3. **Speed** > **Optimization**
   - Enable **Auto Minify** (HTML, CSS, JS)
   - Enable **Brotli** compression

## 🧪 Testing Your Configuration

### 1. DNS Propagation Check

```bash
# Check main domain
dig worldleadersgame.co.uk

# Check API subdomain
dig api.worldleadersgame.co.uk

# Check from multiple locations
nslookup worldleadersgame.co.uk 8.8.8.8
nslookup api.worldleadersgame.co.uk 8.8.8.8
```

### 2. Online Tools

Use these tools to verify DNS propagation:

- [DNS Checker](https://dnschecker.org/)
- [WhatsMyDNS](https://whatsmydns.net/)
- [DNS Propagation Checker](https://www.site24x7.com/tools/dns-lookup.html)

### 3. SSL Certificate Verification

```bash
# Check SSL certificate
openssl s_client -servername worldleadersgame.co.uk -connect worldleadersgame.co.uk:443 -showcerts

# Check API SSL
openssl s_client -servername api.worldleadersgame.co.uk -connect api.worldleadersgame.co.uk:443 -showcerts
```

## 🔧 Azure Custom Domain Configuration

After DNS is configured and propagated (5-30 minutes), add the domains to Azure:

### 1. Add Main Domain

```bash
az webapp config hostname add \
    --resource-group worldleaders-prod-rg \
    --webapp-name worldleaders-web-prod \
    --hostname worldleadersgame.co.uk
```

### 2. Add API Subdomain

```bash
az webapp config hostname add \
    --resource-group worldleaders-prod-rg \
    --webapp-name worldleaders-api-prod \
    --hostname api.worldleadersgame.co.uk
```

### 3. Enable SSL Certificates

```bash
# SSL for main domain
az webapp config ssl bind \
    --resource-group worldleaders-prod-rg \
    --name worldleaders-web-prod \
    --certificate-type Managed \
    --hostname worldleadersgame.co.uk

# SSL for API domain
az webapp config ssl bind \
    --resource-group worldleaders-prod-rg \
    --name worldleaders-api-prod \
    --certificate-type Managed \
    --hostname api.worldleadersgame.co.uk
```

## 🎯 CloudFlare Analytics & Monitoring

### 1. Analytics Setup

1. Go to **Analytics & Logs** > **Web Analytics**
2. Enable **Web Analytics** for your domain
3. Configure **Custom Events** for game interactions

### 2. Performance Monitoring

1. **Speed** > **Optimization**
   - Monitor **Core Web Vitals**
   - Track **Page Load Times**

### 3. Security Monitoring

1. **Security** > **Events**
   - Monitor **Firewall Events**
   - Track **DDoS Attacks**
   - Review **Bot Traffic**

## 🚨 Troubleshooting Common Issues

### DNS Not Propagating

```bash
# Clear local DNS cache (macOS)
sudo dscacheutil -flushcache
sudo killall -HUP mDNSResponder

# Check specific nameservers
dig @ns1.cloudflare.com worldleadersgame.co.uk
dig @ns2.cloudflare.com worldleadersgame.co.uk
```

### SSL Certificate Issues

1. Verify CloudFlare SSL is set to **Full (strict)**
2. Check Azure App Service custom domain configuration
3. Wait 15-30 minutes for certificate provisioning

### 522 Connection Timeout

1. Check Azure App Service is running
2. Verify security group rules
3. Check CloudFlare origin certificate

## 📊 Complete DNS Configuration Summary

| Record Type | Name | Target                                  | Proxy | TTL  |
| ----------- | ---- | --------------------------------------- | ----- | ---- |
| CNAME       | @    | worldleaders-web-prod.azurewebsites.net | ☁️    | Auto |
| CNAME       | api  | worldleaders-api-prod.azurewebsites.net | ☁️    | Auto |
| CNAME       | www  | worldleaders-web-prod.azurewebsites.net | ☁️    | Auto |
| CNAME       | docs | victorsaly.github.io                    | ☁️    | Auto |

## ✅ Verification Checklist

- [ ] DNS records added to CloudFlare
- [ ] Orange cloud (proxy) enabled for all records
- [ ] SSL/TLS set to "Full (strict)"
- [ ] DNS propagation confirmed (dig commands)
- [ ] Custom domains added to Azure App Services
- [ ] SSL certificates enabled in Azure
- [ ] Main site accessible: https://worldleadersgame.co.uk
- [ ] API accessible: https://api.worldleadersgame.co.uk
- [ ] HTTPS redirects working
- [ ] CloudFlare analytics configured

## 🎮 Final Testing URLs

After configuration:

- **Main Game**: https://worldleadersgame.co.uk
- **API Health**: https://api.worldleadersgame.co.uk/health
- **API Swagger**: https://api.worldleadersgame.co.uk/swagger
- **Documentation**: https://docs.worldleadersgame.co.uk (if configured)

---

**⏰ Expected Timeline**: 15-60 minutes for full propagation and SSL activation
