---
layout: page
title: "Pixel Art Asset Creation Guidelines"
date: 2025-08-09
category: "design-guide"
tags: ["pixel-art", "assets", "retro", "32-bit"]
educational_objective: "Create consistent pixel art assets for educational game"
---

# 🎨 Pixel Art Asset Creation Guidelines

**Context**: 32-bit retro transformation requires consistent pixel art assets  
**Educational Objective**: Maintain visual consistency while supporting learning  
**Target**: Character avatars, UI elements, world map graphics, educational icons

---

## 🎯 Design Specifications

### Pixel Art Standards
- **Resolution**: 32x32px base unit (scalable to 64x64, 128x128)
- **Color Palette**: Limited to retro green theme + 8 accent colors
- **Style**: Clean, readable pixel art suitable for 12-year-olds
- **File Format**: PNG with transparency, SVG for scalable elements

### Educational Requirements
- **Cultural Sensitivity**: Respectful representation of all countries/characters
- **Child-Friendly**: Age-appropriate designs with positive messaging
- **Accessibility**: High contrast ratios and clear visual distinction
- **Learning Support**: Visual elements reinforce educational objectives

---

## 🌈 Color Palette Guidelines

### Primary Colors (Required)
```css
/* Retro Green Theme */
--retro-green-primary: #22c55e;   /* Main green - buttons, highlights */
--retro-green-secondary: #16a34a; /* Darker green - borders, shadows */
--retro-green-light: #86efac;     /* Light green - backgrounds, highlights */
--retro-green-dark: #15803d;      /* Dark green - text, outlines */

/* Support Colors */
--retro-blue: #3b82f6;            /* Water, sky, cool elements */
--retro-yellow: #fbbf24;          /* Coins, achievements, warm highlights */
--retro-red: #ef4444;             /* Warnings, alerts (use sparingly) */
--retro-purple: #8b5cf6;          /* Magic, special elements */

/* Neutrals */
--retro-black: #1f2937;           /* Outlines, text, shadows */
--retro-white: #f9fafb;           /* Highlights, backgrounds */
--retro-gray-dark: #4b5563;       /* Mid-tone shadows */
--retro-gray-light: #d1d5db;      /* Light outlines, disabled states */
```

### Color Usage Rules
- **Maximum 4 colors per asset** (excluding black/white outlines)
- **Green must be prominent** in 80% of UI elements
- **Red used sparingly** - only for critical warnings
- **High contrast** between adjacent colors (3:1 minimum)

---

## 👤 Character Avatar Specifications

### Character Persona Assets Needed
```typescript
interface CharacterAssets {
  characters: [
    {
      id: "explorer",
      name: "Alex Explorer", 
      sizes: ["32x32", "64x64", "128x128"],
      expressions: ["neutral", "happy", "thinking"],
      cultural_background: "Adventure gear, neutral appearance"
    },
    {
      id: "strategist",
      name: "Sam Strategist",
      sizes: ["32x32", "64x64", "128x128"], 
      expressions: ["neutral", "happy", "thinking"],
      cultural_background: "Professional attire, neutral appearance"
    },
    {
      id: "diplomat",
      name: "Jordan Diplomat",
      sizes: ["32x32", "64x64", "128x128"],
      expressions: ["neutral", "happy", "thinking"],
      cultural_background: "Formal wear, neutral appearance"
    },
    {
      id: "innovator", 
      name: "Casey Innovator",
      sizes: ["32x32", "64x64", "128x128"],
      expressions: ["neutral", "happy", "thinking"],
      cultural_background: "Creative attire, neutral appearance"
    },
    {
      id: "scholar",
      name: "River Scholar", 
      sizes: ["32x32", "64x64", "128x128"],
      expressions: ["neutral", "happy", "thinking"],
      cultural_background: "Academic attire, neutral appearance"
    },
    {
      id: "builder",
      name: "Taylor Builder",
      sizes: ["32x32", "64x64", "128x128"],
      expressions: ["neutral", "happy", "thinking"],  
      cultural_background: "Work attire, neutral appearance"
    }
  ]
}
```

