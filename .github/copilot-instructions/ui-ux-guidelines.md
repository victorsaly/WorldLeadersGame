# 🎨 UI/UX Guidelines - World Leaders Game

**Module Purpose**: Child-friendly design and accessibility standards for 12-year-old learners.

**Use This Module**: When designing UI components, implementing accessibility features, or creating child-friendly interfaces.

---

## 🧒 Child-Friendly Design Principles

### Core Design Philosophy
- **Large, Accessible Elements**: Easy interaction for 12-year-old coordination
- **Immediate Visual Feedback**: Clear response to every user action
- **Encouraging Visual Language**: Positive, supportive design elements
- **Clear Visual Hierarchy**: Obvious navigation and progression paths
- **Colorful but Purposeful**: Engaging colors that support learning objectives

### Age-Appropriate Interaction Design
- **Touch-Friendly Targets**: Minimum 44px touch targets for finger navigation
- **Simple Navigation**: No more than 3 levels deep in any user flow
- **Forgiving Interface**: Easy to recover from mistakes without losing progress
- **Progress Visualization**: Clear indicators of advancement and achievement
- **Consistent Patterns**: Predictable interactions across all game areas

## 🎯 Visual Design Standards

### Color Palette
```css
/* Primary Colors - Educational and Encouraging */
:root {
  --primary-blue: #3B82F6;      /* Trust, learning, calm focus */
  --primary-purple: #8B5CF6;    /* Creativity, imagination, magic */
  --success-green: #10B981;     /* Achievement, growth, positive */
  --warning-orange: #F59E0B;    /* Attention, caution, friendly alert */
  --error-red: #EF4444;         /* Problems (used sparingly) */
  --neutral-gray: #6B7280;      /* Secondary information */
  
  /* Gradient Combinations */
  --gradient-primary: linear-gradient(135deg, var(--primary-blue) 0%, var(--primary-purple) 100%);
  --gradient-success: linear-gradient(135deg, #10B981 0%, #34D399 100%);
  --gradient-warm: linear-gradient(135deg, #F59E0B 0%, #FBBF24 100%);
}
```

### Typography Scale
```css
/* Child-Friendly Typography */
.text-display {
  @apply text-4xl md:text-6xl font-bold tracking-tight;
  font-family: 'Comic Neue', 'Arial Rounded', sans-serif;
}

.text-heading {
  @apply text-2xl md:text-3xl font-bold;
  font-family: 'Comic Neue', 'Arial Rounded', sans-serif;
}

.text-body {
  @apply text-lg md:text-xl leading-relaxed;
  font-family: 'Open Sans', 'Arial', sans-serif;
}

.text-button {
  @apply text-lg font-bold tracking-wide;
  font-family: 'Comic Neue', 'Arial Rounded', sans-serif;
}
```

### Component Sizing
```css
/* Child-Friendly Component Sizes */
.btn-child-primary {
  @apply px-8 py-4 min-h-[56px] min-w-[120px];
  @apply text-lg font-bold rounded-xl;
  @apply shadow-lg border-4 border-white border-opacity-20;
  @apply transform transition-all duration-300;
  @apply hover:scale-105 hover:shadow-xl active:scale-95;
}

.card-child {
  @apply p-6 md:p-8 rounded-2xl shadow-xl;
  @apply border-4 border-opacity-10;
  @apply min-h-[120px];
  @apply transform transition-all duration-300 hover:scale-102;
}

.input-child {
  @apply px-4 py-3 text-lg rounded-lg;
  @apply border-3 border-gray-300 focus:border-blue-500;
  @apply min-h-[48px] min-w-[200px];
}
```

## 🎮 Game-Specific UI Patterns

