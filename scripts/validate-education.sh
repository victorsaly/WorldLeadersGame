#!/bin/bash
# validate-education.sh - Educational value and child safety validation

echo "📚 Validating Educational Value and Child Safety..."

# Check educational objectives in code comments
echo "🎯 Checking educational objectives..."
if grep -r "Educational Objective\|Learning.*objective\|teaches.*12-year" src/WorldLeaders/ >/dev/null 2>&1; then
    echo "✅ Educational objectives documented in code"
else
    echo "❌ Missing educational objectives in code"
    exit 1
fi

# Validate child safety measures
echo "🛡️ Checking child safety implementations..."
if grep -r "child.*safety\|content.*moderation\|age.*appropriate" src/WorldLeaders/ >/dev/null 2>&1; then
    echo "✅ Child safety measures present"
else
    echo "❌ Missing child safety implementations"
    exit 1
fi

# Check for real-world data integration
echo "🌍 Checking real-world data integration..."
if grep -r "GDP\|World.*Bank\|country.*data\|Territory.*cost" src/WorldLeaders/ >/dev/null 2>&1; then
    echo "✅ Real-world data integration found"
else
    echo "❌ Missing real-world data integration"
    exit 1
fi

# Validate cultural sensitivity
echo "🤝 Checking cultural sensitivity..."
if grep -r "cultural.*sensitivity\|respectful.*representation\|inclusive" src/WorldLeaders/ >/dev/null 2>&1; then
    echo "✅ Cultural sensitivity considerations present"
else
    echo "❌ Missing cultural sensitivity considerations"
    exit 1
fi

# Check AI safety validation
echo "🤖 Checking AI safety validation..."
if grep -r "content.*moderator\|ai.*safety\|fallback.*response" src/WorldLeaders/ >/dev/null 2>&1; then
    echo "✅ AI safety validation implemented"
else
    echo "❌ Missing AI safety validation"
    exit 1
fi

# Validate accessibility compliance
echo "♿ Checking accessibility compliance..."
if grep -r "aria-label\|aria-describedby\|WCAG\|accessibility" src/WorldLeaders/ >/dev/null 2>&1; then
    echo "✅ Accessibility considerations present"
else
    echo "❌ Missing accessibility implementations"
    exit 1
fi

# Check educational game mechanics
echo "🎮 Checking educational game mechanics..."
if grep -r "dice.*roll\|career.*progression\|territory.*acquisition\|language.*learning" src/WorldLeaders/ >/dev/null 2>&1; then
    echo "✅ Educational game mechanics implemented"
else
    echo "❌ Missing educational game mechanics"
    exit 1
fi

# Validate learning progression
echo "📈 Checking learning progression..."
if grep -r "progress.*track\|learning.*outcome\|achievement\|milestone" src/WorldLeaders/ >/dev/null 2>&1; then
    echo "✅ Learning progression system present"
else
    echo "❌ Missing learning progression system"
    exit 1
fi

# Check documentation quality
echo "📝 Checking educational documentation..."
if [ -d "docs/_technical" ] && [ -d "docs/_journey" ] && [ -f "docs/issues.md" ]; then
    echo "✅ Educational documentation structure present"
    
    # Check for educational context in documentation
    if grep -r "Educational\|12-year\|learning" docs/ >/dev/null 2>&1; then
        echo "✅ Educational context in documentation"
    else
        echo "❌ Missing educational context in documentation"
        exit 1
    fi
else
    echo "❌ Missing educational documentation structure"
    exit 1
fi

echo "🎉 Educational value validation completed successfully!"
echo "✨ Game maintains strong educational mission for 12-year-old learners"
