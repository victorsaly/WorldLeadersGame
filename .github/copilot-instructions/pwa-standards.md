# 📱 PWA Standards - World Leaders Game

**Module Purpose**: Progressive Web App validation, branding, icons, and manifest requirements for educational game deployment.

**Use This Module**: When implementing PWA features, validating branding, or preparing for app store deployment.

---

## 🎯 PWA Requirements

### Core PWA Functionality
- **Service Worker**: Offline capability for educational continuity
- **Web App Manifest**: Proper app identity and branding
- **Icon Set**: Complete icon family for all platforms
- **Installability**: Native app-like experience
- **Performance**: Lighthouse PWA score > 90

### Educational PWA Benefits
- **Offline Learning**: Continue gameplay without internet
- **Home Screen Installation**: Easy access for 12-year-old users
- **Native Feel**: App-like experience enhances engagement
- **Cross-Platform**: Works on tablets, phones, and computers

## 🏷️ Branding Standards

### World Leaders Game Brand Identity
```json
{
  "name": "World Leaders Game",
  "shortName": "WorldLeaders",
  "description": "Educational strategy game teaching geography, economics, and languages to 12-year-olds",
  "tagline": "Conquer the World Through Learning",
  "brandColors": {
    "primary": "#22c55e",
    "secondary": "#16a34a", 
    "accent": "#3b82f6",
    "background": "#f0fdf4"
  },
  "designStyle": "32-bit Retro Pixel Art",
  "targetAudience": "12-year-old learners",
  "educationalFocus": "Geography, Economics, Languages"
}
```

### Logo Requirements
- **Primary Logo**: 32-bit pixel art style with green theme
- **Icon Logo**: Simplified version for small sizes
- **Horizontal Logo**: For wide layouts and headers
- **Monochrome Logo**: Single color version for various uses
- **Educational Context**: Must convey learning and world exploration

## 📱 Web App Manifest

### Complete Manifest Template
```json
{
  "name": "World Leaders Game - Educational Strategy",
  "short_name": "WorldLeaders",
  "description": "Educational strategy game teaching 12-year-olds about geography, economics, and languages through AI-assisted gameplay",
  "start_url": "/",
  "display": "standalone",
  "orientation": "any",
  "theme_color": "#22c55e",
  "background_color": "#f0fdf4",
  "scope": "/",
  "lang": "en-US",
  "dir": "ltr",
  "categories": ["education", "games", "kids"],
  "screenshots": [
    {
      "src": "/images/screenshots/desktop-gameplay.png",
      "sizes": "1280x720",
      "type": "image/png",
      "platform": "wide",
      "label": "Main gameplay on desktop"
    },
    {
      "src": "/images/screenshots/mobile-gameplay.png", 
      "sizes": "640x1136",
      "type": "image/png",
      "platform": "narrow",
      "label": "Mobile-friendly gameplay"
    }
  ],
  "icons": [
    {
      "src": "/images/icons/icon-72x72.png",
      "sizes": "72x72",
      "type": "image/png",
      "purpose": "maskable any"
    },
    {
      "src": "/images/icons/icon-96x96.png",
      "sizes": "96x96", 
      "type": "image/png",
      "purpose": "maskable any"
    },
    {
      "src": "/images/icons/icon-128x128.png",
      "sizes": "128x128",
      "type": "image/png",
      "purpose": "maskable any"
    },
    {
      "src": "/images/icons/icon-144x144.png",
      "sizes": "144x144",
      "type": "image/png",
      "purpose": "maskable any"
    },
    {
      "src": "/images/icons/icon-152x152.png",
      "sizes": "152x152",
      "type": "image/png",
      "purpose": "maskable any"
    },
    {
      "src": "/images/icons/icon-192x192.png",
      "sizes": "192x192",
      "type": "image/png",
      "purpose": "maskable any"
    },
    {
      "src": "/images/icons/icon-384x384.png",
      "sizes": "384x384",
      "type": "image/png",
      "purpose": "maskable any"
    },
    {
      "src": "/images/icons/icon-512x512.png",
      "sizes": "512x512",
      "type": "image/png",
      "purpose": "maskable any"
    }
  ],
  "shortcuts": [
    {
      "name": "Start New Game",
      "short_name": "New Game",
      "description": "Begin a new educational world conquest",
      "url": "/game/new",
      "icons": [
        {
          "src": "/images/shortcuts/new-game-96x96.png",
          "sizes": "96x96"
        }
      ]
    },
    {
      "name": "Continue Game", 
      "short_name": "Continue",
      "description": "Resume your world leadership journey",
      "url": "/game/continue",
      "icons": [
        {
          "src": "/images/shortcuts/continue-96x96.png",
          "sizes": "96x96"
        }
      ]
    }
  ]
}
```

