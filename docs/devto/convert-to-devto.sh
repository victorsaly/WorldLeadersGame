#!/bin/bash

# 📝 Convert Jekyll Post to Dev.to Format
# 
# This script converts a Jekyll blog post to dev.to format with:
# - Proper frontmatter formatting
# - Image URLs pointing to docs.worldleadersgame.co.uk
# - ASCII/Unicode diagrams instead of Mermaid
# - Dev.to-optimized content structure
#
# Usage: ./convert-to-devto.sh <jekyll-post-file>
# Example: ./convert-to-devto.sh _posts/2025-08-02-my-article.md

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
DOMAIN="docs.worldleadersgame.co.uk"
OUTPUT_DIR="devto/articles"
WORKING_DIR="devto/working"

# Helper functions
log_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

log_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

log_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

log_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Check if file argument provided
if [ $# -eq 0 ]; then
    log_error "No file specified"
    echo "Usage: $0 <jekyll-post-file>"
    echo "Example: $0 _posts/2025-08-02-my-article.md"
    exit 1
fi

INPUT_FILE="$1"

# Validate input file exists
if [ ! -f "$INPUT_FILE" ]; then
    log_error "File not found: $INPUT_FILE"
    exit 1
fi

# Extract filename and create output filename
FILENAME=$(basename "$INPUT_FILE")
BASE_NAME="${FILENAME%.*}"
OUTPUT_FILE="$OUTPUT_DIR/${BASE_NAME}.md"

# Create directories if they don't exist
mkdir -p "$OUTPUT_DIR"
mkdir -p "$WORKING_DIR"

log_info "Converting Jekyll post to dev.to format..."
log_info "Input:  $INPUT_FILE"
log_info "Output: $OUTPUT_FILE"

# Copy file to working directory
WORK_FILE="$WORKING_DIR/${FILENAME}"
cp "$INPUT_FILE" "$WORK_FILE"

log_info "Processing content..."

# Step 1: Convert frontmatter
log_info "1. Converting frontmatter..."

# Create temporary file for frontmatter conversion
TEMP_FILE=$(mktemp)

# Extract and convert frontmatter
cat > "$TEMP_FILE" << EOF
import sys
import re
import yaml
from datetime import datetime

# Read the input file
with open('$WORK_FILE', 'r') as f:
    content = f.read()

# Extract frontmatter
if content.startswith('---'):
    parts = content.split('---', 2)
    if len(parts) >= 3:
        frontmatter = parts[1].strip()
        body = parts[2].strip()
    else:
        frontmatter = ""
        body = content
else:
    frontmatter = ""
    body = content

# Parse existing frontmatter
try:
    fm_data = yaml.safe_load(frontmatter) if frontmatter else {}
except:
    fm_data = {}

# Extract title
title = fm_data.get('title', 'Untitled Article')

# Clean title for dev.to (remove quotes if present)
if title.startswith('"') and title.endswith('"'):
    title = title[1:-1]

# Create description from first paragraph or use existing
description = fm_data.get('description', '')
if not description and body:
    # Extract first meaningful paragraph
    lines = body.split('\n')
    for line in lines:
        line = line.strip()
        if line and not line.startswith('#') and not line.startswith('>') and len(line) > 50:
            # Clean markdown formatting
            description = re.sub(r'\[([^\]]+)\]\([^\)]+\)', r'\1', line)
            description = re.sub(r'\*\*([^*]+)\*\*', r'\1', description)
            description = re.sub(r'\*([^*]+)\*', r'\1', description)
            if len(description) > 160:
                description = description[:157] + '...'
            break

# Generate canonical URL from filename
filename = '$WORK_FILE'.split('/')[-1]
date_match = re.match(r'(\d{4}-\d{2}-\d{2})-(.*)', filename)
if date_match:
    date_str = date_match.group(1)
    slug = date_match.group(2).replace('.md', '').replace('-dev-to', '')
    canonical_url = f"https://docs.worldleadersgame.co.uk/post/{slug}/"
else:
    canonical_url = f"https://docs.worldleadersgame.co.uk/post/{filename.replace('.md', '')}/"

# Create dev.to frontmatter
devto_frontmatter = {
    'title': title,
    'published': False,
    'description': description,
    'tags': 'ai, education, gamedev, softwaredevelopment',
    'cover_image': f'https://docs.worldleadersgame.co.uk/assets/linkedin-images/{filename.replace(".md", "").replace("-dev-to", "")}-linkedin.png',
    'canonical_url': canonical_url
}

# Add series if present
if 'categories' in fm_data and 'AI-First' in str(fm_data['categories']):
    devto_frontmatter['series'] = 'AI-First Educational Game Development'

# Write new frontmatter and body
print('---')
for key, value in devto_frontmatter.items():
    if isinstance(value, bool):
        print(f'{key}: {str(value).lower()}')
    else:
        print(f'{key}: {value}')
print('---')
print()
print(body)
EOF

python3 "$TEMP_FILE" > "${WORK_FILE}.new"
mv "${WORK_FILE}.new" "$WORK_FILE"
rm "$TEMP_FILE"

log_success "Frontmatter converted"

# Step 2: Convert image URLs
log_info "2. Converting image URLs..."

# Replace relative image paths with full URLs
sed -i '' "s|/assets/images/|https://${DOMAIN}/assets/images/|g" "$WORK_FILE"
sed -i '' "s|![Image](images/|![Image](https://${DOMAIN}/assets/images/|g" "$WORK_FILE"
sed -i '' "s|](images/|](https://${DOMAIN}/assets/images/|g" "$WORK_FILE"

# Fix cover_image frontmatter if it has duplicate protocol
sed -i '' 's|cover_image: cover_image: |cover_image: |g' "$WORK_FILE"

log_success "Image URLs converted"

# Step 3: Convert Mermaid diagrams to ASCII
log_info "3. Converting diagrams to ASCII format..."

# This is a complex conversion - for now, we'll flag Mermaid blocks for manual conversion
if grep -q '```mermaid' "$WORK_FILE"; then
    log_warning "Mermaid diagrams detected - manual conversion required"
    log_info "Converting common Mermaid patterns to ASCII..."
    
    # Simple flowchart conversion
    sed -i '' 's/```mermaid/```/g' "$WORK_FILE"
    
    # Add comment for manual review
    sed -i '' '/^```$/,/^```$/ {
        /graph\|flowchart/ {
            i\
<!-- TODO: Convert this Mermaid diagram to ASCII format -->
        }
    }' "$WORK_FILE"