### Character Design Guidelines
- **Gender Neutral**: All characters should be androgynous and welcoming
- **Cultural Diversity**: Represent global diversity respectfully
- **Age Appropriate**: Friendly, non-intimidating designs
- **Professional Quality**: Clean pixel art with consistent style
- **Scalability**: Work well at 32px and larger sizes

---

## 🗺️ World Map Asset Requirements

### Country Territory Graphics
```typescript
interface WorldMapAssets {
  countries: {
    format: "SVG with pixel art styling",
    style: "Simplified country shapes with retro borders",
    colors: {
      default: "--retro-gray-light",
      available: "--retro-blue", 
      owned: "--retro-green-primary",
      locked: "--retro-gray-dark"
    },
    hover_effects: "Brightness increase + pixel art glow",
    size_categories: ["small", "medium", "large"] // based on GDP tiers
  },
  
  ui_elements: {
    map_background: "Gradient from blue (water) to green (land)",
    borders: "2px solid black pixel art style", 
    selection_indicators: "Animated pixel art highlights",
    info_panels: "Retro card design with country information"
  }
}
```

### Map Design Standards
- **Simplified Geography**: Clear, recognizable country shapes
- **Pixel Art Borders**: 2-4px solid borders with retro styling
- **Interactive States**: Clear visual feedback for hover/selection
- **Educational Context**: Visual indicators for learning content

---

## 🎮 UI Element Assets

### Button Components
```css
/* Retro button sprite specifications */
.retro-button-assets {
  /* Base button: 120x40px minimum */
  normal: "Green gradient with black border and drop shadow";
  hover: "Slightly translated with reduced shadow"; 
  active: "Fully translated with no shadow";
  disabled: "Grayscale with reduced opacity";
  
  /* Special buttons */
  primary: "Green theme with white text";
  secondary: "Blue theme with white text";
  warning: "Yellow theme with black text";
  danger: "Red theme with white text (use sparingly)";
}
```

### Card Components
```css
/* Retro card sprite specifications */
.retro-card-assets {
  /* Standard card: 300x200px base */
  border: "4px solid black with 8bit corners";
  background: "White with subtle green accent gradient";
  shadow: "Offset black shadow for depth";
  header: "Green gradient header section";
  
  /* Card variations */
  character_card: "Focus on character display";
  territory_card: "Country information layout";
  achievement_card: "Celebration styling with effects";
}
```

### Icon Set Requirements
```typescript
interface IconAssets {
  game_icons: [
    "dice_1", "dice_2", "dice_3", "dice_4", "dice_5", "dice_6", // Dice faces
    "coin", "reputation", "happiness",                           // Resources
    "territory", "flag", "language",                            // Game elements
    "settings", "help", "back", "forward"                       // Navigation
  ],
  
  educational_icons: [
    "geography", "economics", "language",                        // Learning areas
    "world", "country", "city",                                 // Geographic concepts
    "growth", "success", "achievement"                          // Progress indicators
  ],
  
  specifications: {
    size: "32x32px base (16x16, 64x64 variants)",
    style: "Pixel art with black outlines",
    colors: "Maximum 3 colors per icon + black outline",
    format: "PNG with transparency"
  }
}
```

---

## 📱 PWA Icon Specifications

### App Icon Design Requirements
```typescript
interface PWAIconSpecs {
  sizes: ["72x72", "96x96", "128x128", "144x144", "152x152", "192x192", "384x384", "512x512"],
  
  design_elements: {
    background: "Green gradient (#22c55e to #16a34a)",
    main_symbol: "Globe or world map in pixel art style",
    secondary_elements: "Education symbols (book, graduation cap)",
    safe_area: "20% padding for maskable icons",
    style: "32-bit pixel art with educational theme"
  },
  
  requirements: {
    format: "PNG with square aspect ratio",
    purpose: "maskable any", 
    theme: "Educational gaming with retro aesthetic",
    recognition: "Instantly recognizable as educational game"
  }
}
```