## 🖼️ Icon Requirements

### Icon Specifications
```typescript
interface IconRequirements {
  sizes: [
    '72x72',   // Android Chrome
    '96x96',   // Android Chrome
    '128x128', // Chrome Web Store
    '144x144', // Windows
    '152x152', // iOS Safari
    '192x192', // Android Chrome (recommended)
    '384x384', // Android Chrome
    '512x512'  // Android Chrome (required)
  ];
  formats: ['PNG', 'SVG'];
  purposes: ['any', 'maskable'];
  design: 'retro-pixel-art';
  theme: 'green-education';
}
```

### Icon Design Guidelines
```css
/* Icon design principles */
.app-icon {
  /* Core Requirements */
  background: linear-gradient(135deg, #22c55e 0%, #16a34a 100%);
  border-radius: 22%; /* Modern app icon rounding */
  padding: 10%; /* Safe area for maskable icons */
  
  /* Pixel Art Elements */
  image-rendering: pixelated;
  image-rendering: -moz-crisp-edges;
  image-rendering: crisp-edges;
  
  /* Educational Symbols */
  /* Globe, book, or world map central element */
  /* Bright, recognizable 32-bit style */
  /* Child-friendly color scheme */
}

.icon-maskable {
  /* Safe area for maskable icons */
  padding: 20%; /* Larger safe area */
  background: radial-gradient(circle, #22c55e 0%, #16a34a 100%);
}
```

## 🔧 Service Worker Implementation

### Educational PWA Service Worker
```typescript
// service-worker.ts - Educational game optimized
const CACHE_NAME = 'world-leaders-v1.0.0';
const EDUCATIONAL_ASSETS = [
  '/',
  '/game',
  '/offline',
  '/images/icons/',
  '/images/characters/',
  '/images/flags/',
  '/css/retro-styles.css',
  '/js/game-core.js',
  '/data/countries.json',
  '/data/educational-content.json'
];

// Install event - cache educational assets
self.addEventListener('install', (event: ExtendableEvent) => {
  event.waitUntil(
    caches.open(CACHE_NAME)
      .then(cache => cache.addAll(EDUCATIONAL_ASSETS))
      .then(() => self.skipWaiting())
  );
});

// Fetch event - serve cached content for offline learning
self.addEventListener('fetch', (event: FetchEvent) => {
  if (event.request.destination === 'document') {
    // Serve game pages from cache when offline
    event.respondWith(
      caches.match(event.request)
        .then(response => response || fetch(event.request))
        .catch(() => caches.match('/offline'))
    );
  } else {
    // Cache-first strategy for game assets
    event.respondWith(
      caches.match(event.request)
        .then(response => response || fetch(event.request))
    );
  }
});
```

### Offline Page for Educational Continuity
```html
<!-- offline.html -->
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>World Leaders Game - Offline</title>
    <link rel="stylesheet" href="/css/retro-styles.css">
</head>
<body class="retro-background">
    <div class="offline-container">
        <div class="retro-card">
            <h1 class="retro-heading-xl">🌍 Offline Mode</h1>
            <p class="retro-body">
                No internet? No problem! Continue your world leadership journey 
                with cached educational content.
            </p>
            <div class="offline-features">
                <div class="feature-item">
                    <span class="emoji">🎮</span>
                    <span>Continue saved games</span>
                </div>
                <div class="feature-item">
                    <span class="emoji">📚</span>
                    <span>Review learned countries</span>
                </div>
                <div class="feature-item">
                    <span class="emoji">🗺️</span>
                    <span>Explore world map</span>
                </div>
            </div>
            <button class="retro-button" onclick="location.reload()">
                🔄 Check Connection
            </button>
        </div>
    </div>
</body>
</html>
```

## ✅ PWA Validation Checklist

### Pre-Deployment Validation
```typescript
interface PWAValidationChecklist {
  manifest: {
    present: boolean;           // ✅ manifest.json exists
    valid: boolean;             // ✅ Valid JSON structure
    icons: boolean;             // ✅ All required icon sizes
    screenshots: boolean;       // ✅ Desktop and mobile screenshots
    startUrl: boolean;          // ✅ Valid start_url
    scope: boolean;             // ✅ Proper scope definition
  };
  
  serviceWorker: {
    registered: boolean;        // ✅ Service worker registration
    caching: boolean;           // ✅ Offline asset caching
    offlinePage: boolean;       // ✅ Offline fallback page
    updateFlow: boolean;        // ✅ Update notification system
  };
  
  icons: {
    allSizes: boolean;          // ✅ 72x72 through 512x512
    maskable: boolean;          // ✅ Maskable purpose icons
    pixelArt: boolean;          // ✅ Retro pixel art design
    educational: boolean;       // ✅ Educational theme visible
  };
  
  performance: {
    lighthouse: number;         // ✅ PWA score > 90
    loadTime: number;           // ✅ < 2 seconds initial load
    cacheRatio: number;         // ✅ > 80% assets cached
  };
  
  branding: {
    logoPresent: boolean;       // ✅ Primary logo implemented
    colorConsistent: boolean;   // ✅ Brand colors throughout
    retroTheme: boolean;        // ✅ 32-bit pixel art theme
    educationalContext: boolean; // ✅ Learning theme evident
  };
}
```

