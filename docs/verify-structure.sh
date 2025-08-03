#!/bin/bash

# Documentation Structure Verification Script
# Ensures the docs directory maintains proper organization

echo "🔍 Documentation Structure Verification"
echo "======================================"
echo ""

cd "$(dirname "$0")"

# Check for required collections
echo "📁 Checking Jekyll collections..."
for collection in "_posts" "_journey" "_technical" "_milestones"; do
    if [ -d "$collection" ]; then
        count=$(find "$collection" -name "*.md" | wc -l)
        echo "✅ $collection ($count files)"
    else
        echo "❌ Missing: $collection"
    fi
done

echo ""
echo "📄 Checking root navigation files..."
required_files=("index.md" "blog.md" "journey.md" "technical-docs.md" "about.md" "LOCAL-TESTING.md")
for file in "${required_files[@]}"; do
    if [ -f "$file" ]; then
        echo "✅ $file"
    else
        echo "❌ Missing: $file"
    fi
done

echo ""
echo "🔧 Checking test scripts..."
test_scripts=("simple-test.sh" "test-docker.sh")
for script in "${test_scripts[@]}"; do
    if [ -f "$script" ] && [ -x "$script" ]; then
        echo "✅ $script (executable)"
    elif [ -f "$script" ]; then
        echo "⚠️  $script (not executable)"
    else
        echo "❌ Missing: $script"
    fi
done

echo ""
echo "🚫 Checking for unwanted files..."
unwanted_patterns=("README*.md" "*summary*.md" "*SUMMARY*.md" "test-local.sh" "quick-test.sh")
found_unwanted=false
for pattern in "${unwanted_patterns[@]}"; do
    if ls $pattern 1> /dev/null 2>&1; then
        echo "❌ Found unwanted: $pattern"
        found_unwanted=true
    fi
done
if [ "$found_unwanted" = false ]; then
    echo "✅ No unwanted files found"
fi

echo ""
echo "📊 Structure Summary:"
echo "Collections: $(find . -name "_*" -type d | wc -l)"
echo "Total .md files: $(find . -name "*.md" | wc -l)"
echo "Test scripts: $(find . -name "*.sh" | wc -l)"

echo ""
if [ "$found_unwanted" = false ]; then
    echo "🎉 Documentation structure is clean and compliant!"
else
    echo "⚠️  Please fix the issues above to maintain clean structure."
fi