### Icon Creation Checklist
- [ ] **Conceptual Design**: Globe + education symbols in pixel art
- [ ] **Color Consistency**: Green theme with high contrast
- [ ] **Scalability**: Readable at smallest size (72x72)
- [ ] **Safe Area**: 20% padding for maskable versions
- [ ] **Brand Alignment**: Consistent with game's educational mission
- [ ] **Platform Testing**: Verify appearance on iOS and Android

---

## 🔧 Asset Creation Workflow

### Design Process
1. **Concept Sketching**: Hand-drawn concepts honoring child designer vision
2. **Pixel Art Creation**: Use specialized tools (Aseprite, Pixaki, Photoshop)
3. **Color Application**: Apply consistent retro green palette
4. **Size Variations**: Create multiple resolutions from base design
5. **Educational Review**: Verify age-appropriateness and learning support
6. **Technical Validation**: Test rendering and performance

### Recommended Tools
- **Aseprite**: Professional pixel art animation tool
- **Pixaki (iPad)**: Touch-friendly pixel art creation
- **Photoshop**: With pixel art brushes and settings
- **GIMP**: Free alternative with pixel art plugins
- **Online Tools**: Piskel, Pixilart for quick prototypes

### File Organization
```
src/WorldLeaders/WorldLeaders.Web/wwwroot/images/
├── characters/
│   ├── explorer-32x32.png
│   ├── explorer-64x64.png
│   ├── explorer-128x128.png
│   └── ... (all 6 characters, all sizes)
├── icons/
│   ├── ui/
│   │   ├── dice-32x32.png
│   │   ├── coin-32x32.png
│   │   └── ... (all UI icons)
│   └── pwa/
│       ├── icon-72x72.png
│       ├── icon-96x96.png
│       └── ... (all PWA sizes)
├── maps/
│   ├── world-background.svg
│   ├── countries/
│   │   ├── usa.svg
│   │   ├── canada.svg
│   │   └── ... (all countries)
└── ui-elements/
    ├── buttons/
    ├── cards/
    └── decorative/
```

---

## ✅ Quality Assurance

### Design Validation Checklist
- [ ] **Pixel Perfect**: Clean pixel art without anti-aliasing
- [ ] **Color Compliance**: Uses approved retro green palette
- [ ] **Educational Appropriate**: Suitable for 12-year-old audience
- [ ] **Cultural Sensitivity**: Respectful representation
- [ ] **Accessibility**: High contrast and clear recognition
- [ ] **Performance**: Optimized file sizes without quality loss
- [ ] **Consistency**: Matches established visual style
- [ ] **Scalability**: Works at multiple sizes

### Testing Requirements
- [ ] **Multiple Devices**: Test on tablets, phones, desktops
- [ ] **Different Resolutions**: Verify crisp rendering at all sizes
- [ ] **Color Blindness**: Test accessibility for color vision deficiency
- [ ] **Loading Performance**: Measure impact on game load times
- [ ] **Child User Testing**: Verify appeal and recognition with target age

---

## 🎨 Implementation Integration

### CSS Pixel Art Optimization
```css
/* Ensure crisp pixel art rendering */
.pixel-art {
  image-rendering: pixelated;
  image-rendering: -moz-crisp-edges;
  image-rendering: crisp-edges;
  image-rendering: -webkit-optimize-contrast;
}

/* Retro scaling for different screen sizes */
@media (max-width: 768px) {
  .pixel-art-icon {
    image-rendering: auto; /* Better scaling on small screens */
  }
}
```

### Component Integration
```razor
@* Character selection with pixel art assets *@
<div class="character-grid">
    @foreach (var character in Characters)
    {
        <div class="character-card">
            <img src="/images/characters/@(character.Id)-64x64.png" 
                 alt="@character.Name"
                 class="pixel-art character-avatar" />
            <h3 class="retro-heading-sm">@character.Name</h3>
        </div>
    }
</div>
```

---

**Remember**: Every asset must support the educational mission while honoring the 12-year-old designer's retro pixel art vision. Quality and consistency are essential for maintaining the game's professional educational standards!
