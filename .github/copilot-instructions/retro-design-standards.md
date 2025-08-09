# 🎨 Retro Design Standards - World Leaders Game

**Module Purpose**: 32-bit pixel art design standards and retro aesthetic implementation for child designer vision.

**Use This Module**: When implementing retro transformation, pixel art components, or 32-bit styling elements.

---

## 🎮 Retro Design Philosophy

### Child Designer Vision (12-Year-Old Led)
- **Green Background Theme**: Primary green gradients and nature-inspired palettes
- **32-Bit Pixel Art**: Clean, readable pixel art suitable for educational content
- **Character Personas**: Replace name input with visual character selection
- **Interactive World Map**: Pixel art countries with engaging interactions
- **Mobile-First Retro**: Touch-friendly pixel art optimized for tablets

### Educational Preservation
- **Learning Value**: All retro elements must enhance, not distract from education
- **Age Appropriateness**: Pixel art styling suitable for 12-year-old learners
- **Cultural Sensitivity**: Respectful representation in all visual elements
- **Accessibility**: Retro design meeting WCAG 2.1 AA standards

## 🎨 Retro Color Palette

### Primary Green Theme
```css
:root {
  /* Child Designer Green Theme */
  --retro-green-primary: #22c55e;      /* Bright, engaging green */
  --retro-green-secondary: #16a34a;    /* Deeper green for contrast */
  --retro-green-light: #86efac;        /* Light green for highlights */
  --retro-green-dark: #15803d;         /* Dark green for depth */
  
  /* 32-Bit Complementary Colors */
  --retro-blue: #3b82f6;               /* Classic game blue */
  --retro-yellow: #fbbf24;             /* Coin/achievement gold */
  --retro-red: #ef4444;                /* Attention/warning */
  --retro-purple: #8b5cf6;             /* Magic/special elements */
  
  /* Pixel Art Neutrals */
  --retro-black: #1f2937;              /* Deep shadows */
  --retro-white: #f9fafb;              /* Highlights */
  --retro-gray-dark: #4b5563;          /* Mid-tone shadows */
  --retro-gray-light: #d1d5db;         /* Light outlines */
}
```

### Gradient Combinations
```css
/* Green Theme Gradients */
.retro-gradient-primary {
  background: linear-gradient(135deg, 
    var(--retro-green-primary) 0%, 
    var(--retro-green-secondary) 100%);
}

.retro-gradient-nature {
  background: linear-gradient(135deg, 
    var(--retro-green-light) 0%, 
    var(--retro-green-primary) 50%, 
    var(--retro-green-dark) 100%);
}

.retro-gradient-world {
  background: linear-gradient(135deg, 
    var(--retro-blue) 0%, 
    var(--retro-green-primary) 100%);
}
```

## 🖼️ Pixel Art Component Standards

### Button Styling
```css
.retro-button {
  @apply relative px-6 py-3 font-bold text-white;
  background: var(--retro-green-primary);
  border: 3px solid var(--retro-white);
  border-radius: 0; /* No rounded corners for pixel art */
  box-shadow: 
    3px 3px 0 var(--retro-green-dark),
    6px 6px 0 var(--retro-black);
  transition: all 0.1s ease-in-out;
  image-rendering: pixelated;
  font-family: 'Press Start 2P', monospace;
  font-size: 12px;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.retro-button:hover {
  transform: translate(2px, 2px);
  box-shadow: 
    1px 1px 0 var(--retro-green-dark),
    2px 2px 0 var(--retro-black);
}

.retro-button:active {
  transform: translate(3px, 3px);
  box-shadow: none;
}
```

### Card Components
```css
.retro-card {
  @apply bg-white p-6 relative;
  border: 4px solid var(--retro-black);
  box-shadow: 
    4px 4px 0 var(--retro-gray-dark),
    8px 8px 0 var(--retro-black);
  image-rendering: pixelated;
  background-image: 
    linear-gradient(45deg, transparent 40%, rgba(34, 197, 94, 0.1) 50%, transparent 60%);
}

.retro-card-green {
  background: linear-gradient(135deg, 
    var(--retro-green-light) 0%, 
    var(--retro-white) 100%);
  border-color: var(--retro-green-primary);
}
```

### Typography
```css
/* Retro Typography System */
.retro-heading-xl {
  font-family: 'Press Start 2P', monospace;
  font-size: 24px;
  line-height: 1.2;
  color: var(--retro-green-primary);
  text-shadow: 2px 2px 0 var(--retro-black);
  letter-spacing: 2px;
  image-rendering: pixelated;
}

.retro-heading-lg {
  font-family: 'Press Start 2P', monospace;
  font-size: 18px;
  line-height: 1.3;
  color: var(--retro-green-dark);
  text-shadow: 1px 1px 0 var(--retro-black);
  letter-spacing: 1px;
}

.retro-body {
  font-family: 'Orbitron', monospace;
  font-size: 14px;
  line-height: 1.6;
  color: var(--retro-black);
  font-weight: 500;
}
```

## 👤 Character Persona Design