### Resource Display Components
```razor
@* Happiness Meter Component *@
<div class="resource-meter">
    <div class="meter-label">
        <span class="text-2xl">😊</span>
        <span class="font-bold text-lg">Happiness</span>
    </div>
    <div class="meter-container">
        <div class="meter-fill" style="width: @(Happiness)%">
            <div class="meter-sparkle"></div>
        </div>
    </div>
    <div class="meter-value">@Happiness%</div>
</div>

<style>
.resource-meter {
    @apply bg-white rounded-xl p-4 shadow-lg border-4 border-opacity-10;
    @apply flex items-center space-x-4;
}

.meter-container {
    @apply flex-1 bg-gray-200 rounded-full h-6 overflow-hidden;
    @apply border-2 border-gray-300 shadow-inner;
}

.meter-fill {
    @apply bg-gradient-to-r from-green-400 to-blue-500 h-full rounded-full;
    @apply transition-all duration-1000 ease-out relative;
    @apply shadow-lg;
}

.meter-sparkle {
    @apply absolute inset-0 bg-gradient-to-r from-transparent via-white to-transparent;
    @apply opacity-30 animate-pulse;
}

.meter-value {
    @apply text-xl font-bold text-gray-700 min-w-[60px] text-center;
}
</style>
```

### Dice Rolling Component
```razor
@* Educational Dice Component *@
<div class="dice-container" @onclick="RollDice">
    <div class="dice @(IsRolling ? "rolling" : "")" >
        <div class="dice-face">@CurrentValue</div>
    </div>
    <div class="dice-instruction">
        <p class="text-lg font-bold text-white">Click to roll!</p>
        <p class="text-sm text-white opacity-80">Discover your next career opportunity</p>
    </div>
</div>

<style>
.dice-container {
    @apply flex flex-col items-center space-y-4 p-6;
    @apply bg-gradient-to-br from-purple-500 to-blue-600 rounded-2xl;
    @apply cursor-pointer transform transition-all duration-300;
    @apply hover:scale-105 active:scale-95;
}

.dice {
    @apply w-20 h-20 bg-white rounded-lg shadow-xl;
    @apply flex items-center justify-center;
    @apply text-3xl font-bold text-gray-800;
    @apply border-4 border-gray-200;
}

.dice.rolling {
    @apply animate-bounce;
    animation-duration: 0.8s;
    animation-iteration-count: 3;
}

.dice-face {
    @apply select-none;
}
</style>
```

### Territory Card Component
```razor
@* Educational Territory Card *@
<div class="territory-card @(IsOwned ? "owned" : "available")" @onclick="() => OnTerritoryClick(Territory)">
    <div class="territory-header">
        <img src="/images/flags/@(Territory.CountryCode.ToLower()).png" 
             alt="@Territory.CountryName flag" 
             class="territory-flag" />
        <h3 class="territory-name">@Territory.CountryName</h3>
    </div>
    
    <div class="territory-info">
        <div class="info-item">
            <span class="info-icon">💰</span>
            <span class="info-label">Cost:</span>
            <span class="info-value">$@Territory.Cost.ToString("N0")</span>
        </div>
        
        <div class="info-item">
            <span class="info-icon">⭐</span>
            <span class="info-label">Reputation:</span>
            <span class="info-value">@Territory.ReputationRequired%</span>
        </div>
        
        <div class="info-item">
            <span class="info-icon">🗣️</span>
            <span class="info-label">Languages:</span>
            <span class="info-value">@string.Join(", ", Territory.OfficialLanguages.Take(2))</span>
        </div>
    </div>
    
    @if (IsOwned)
    {
        <div class="owned-badge">
            <span class="text-2xl">🏆</span>
            <span class="font-bold">Owned!</span>
        </div>
    }
    else
    {
        <button class="purchase-button">
            @if (CanPurchase)
            {
                <span>Purchase Territory!</span>
            }
            else
            {
                <span>Need @(Territory.ReputationRequired - CurrentReputation)% more reputation</span>
            }
        </button>
    }
</div>

<style>
.territory-card {
    @apply bg-white rounded-2xl shadow-xl p-6;
    @apply border-4 border-transparent;
    @apply transform transition-all duration-300;
    @apply cursor-pointer;
}

.territory-card:hover {
    @apply scale-105 shadow-2xl;
}

.territory-card.available {
    @apply border-blue-200 hover:border-blue-400;
}

.territory-card.owned {
    @apply border-green-200 bg-green-50 hover:border-green-400;
}

.territory-header {
    @apply flex items-center space-x-3 mb-4;
}

.territory-flag {
    @apply w-12 h-8 rounded border-2 border-gray-200 object-cover;
}

.territory-name {
    @apply text-xl font-bold text-gray-800;
}

.territory-info {
    @apply space-y-2 mb-4;
}

.info-item {
    @apply flex items-center space-x-2;
}

.info-icon {
    @apply text-lg;
}

.info-label {
    @apply font-medium text-gray-600 min-w-[80px];
}

.info-value {
    @apply font-bold text-gray-800;
}

.owned-badge {
    @apply bg-green-100 border-2 border-green-200 rounded-lg p-2;
    @apply flex items-center justify-center space-x-2;
}

.purchase-button {
    @apply w-full py-3 px-4 rounded-lg font-bold;
    @apply transition-all duration-300;
}

.purchase-button:enabled {
    @apply bg-gradient-to-r from-green-500 to-blue-500 text-white;
    @apply hover:from-green-600 hover:to-blue-600;
    @apply shadow-lg hover:shadow-xl;
}

.purchase-button:disabled {
    @apply bg-gray-200 text-gray-500 cursor-not-allowed;
}
</style>
```

