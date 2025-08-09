#!/bin/bash
# validate-pwa.sh - Comprehensive PWA and branding validation

echo "📱 Validating World Leaders Game PWA and Branding..."

# Check manifest.json
if [ -f "src/WorldLeaders/WorldLeaders.Web/wwwroot/manifest.json" ]; then
    echo "✅ PWA Manifest found"
    
    # Validate JSON structure
    if cat src/WorldLeaders/WorldLeaders.Web/wwwroot/manifest.json | python -m json.tool > /dev/null 2>&1; then
        echo "✅ Manifest JSON is valid"
    else
        echo "❌ Invalid manifest JSON"
        exit 1
    fi
    
    # Validate educational branding in manifest
    if grep -q "Educational" src/WorldLeaders/WorldLeaders.Web/wwwroot/manifest.json; then
        echo "✅ Educational branding in manifest"
    else
        echo "❌ Missing educational branding"
        exit 1
    fi
else
    echo "❌ PWA Manifest missing"
    exit 1
fi

# Check required icons
REQUIRED_ICONS=("72x72" "96x96" "128x128" "144x144" "152x152" "192x192" "384x384" "512x512")
ICON_PATH="src/WorldLeaders/WorldLeaders.Web/wwwroot/images/icons"

echo "🖼️ Validating PWA Icons..."
for size in "${REQUIRED_ICONS[@]}"; do
    if [ -f "${ICON_PATH}/icon-${size}.png" ]; then
        echo "✅ Icon ${size} found"
    else
        echo "❌ Missing icon: ${size}"
        exit 1
    fi
done

# Validate service worker
if [ -f "src/WorldLeaders/WorldLeaders.Web/wwwroot/sw.js" ]; then
    echo "✅ Service worker present"
else
    echo "❌ Service worker missing"
    exit 1
fi

# Check offline page
if [ -f "src/WorldLeaders/WorldLeaders.Web/wwwroot/offline.html" ]; then
    echo "✅ Offline page present"
else
    echo "❌ Offline page missing"
    exit 1
fi

# Check logo implementation
if [ -f "src/WorldLeaders/WorldLeaders.Web/wwwroot/images/logo.png" ] || [ -f "src/WorldLeaders/WorldLeaders.Web/wwwroot/images/logo.svg" ]; then
    echo "✅ Primary logo present"
else
    echo "❌ Primary logo missing"
    exit 1
fi

# Validate educational theme in branding
if grep -r "Educational\|Learning\|Geography" src/WorldLeaders/WorldLeaders.Web/wwwroot/ >/dev/null 2>&1; then
    echo "✅ Educational theme evident in branding"
else
    echo "❌ Educational theme missing from branding"
    exit 1
fi

echo "🎉 PWA and Branding validation completed successfully!"