fi

log_success "Diagram conversion completed"

# Step 4: Fix internal links
log_info "4. Converting internal links to external URLs..."

# Convert Jekyll internal links to external URLs
sed -i '' "s|](/post/|](https://${DOMAIN}/post/|g" "$WORK_FILE"
sed -i '' "s|](/journey/|](https://${DOMAIN}/journey/|g" "$WORK_FILE"
sed -i '' "s|](/technical/|](https://${DOMAIN}/technical/|g" "$WORK_FILE"

# Fix domain references
sed -i '' 's|worldleadersgame\.co\.uk|docs.worldleadersgame.co.uk|g' "$WORK_FILE"

log_success "Internal links converted"

# Step 5: Add dev.to optimizations
log_info "5. Adding dev.to optimizations..."

# Add TL;DR if not present
if ! grep -q "TL;DR" "$WORK_FILE"; then
    # Insert TL;DR after first paragraph
    sed -i '' '1,/^$/s/^$/>/' "$WORK_FILE"
    sed -i '' 's|^>|> **TL;DR**: [Add compelling summary here]|' "$WORK_FILE"
fi

# Ensure discussion questions section exists
if ! grep -q "Discussion Questions" "$WORK_FILE"; then
    cat >> "$WORK_FILE" << 'EOQ'

---

## 💭 Discussion Questions

I'm curious about your experience with the topics covered:

1. **What's your experience with [specific topic]?**
2. **Have you tried [specific approach/technique]?**
3. **What challenges have you encountered?**
4. **How do you balance [competing concerns]?**

Share your thoughts and experiences in the comments below! 👇
EOQ
fi

log_success "Dev.to optimizations added"

# Step 6: Move to output directory
log_info "6. Finalizing output..."

mv "$WORK_FILE" "$OUTPUT_FILE"

log_success "Conversion completed!"
log_info "Output file: $OUTPUT_FILE"

# Step 7: Validation
log_info "7. Running validation..."

# Check for common issues
ISSUES=0

# Check frontmatter
if ! grep -q "published: false" "$OUTPUT_FILE"; then
    log_warning "Consider setting 'published: false' for drafts"
    ((ISSUES++))
fi

# Check for Mermaid
if grep -q "mermaid" "$OUTPUT_FILE"; then
    log_warning "Mermaid references found - manual diagram conversion needed"
    ((ISSUES++))
fi

# Check for relative URLs
if grep -q "](/" "$OUTPUT_FILE" && ! grep -q "https://" "$OUTPUT_FILE"; then
    log_warning "Potential relative URLs found - review links"
    ((ISSUES++))
fi

# Check for cover image
if ! grep -q "cover_image:" "$OUTPUT_FILE"; then
    log_warning "No cover image specified"
    ((ISSUES++))
fi

if [ $ISSUES -eq 0 ]; then
    log_success "No issues detected - ready for review!"
else
    log_warning "$ISSUES issue(s) detected - review needed"
fi

echo ""
log_info "Next steps:"
echo "1. Review and edit: $OUTPUT_FILE"
echo "2. Manual diagram conversion if needed"
echo "3. Validate with: ./devto/validate-devto-article.sh $OUTPUT_FILE"
echo "4. Set 'published: true' when ready to deploy"

echo ""
log_info "Dev.to publishing checklist:"
echo "- [ ] Review frontmatter (title, description, tags)"
echo "- [ ] Verify all images load correctly"
echo "- [ ] Convert any remaining Mermaid diagrams"
echo "- [ ] Test all external links"
echo "- [ ] Review discussion questions"
echo "- [ ] Set published: true"
