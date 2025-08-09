#!/bin/bash
# validate-retro.sh - Retro 32-bit design compliance validation

echo "🎨 Validating Retro 32-Bit Design Standards..."

# Check green theme implementation
echo "🟢 Checking green theme implementation..."
if grep -r "retro-green\|#22c55e\|#16a34a" src/WorldLeaders/WorldLeaders.Web/ >/dev/null 2>&1; then
    echo "✅ Green theme colors implemented"
else
    echo "❌ Missing green theme implementation"
    exit 1
fi

# Verify pixel art styling
echo "🖼️ Checking pixel art rendering..."
if grep -r "image-rendering: pixelated\|image-rendering: crisp-edges" src/WorldLeaders/WorldLeaders.Web/ >/dev/null 2>&1; then
    echo "✅ Pixel art rendering enabled"
else
    echo "❌ Missing pixel art rendering"
    exit 1
fi

# Check retro typography
echo "🔤 Checking retro typography..."
if grep -r "Press Start 2P\|Orbitron" src/WorldLeaders/WorldLeaders.Web/ >/dev/null 2>&1; then
    echo "✅ Retro fonts implemented"
else
    echo "❌ Missing retro typography"
    exit 1
fi

# Validate retro button styling
echo "🔘 Checking retro button components..."
if grep -r "retro-button\|box-shadow.*px.*px.*0" src/WorldLeaders/WorldLeaders.Web/ >/dev/null 2>&1; then
    echo "✅ Retro button styling present"
else
    echo "❌ Missing retro button styling"
    exit 1
fi

# Check for character persona system
echo "👤 Checking character persona system..."
if grep -r "character.*persona\|character.*selection" src/WorldLeaders/WorldLeaders.Web/ >/dev/null 2>&1; then
    echo "✅ Character persona system detected"
else
    echo "⚠️ Character persona system not yet implemented (planned for Week 6)"
fi

# Validate mobile-friendly retro design
echo "📱 Checking mobile retro optimization..."
if grep -r "@media.*max-width.*768px\|touch-target\|min-h-\[44px\]" src/WorldLeaders/WorldLeaders.Web/ >/dev/null 2>&1; then
    echo "✅ Mobile retro optimization present"
else
    echo "❌ Missing mobile retro optimization"
    exit 1
fi

# Check for retro animations
echo "✨ Checking retro animations..."
if grep -r "@keyframes.*retro\|animation.*retro" src/WorldLeaders/WorldLeaders.Web/ >/dev/null 2>&1; then
    echo "✅ Retro animations implemented"
else
    echo "⚠️ Retro animations not yet implemented (planned for Week 6)"
fi

echo "🎉 Retro design validation completed!"
echo "📝 Note: Some features may be planned for Week 6 implementation"
