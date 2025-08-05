#!/bin/bash

# 🔍 CloudFlare DNS Configuration Checker
# Verify your worldleadersgame.co.uk DNS setup

echo "🔍 Checking CloudFlare DNS Configuration"
echo "========================================"
echo ""

DOMAIN="worldleadersgame.co.uk"

# Function to check DNS record
check_dns_record() {
    local subdomain=$1
    local full_domain="${subdomain}.${DOMAIN}"
    if [ "$subdomain" = "@" ]; then
        full_domain="$DOMAIN"
    fi
    
    echo "🔍 Checking $full_domain..."
    
    # Check A record
    A_RECORD=$(dig +short $full_domain A @8.8.8.8)
    if [ ! -z "$A_RECORD" ]; then
        echo "   ✅ A Record: $A_RECORD"
    fi
    
    # Check CNAME record
    CNAME_RECORD=$(dig +short $full_domain CNAME @8.8.8.8)
    if [ ! -z "$CNAME_RECORD" ]; then
        echo "   ✅ CNAME Record: $CNAME_RECORD"
    fi
    
    # Check if it's proxied through CloudFlare
    CF_CHECK=$(curl -s -I "http://$full_domain" 2>/dev/null | grep -i "cf-ray\|cloudflare")
    if [ ! -z "$CF_CHECK" ]; then
        echo "   ☁️  CloudFlare Proxy: Active"
    else
        echo "   ⚠️  CloudFlare Proxy: Not detected"
    fi
    
    # Check HTTPS
    HTTPS_CHECK=$(curl -s -I "https://$full_domain" 2>/dev/null | head -1)
    if [[ "$HTTPS_CHECK" == *"200"* ]] || [[ "$HTTPS_CHECK" == *"301"* ]] || [[ "$HTTPS_CHECK" == *"302"* ]]; then
        echo "   🔒 HTTPS: Working"
    else
        echo "   ❌ HTTPS: Not working"
    fi
    
    echo ""
}

# Check main domain
check_dns_record "@"

# Check API subdomain
check_dns_record "api"

# Check WWW subdomain
check_dns_record "www"

# Advanced CloudFlare checks
echo "🌐 CloudFlare Configuration Analysis"
echo "===================================="
echo ""

# Check SSL/TLS settings
echo "🔒 SSL/TLS Check:"
SSL_INFO=$(curl -s -I "https://$DOMAIN" 2>/dev/null | grep -i "strict-transport-security\|cf-cache-status\|cf-ray")
if [ ! -z "$SSL_INFO" ]; then
    echo "$SSL_INFO" | while read line; do
        echo "   $line"
    done
else
    echo "   ⚠️  SSL headers not detected"
fi
echo ""

# Check security headers
echo "🛡️  Security Headers:"
SECURITY_HEADERS=$(curl -s -I "https://$DOMAIN" 2>/dev/null | grep -i "x-frame-options\|x-content-type-options\|x-xss-protection")
if [ ! -z "$SECURITY_HEADERS" ]; then
    echo "$SECURITY_HEADERS" | while read line; do
        echo "   $line"
    done
else
    echo "   ⚠️  Security headers not detected"
fi
echo ""

# Performance check
echo "⚡ Performance Check:"
RESPONSE_TIME=$(curl -o /dev/null -s -w "%{time_total}" "https://$DOMAIN" 2>/dev/null)
if [ ! -z "$RESPONSE_TIME" ]; then
    echo "   Response time: ${RESPONSE_TIME}s"
    if (( $(echo "$RESPONSE_TIME < 2.0" | bc -l) )); then
        echo "   ✅ Good performance"
    else
        echo "   ⚠️  Consider optimization"
    fi
else
    echo "   ❌ Could not measure response time"
fi
echo ""

# API health check
echo "🔧 API Health Check:"
API_HEALTH=$(curl -s "https://api.$DOMAIN/health" 2>/dev/null)
if [ ! -z "$API_HEALTH" ]; then
    echo "   ✅ API responding: $API_HEALTH"
else
    echo "   ❌ API not responding"
fi
echo ""

echo "📋 CloudFlare Recommendations:"
echo "=============================="
echo ""
echo "1. SSL/TLS Settings:"
echo "   • Set to 'Full (strict)' mode"
echo "   • Enable 'Always Use HTTPS'"
echo "   • Enable 'HTTP Strict Transport Security (HSTS)'"
echo ""
echo "2. Security Settings:"
echo "   • Enable Web Application Firewall (WAF)"
echo "   • Set Security Level to 'Medium'"
echo "   • Enable 'Browser Integrity Check'"
echo ""
echo "3. Performance Settings:"
echo "   • Enable 'Auto Minify' for HTML, CSS, JS"
echo "   • Enable 'Brotli' compression"
echo "   • Enable 'Early Hints'"
echo ""
echo "4. Page Rules (Optional):"
echo "   • api.$DOMAIN/* - SSL: Full, Cache Level: Bypass"
echo "   • $DOMAIN/* - SSL: Full, Cache Level: Standard"
echo ""

# Summary
echo "🎯 Configuration Summary:"
echo "========================="

# Count working services
WORKING_COUNT=0

if dig +short $DOMAIN @8.8.8.8 > /dev/null 2>&1; then
    ((WORKING_COUNT++))
fi

if dig +short "api.$DOMAIN" @8.8.8.8 > /dev/null 2>&1; then
    ((WORKING_COUNT++))
fi

if curl -s -I "https://$DOMAIN" > /dev/null 2>&1; then
    ((WORKING_COUNT++))
fi

if curl -s -I "https://api.$DOMAIN" > /dev/null 2>&1; then
    ((WORKING_COUNT++))
fi

echo "Working services: $WORKING_COUNT/4"

if [ $WORKING_COUNT -eq 4 ]; then
    echo "✅ All services configured correctly!"
elif [ $WORKING_COUNT -gt 2 ]; then
    echo "⚠️  Most services working, check failed items above"
else
    echo "❌ Configuration needs attention"
fi

echo ""
echo "🔗 Quick Test URLs:"
echo "   Main site: https://$DOMAIN"
echo "   API health: https://api.$DOMAIN/health"
echo "   API docs: https://api.$DOMAIN/swagger"
echo ""
echo "✅ DNS configuration check completed!"
