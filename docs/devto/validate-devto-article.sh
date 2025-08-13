#!/bin/bash

# 🔍 Validate Dev.to Article
# 
# This script validates a dev.to article for deployment readiness:
# - Frontmatter compliance
# - Image accessibility  
# - Link validation
# - Format compatibility
# - Content optimization
#
# Usage: ./validate-devto-article.sh <article-file>
# Example: ./validate-devto-article.sh devto/articles/2025-08-02-my-article.md

# set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Counters
ERRORS=0
WARNINGS=0
INFO=0

# Helper functions
log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
    ((INFO++))
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
    ((WARNINGS++))
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
    ((ERRORS++))
}

# Check if file argument provided
if [ $# -eq 0 ]; then
    log_error "No file specified"
    echo "Usage: $0 <article-file>"
    echo "Example: $0 devto/articles/2025-08-02-my-article.md"
    exit 1
fi

ARTICLE_FILE="$1"

# Validate input file exists
if [ ! -f "$ARTICLE_FILE" ]; then
    log_error "File not found: $ARTICLE_FILE"
    exit 1
fi

log_info "Validating dev.to article: $ARTICLE_FILE"
echo ""

# Validation 1: Frontmatter
log_info "1. Validating frontmatter..."

if ! head -1 "$ARTICLE_FILE" | grep -q "^---$"; then
    log_error "Missing frontmatter start delimiter"
else
    log_success "Frontmatter delimiter found"
fi

# Check required frontmatter fields
required_fields=("title" "published" "description" "tags")
for field in "${required_fields[@]}"; do
    if grep -q "^${field}:" "$ARTICLE_FILE"; then
        log_success "Required field: $field"
    else
        log_error "Missing required field: $field"
    fi
done

# Check published status
if grep -q "published: true" "$ARTICLE_FILE"; then
    log_warning "Article is set to publish immediately"
elif grep -q "published: false" "$ARTICLE_FILE"; then
    log_success "Article is in draft mode"
else
    log_error "Published status not specified"
fi

# Check tag count
tag_line=$(grep "^tags:" "$ARTICLE_FILE" || echo "")
if [ -n "$tag_line" ]; then
    tag_count=$(echo "$tag_line" | sed 's/tags: //' | tr ',' '\n' | wc -l | tr -d ' ')
    if [ "$tag_count" -le 4 ]; then
        log_success "Tag count: $tag_count (within limit)"
    else
        log_warning "Tag count: $tag_count (exceeds recommended 4)"
    fi
fi

echo ""

# Validation 2: Images
log_info "2. Validating images..."

# Check for image URLs
image_count=$(grep -c "!\[.*\](" "$ARTICLE_FILE" || echo "0")
log_info "Found $image_count images"

# Check image domains
if grep -q "!\[.*\](https://docs.worldleadersgame.co.uk" "$ARTICLE_FILE"; then
    log_success "Images use correct domain"
elif grep -q "!\[.*\](/" "$ARTICLE_FILE"; then
    log_error "Relative image URLs found - need full URLs"
else
    log_info "No relative image URLs detected"
fi

# Check cover image
if grep -q "cover_image:" "$ARTICLE_FILE"; then
    cover_url=$(grep "cover_image:" "$ARTICLE_FILE" | sed 's/cover_image: //')
    if [[ "$cover_url" == https://docs.worldleadersgame.co.uk* ]]; then
        log_success "Cover image uses correct domain"
    else
        log_warning "Cover image domain should be docs.worldleadersgame.co.uk"
    fi
else
    log_warning "No cover image specified"
fi

echo ""

# Validation 3: Links
log_info "3. Validating links..."

# Check for relative links
relative_links=$(grep -c "](/" "$ARTICLE_FILE" | grep -v "https://" || echo "0")
if [ "$relative_links" -eq 0 ]; then
    log_success "No relative links found"
else
    log_warning "$relative_links relative links found - should be absolute URLs"
fi

# Check for GitHub links
if grep -q "github.com/victorsaly/WorldLeadersGame" "$ARTICLE_FILE"; then
    log_success "GitHub repository links found"
fi

# Check canonical URL
if grep -q "canonical_url:" "$ARTICLE_FILE"; then
    canonical_url=$(grep "canonical_url:" "$ARTICLE_FILE" | sed 's/canonical_url: //')
    if [[ "$canonical_url" == https://docs.worldleadersgame.co.uk* ]]; then
        log_success "Canonical URL uses correct domain"
    else
        log_warning "Canonical URL should use docs.worldleadersgame.co.uk"
    fi
else
    log_error "Missing canonical URL"
fi

echo ""

# Validation 4: Format Compatibility
log_info "4. Checking format compatibility..."

# Check for Mermaid
if grep -q "mermaid" "$ARTICLE_FILE"; then
    log_error "Mermaid references found - not supported on dev.to"
else
    log_success "No Mermaid references found"
fi

# Check for Jekyll-specific syntax
if grep -q "{%" "$ARTICLE_FILE"; then
    log_warning "Jekyll liquid tags found - may not work on dev.to"
else
    log_success "No Jekyll-specific syntax found"
fi

# Check for proper code blocks
code_blocks=$(grep -c "^\`\`\`" "$ARTICLE_FILE" || echo "0")
if [ $((code_blocks % 2)) -eq 0 ]; then
    log_success "Code blocks properly closed"
else
    log_error "Unmatched code blocks detected"
fi

echo ""

# Validation 5: Content Optimization
log_info "5. Checking content optimization..."

# Check for TL;DR
if grep -q "TL;DR" "$ARTICLE_FILE"; then
    log_success "TL;DR section found"
else
    log_warning "No TL;DR section - consider adding for engagement"
fi

# Check for discussion questions
if grep -q "Discussion Questions" "$ARTICLE_FILE"; then
    log_success "Discussion questions found"
else
    log_warning "No discussion questions - consider adding for engagement"
fi

# Check for call-to-action
if grep -q "Follow me\|Subscribe\|Learn More" "$ARTICLE_FILE"; then
    log_success "Call-to-action found"
else
    log_warning "No clear call-to-action found"
fi

# Check article length
word_count=$(wc -w < "$ARTICLE_FILE")
if [ "$word_count" -ge 1000 ]; then
    log_success "Article length: $word_count words (good for SEO)"
elif [ "$word_count" -ge 500 ]; then
    log_info "Article length: $word_count words (acceptable)"
else
    log_warning "Article length: $word_count words (consider expanding)"
fi

echo ""

# Summary
log_info "=== VALIDATION SUMMARY ==="

if [ $ERRORS -eq 0 ] && [ $WARNINGS -eq 0 ]; then
    log_success "Perfect! No issues found - ready for publication ✨"
    echo ""
    echo "🚀 Ready to publish:"
    echo "   1. Set 'published: true' in frontmatter"
    echo "   2. Copy content to dev.to editor" 
    echo "   3. Preview and publish"
    exit 0
elif [ $ERRORS -eq 0 ]; then
    log_warning "Article ready with $WARNINGS warning(s) - review recommended"
    echo ""
    echo "📝 Recommended actions:"
    echo "   1. Review warnings above"
    echo "   2. Consider improvements"
    echo "   3. Set 'published: true' when ready"
    exit 0
else
    log_error "Article has $ERRORS error(s) and $WARNINGS warning(s) - fixes required"
    echo ""
    echo "🔧 Required actions:"
    echo "   1. Fix all errors listed above"
    echo "   2. Address warnings if possible"
    echo "   3. Re-run validation"
    echo "   4. Set 'published: true' when ready"
    exit 1
fi