### Character Selection Interface
```razor
@* Retro Character Selection Component *@
<div class="character-selection-grid">
    @foreach (var character in AvailableCharacters)
    {
        <div class="character-card @(IsSelected(character) ? "selected" : "")" 
             @onclick="() => SelectCharacter(character)">
            <div class="character-avatar">
                <img src="/images/characters/@(character.Id).png" 
                     alt="@character.Name" 
                     class="pixel-art-image" />
            </div>
            <div class="character-info">
                <h3 class="retro-heading-sm">@character.Name</h3>
                <p class="retro-body-small">@character.Description</p>
                <div class="character-stats">
                    @foreach (var trait in character.Traits)
                    {
                        <span class="trait-badge">@trait</span>
                    }
                </div>
            </div>
        </div>
    }
</div>

<style>
.character-selection-grid {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
    gap: 16px;
    padding: 20px;
}

.character-card {
    @apply bg-white border-4 border-black p-4 cursor-pointer;
    box-shadow: 4px 4px 0 var(--retro-gray-dark);
    transition: all 0.1s ease-in-out;
    image-rendering: pixelated;
}

.character-card:hover {
    transform: translate(-2px, -2px);
    box-shadow: 6px 6px 0 var(--retro-gray-dark);
}

.character-card.selected {
    border-color: var(--retro-green-primary);
    background: linear-gradient(135deg, 
        var(--retro-green-light) 0%, 
        var(--retro-white) 100%);
}

.character-avatar {
    @apply w-16 h-16 mx-auto mb-3;
    image-rendering: pixelated;
}

.pixel-art-image {
    @apply w-full h-full object-cover;
    image-rendering: pixelated;
    image-rendering: -moz-crisp-edges;
    image-rendering: crisp-edges;
}

.trait-badge {
    @apply inline-block px-2 py-1 text-xs;
    background: var(--retro-green-primary);
    color: white;
    border: 1px solid var(--retro-black);
    font-family: 'Press Start 2P', monospace;
    font-size: 8px;
}
</style>
```

## 🗺️ Interactive World Map Design

### Pixel Art Countries
```css
.world-map-container {
    @apply w-full h-full relative overflow-hidden;
    background: linear-gradient(135deg, 
        var(--retro-blue) 0%, 
        var(--retro-green-light) 100%);
    image-rendering: pixelated;
}

.country-territory {
    @apply absolute cursor-pointer;
    border: 2px solid transparent;
    transition: all 0.2s ease-in-out;
    image-rendering: pixelated;
}

.country-territory:hover {
    border-color: var(--retro-yellow);
    transform: scale(1.05);
    z-index: 10;
    filter: brightness(1.2);
}

.country-territory.owned {
    border-color: var(--retro-green-primary);
    background-color: rgba(34, 197, 94, 0.3);
}

.country-territory.available {
    border-color: var(--retro-gray-light);
    opacity: 0.8;
}

.country-territory.locked {
    border-color: var(--retro-red);
    opacity: 0.5;
    filter: grayscale(1);
}
```

## 📱 Mobile-First Retro Design

### Touch-Friendly Pixel Art
```css
/* Mobile-optimized retro components */
@media (max-width: 768px) {
    .retro-button {
        @apply min-h-[56px] px-8 py-4;
        font-size: 14px;
        border-width: 4px;
    }
    
    .retro-card {
        @apply p-4;
        border-width: 3px;
    }
    
    .character-card {
        @apply p-3;
        border-width: 3px;
    }
    
    .retro-heading-xl {
        font-size: 20px;
    }
    
    .retro-heading-lg {
        font-size: 16px;
    }
}

/* Touch target improvements */
.touch-target {
    @apply min-h-[44px] min-w-[44px];
    touch-action: manipulation;
}
```

## 🎯 Animation Standards

### Retro Game Animations
```css
/* Pixel art friendly animations */
@keyframes retro-bounce {
    0%, 100% { transform: translateY(0); }
    50% { transform: translateY(-8px); }
}

@keyframes retro-glow {
    0%, 100% { 
        box-shadow: 0 0 5px var(--retro-green-primary);
    }
    50% { 
        box-shadow: 0 0 20px var(--retro-green-primary),
                    0 0 30px var(--retro-green-light);
    }
}

@keyframes pixel-shimmer {
    0% { opacity: 1; }
    50% { opacity: 0.7; }
    100% { opacity: 1; }
}

.retro-animate-bounce {
    animation: retro-bounce 1s ease-in-out infinite;
}

.retro-animate-glow {
    animation: retro-glow 2s ease-in-out infinite;
}

.retro-animate-shimmer {
    animation: pixel-shimmer 1.5s ease-in-out infinite;
}
```

## 🎵 Retro UI Feedback

### Sound Integration
```typescript
// Retro sound effects for UI interactions
export class RetroSoundManager {
    private sounds = {
        buttonClick: '/sounds/retro-click.wav',
        achievement: '/sounds/retro-success.wav',
        error: '/sounds/retro-error.wav',
        characterSelect: '/sounds/retro-select.wav',
        territoryPurchase: '/sounds/retro-purchase.wav'
    };

    playSound(soundType: keyof typeof this.sounds) {
        const audio = new Audio(this.sounds[soundType]);
        audio.volume = 0.3; // Child-friendly volume
        audio.play().catch(() => {
            // Handle autoplay restrictions gracefully
        });
    }
}
```

## 📚 Cross-Module Relationships

### This Module Connects To:
- **[ui-ux-guidelines.md](./ui-ux-guidelines.md)**: Base child-friendly design principles
- **[core-principles.md](./core-principles.md)**: Educational mission and child safety
- **[pwa-standards.md](./pwa-standards.md)**: Progressive Web App requirements

### Retro Design Pattern:
```
Core Principles (educational mission)
↓
UI/UX Guidelines (child-friendly base)
↓
Retro Design Standards (this module)
↓
= Educational pixel art game with child designer vision
```

---

**Remember**: Every retro design element must enhance the educational experience while honoring the 12-year-old designer's creative vision. Pixel art should be clear, engaging, and culturally sensitive.
