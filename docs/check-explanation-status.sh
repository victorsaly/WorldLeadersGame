#!/bin/bash

# Simple helper script to show remaining blog posts that need explanations

echo "📝 Blog Posts Status:"
echo "===================="
echo ""

# Count total posts
total_posts=$(find docs/_posts -name "*.md" | wc -l | tr -d ' ')
echo "Total blog posts: $total_posts"

# Count posts with explanations
posts_with_explanations=$(grep -l "code-explanation" docs/_posts/*.md | wc -l | tr -d ' ')
echo "Posts with explanations: $posts_with_explanations"

echo "Remaining to update: $((total_posts - posts_with_explanations))"
echo ""

echo "📂 Posts WITHOUT explanations:"
echo "=============================="

# Find posts without explanations
for file in docs/_posts/*.md; do
    if ! grep -q "code-explanation" "$file"; then
        basename "$file"
    fi
done

echo ""
echo "✅ Posts WITH explanations:"
echo "=========================="

# Find posts with explanations
for file in docs/_posts/*.md; do
    if grep -q "code-explanation" "$file"; then
        basename "$file"
    fi
done

echo ""
echo "💡 Next steps:"
echo "1. Open any file from the 'WITHOUT explanations' list"
echo "2. Find code blocks (starting with three backticks)"
echo "3. Add explanation using the template in add-explanation-template.md"
echo "4. Place explanation immediately BEFORE the code block"
echo ""
echo "Template location: docs/add-explanation-template.md"