## ♿ Accessibility Standards

### WCAG 2.1 AA Compliance
```css
/* Accessibility-First Design */
.accessible-focus {
    @apply focus:outline-none focus:ring-4 focus:ring-blue-300;
    @apply focus:ring-opacity-50;
}

.high-contrast-text {
    /* Minimum 4.5:1 contrast ratio for normal text */
    color: #1F2937; /* Dark gray on white background */
}

.large-touch-target {
    /* Minimum 44px for touch accessibility */
    @apply min-h-[44px] min-w-[44px];
}

/* Screen reader friendly content */
.sr-only {
    @apply absolute w-px h-px p-0 -m-px overflow-hidden whitespace-nowrap;
    @apply border-0;
    clip: rect(0, 0, 0, 0);
}
```

### Semantic HTML Patterns
```razor
@* Proper semantic structure for screen readers *@
<main role="main" aria-label="World Leaders Game">
    <section aria-labelledby="game-status">
        <h2 id="game-status">Current Game Status</h2>
        <!-- Game status content -->
    </section>
    
    <section aria-labelledby="available-actions">
        <h2 id="available-actions">Available Actions</h2>
        <nav aria-label="Game actions">
            <button type="button" 
                    aria-describedby="dice-description"
                    @onclick="RollDice">
                Roll Career Dice
            </button>
            <p id="dice-description" class="sr-only">
                Roll the dice to determine your next career opportunity
            </p>
        </nav>
    </section>
</main>
```

### Keyboard Navigation
```csharp
// Blazor component with keyboard accessibility
@code {
    private async Task HandleKeyDown(KeyboardEventArgs e)
    {
        switch (e.Key)
        {
            case "Enter":
            case " ": // Spacebar
                await PerformPrimaryAction();
                break;
            case "Escape":
                await CancelAction();
                break;
            case "ArrowLeft":
                await NavigatePrevious();
                break;
            case "ArrowRight":
                await NavigateNext();
                break;
        }
    }
}

<div @onkeydown="HandleKeyDown" tabindex="0" class="accessible-focus">
    <!-- Component content -->
</div>
```

## 📱 Responsive Design Patterns

### Mobile-First Breakpoints
```css
/* Tailwind CSS responsive design for children */
.responsive-container {
    @apply px-4 py-6;          /* Mobile: 16px padding */
    @apply sm:px-6 sm:py-8;    /* Small: 24px padding */
    @apply md:px-8 md:py-10;   /* Medium: 32px padding */
    @apply lg:px-12 lg:py-12;  /* Large: 48px padding */
}

.responsive-grid {
    @apply grid gap-4;                    /* Mobile: single column */
    @apply sm:grid-cols-2 sm:gap-6;       /* Small: 2 columns */
    @apply lg:grid-cols-3 lg:gap-8;       /* Large: 3 columns */
}

.responsive-text {
    @apply text-lg;              /* Mobile: 18px */
    @apply sm:text-xl;           /* Small: 20px */
    @apply md:text-2xl;          /* Medium: 24px */
}
```