### Automated Validation Script
```bash
#!/bin/bash
# validate-pwa.sh - Comprehensive PWA validation

echo "🔍 Validating World Leaders Game PWA..."

# Check manifest.json
if [ -f "src/WorldLeaders/WorldLeaders.Web/wwwroot/manifest.json" ]; then
    echo "✅ Manifest file found"
    # Validate JSON structure
    if cat src/WorldLeaders/WorldLeaders.Web/wwwroot/manifest.json | python -m json.tool > /dev/null 2>&1; then
        echo "✅ Manifest JSON is valid"
    else
        echo "❌ Invalid manifest JSON"
        exit 1
    fi
else
    echo "❌ Manifest file missing"
    exit 1
fi

# Check required icons
REQUIRED_ICONS=("72x72" "96x96" "128x128" "144x144" "152x152" "192x192" "384x384" "512x512")
ICON_PATH="src/WorldLeaders/WorldLeaders.Web/wwwroot/images/icons"

for size in "${REQUIRED_ICONS[@]}"; do
    if [ -f "${ICON_PATH}/icon-${size}.png" ]; then
        echo "✅ Icon ${size} found"
    else
        echo "❌ Missing icon: ${size}"
        exit 1
    fi
done

# Check service worker
if [ -f "src/WorldLeaders/WorldLeaders.Web/wwwroot/sw.js" ]; then
    echo "✅ Service worker found"
else
    echo "❌ Service worker missing"
    exit 1
fi

# Check offline page
if [ -f "src/WorldLeaders/WorldLeaders.Web/wwwroot/offline.html" ]; then
    echo "✅ Offline page found"
else
    echo "❌ Offline page missing"
    exit 1
fi

# Validate educational branding
if grep -q "Educational" src/WorldLeaders/WorldLeaders.Web/wwwroot/manifest.json; then
    echo "✅ Educational branding present"
else
    echo "❌ Educational branding missing"
    exit 1
fi

echo "🎉 PWA validation completed successfully!"
```

## 🚀 Deployment Validation

### Production PWA Checklist
- [ ] **Manifest Valid**: JSON structure correct and complete
- [ ] **Icons Complete**: All 8 required sizes (72x72 to 512x512)
- [ ] **Service Worker**: Registered and caching educational assets
- [ ] **Offline Support**: Graceful offline experience for learning continuity
- [ ] **Retro Branding**: 32-bit pixel art theme consistent throughout
- [ ] **Educational Context**: Learning mission evident in all branding
- [ ] **Performance**: Lighthouse PWA score > 90
- [ ] **Mobile Optimized**: Touch-friendly interface for tablets
- [ ] **Accessibility**: WCAG 2.1 AA compliance
- [ ] **Child Safety**: All content appropriate for 12-year-olds

### App Store Preparation
```json
{
  "app_store_assets": {
    "screenshots": {
      "desktop": ["1280x720", "1920x1080"],
      "tablet": ["1024x768", "1366x1024"],
      "mobile": ["375x667", "414x896"]
    },
    "descriptions": {
      "short": "Educational strategy game teaching geography, economics, and languages",
      "full": "World Leaders Game combines strategic gameplay with real-world education, teaching 12-year-olds about geography, economics, and languages through AI-assisted learning and retro pixel art design.",
      "keywords": ["education", "geography", "strategy", "kids", "learning", "retro"]
    },
    "age_rating": "4+",
    "category": "Education",
    "educational_compliance": "COPPA, GDPR"
  }
}
```

## 📚 Cross-Module Relationships

### This Module Connects To:
- **[retro-design-standards.md](./retro-design-standards.md)**: Retro branding and visual identity
- **[ui-ux-guidelines.md](./ui-ux-guidelines.md)**: Child-friendly design principles
- **[technical-architecture.md](./technical-architecture.md)**: PWA implementation patterns

### PWA Integration Pattern:
```
Technical Architecture (service worker, manifest)
↓
Retro Design Standards (branding, icons)
↓
PWA Standards (this module)
↓
= Complete educational PWA with retro branding
```

---

**Remember**: PWA implementation must enhance the educational experience while maintaining the retro aesthetic and ensuring safe, engaging gameplay for 12-year-old learners.