### Touch-Optimized Interactions
```css
/* Touch-friendly design for tablets */
.touch-optimized {
    @apply min-h-[56px] min-w-[56px];    /* Large touch targets */
    @apply p-4;                          /* Generous padding */
    @apply select-none;                  /* Prevent text selection */
    @apply touch-manipulation;           /* Optimize touch response */
}

.gesture-friendly {
    /* Swipe and pinch-friendly spacing */
    @apply space-y-6 space-x-6;
    /* Prevent accidental zoom */
    touch-action: manipulation;
}
```

## 🎉 Animation & Feedback

### Encouraging Animations
```css
/* Success and achievement animations */
@keyframes celebrate {
    0% { transform: scale(1) rotate(0deg); }
    25% { transform: scale(1.1) rotate(5deg); }
    50% { transform: scale(1.2) rotate(-5deg); }
    75% { transform: scale(1.1) rotate(3deg); }
    100% { transform: scale(1) rotate(0deg); }
}

@keyframes sparkle {
    0%, 100% { opacity: 0; transform: scale(0.8); }
    50% { opacity: 1; transform: scale(1.2); }
}

@keyframes countUp {
    0% { transform: translateY(20px); opacity: 0; }
    100% { transform: translateY(0); opacity: 1; }
}

.achievement-celebration {
    animation: celebrate 1000ms ease-out;
}

.resource-update {
    animation: countUp 500ms ease-in;
}

.success-sparkle {
    animation: sparkle 2000ms ease-in-out infinite;
}
```

### Loading States
```razor
@* Child-friendly loading component *@
<div class="loading-container">
    <div class="loading-spinner">
        <div class="spinner-ring"></div>
        <div class="spinner-center">🌍</div>
    </div>
    <p class="loading-text">@LoadingMessage</p>
</div>

<style>
.loading-container {
    @apply flex flex-col items-center justify-center p-8;
}

.loading-spinner {
    @apply relative w-16 h-16 mb-4;
}

.spinner-ring {
    @apply absolute inset-0 border-4 border-blue-200 border-t-blue-500 rounded-full;
    animation: spin 1s linear infinite;
}

.spinner-center {
    @apply absolute inset-0 flex items-center justify-center text-2xl;
}

.loading-text {
    @apply text-lg font-medium text-gray-600 animate-pulse;
}

@keyframes spin {
    to { transform: rotate(360deg); }
}
</style>

@code {
    [Parameter] public string LoadingMessage { get; set; } = "Loading your adventure...";
}
```

## 🧪 UI Testing Guidelines

### Component Testing Pattern
```csharp
[TestClass]
public class DiceComponentTests
{
    [TestMethod]
    public void DiceComponent_RendersCorrectly()
    {
        // Arrange
        using var ctx = new TestContext();
        
        // Act
        var component = ctx.RenderComponent<DiceComponent>();
        
        // Assert
        Assert.IsTrue(component.Find(".dice-container") != null);
        Assert.IsTrue(component.Find(".dice-instruction") != null);
        Assert.AreEqual("Click to roll!", component.Find(".dice-instruction p").TextContent);
    }
    
    [TestMethod]
    public void DiceComponent_IsAccessible()
    {
        // Arrange
        using var ctx = new TestContext();
        
        // Act
        var component = ctx.RenderComponent<DiceComponent>();
        
        // Assert
        var button = component.Find("button, [role='button']");
        Assert.IsTrue(button.HasAttribute("aria-label") || button.HasAttribute("aria-describedby"));
        Assert.IsTrue(button.HasAttribute("tabindex") || button.TagName == "BUTTON");
    }
}
```

## 📚 Cross-Module Relationships

### This Module Connects To:
- **[core-principles.md](./core-principles.md)**: Child safety and age-appropriate design
- **[educational-game-development.md](./educational-game-development.md)**: Game-specific UI requirements
- **[technical-architecture.md](./technical-architecture.md)**: Blazor component implementation
- **[ai-safety-and-child-protection.md](./ai-safety-and-child-protection.md)**: Safe content display

### Design Pattern:
```
Core Principles (child safety, age-appropriate)
↓
UI/UX Guidelines (this module)
↓
+ Educational Game Development + Technical Architecture
↓
= Engaging, safe, accessible educational interface
```

---

**Remember**: Every design decision should ask: "Does this help a 12-year-old learn more effectively while feeling encouraged and safe?"