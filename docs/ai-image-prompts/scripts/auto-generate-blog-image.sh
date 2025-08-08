#!/bin/bash

# Auto-Generate LinkedIn Images for Blog Posts
# Usage: ./auto-generate-blog-image.sh <blog-post-filename> [color-theme]
# Example: ./auto-generate-blog-image.sh "my-new-post" "electric-blue"

set -e

# Color definitions - using simple variables instead of associative arrays for macOS compatibility
get_color() {
    case "$1" in
        "electric-blue") echo "#3B82F6" ;;
        "royal-purple") echo "#8B5CF6" ;;
        "educational-green") echo "#10B981" ;;
        "warm-orange") echo "#F59E0B" ;;
        "pink") echo "#EC4899" ;;
        "ocean-blue") echo "#0EA5E9" ;;
        "gold") echo "#F59E0B" ;;
        "teal") echo "#14B8A6" ;;
        *) echo "#3B82F6" ;;  # Default to electric-blue
    esac
}

get_available_colors() {
    echo "electric-blue royal-purple educational-green warm-orange pink ocean-blue gold teal"
}

echo "🎨 Auto LinkedIn Image Generator for Blog Posts"
echo "=============================================="

# Check arguments
if [ $# -eq 0 ]; then
    echo "❌ Error: Blog post filename required"
    echo "Usage: $0 <blog-post-filename> [color-theme]"
    echo ""
    echo "Available color themes:"
    for color in $(get_available_colors); do
        echo "  - $color ($(get_color $color))"
    done
    echo ""
    echo "Examples:"
    echo "  $0 my-new-post electric-blue"
    echo "  $0 ai-development-tutorial"
    exit 1
fi

BLOG_POST_NAME=$1
COLOR_THEME=${2:-"electric-blue"}  # Default to electric-blue if not specified
COLOR_VALUE=$(get_color "$COLOR_THEME")

# Validate color theme
if [ "$COLOR_VALUE" = "#3B82F6" ] && [ "$COLOR_THEME" != "electric-blue" ]; then
    echo "❌ Error: Invalid color theme '$COLOR_THEME'"
    echo "Available themes: $(get_available_colors)"
    exit 1
fi

# Paths
DOCS_DIR="/Users/victorsaly/Documents/StormDev/ConquerTheWorldGame/docs"
BLOG_POST_PATH="$DOCS_DIR/_posts"
PROMPT_DIR="$DOCS_DIR/ai-image-prompts/blog-post-prompts"
SCRIPTS_DIR="$DOCS_DIR/ai-image-prompts/scripts"
OUTPUT_DIR="$DOCS_DIR/ai-image-prompts/output"
ASSETS_DIR="$DOCS_DIR/assets/linkedin-images"

# Find the blog post file
BLOG_POST_FILE=""
for file in "$BLOG_POST_PATH"/*.md; do
    if [[ "$(basename "$file")" == *"$BLOG_POST_NAME"* ]]; then
        BLOG_POST_FILE="$file"
        break
    fi
done

if [ -z "$BLOG_POST_FILE" ]; then
    echo "❌ Error: Blog post not found matching '$BLOG_POST_NAME'"
    echo "Available blog posts:"
    ls -1 "$BLOG_POST_PATH"/*.md | sed 's|.*/||; s|\.md$||' | sed 's/^/  - /'
    exit 1
fi

BLOG_POST_BASENAME=$(basename "$BLOG_POST_FILE" .md)
echo "📖 Found blog post: $BLOG_POST_BASENAME"

# Step 1: Extract blog post metadata and content
echo ""
echo "📝 Step 1: Analyzing blog post content..."

# Extract title, excerpt, and content
TITLE=$(grep "^title:" "$BLOG_POST_FILE" | sed 's/title: *"\(.*\)"/\1/' | sed 's/title: *\(.*\)/\1/')
EXCERPT=$(grep "^excerpt:" "$BLOG_POST_FILE" | sed 's/excerpt: *"\(.*\)"/\1/' | sed 's/excerpt: *\(.*\)/\1/')
TAGS=$(grep "^tags:" "$BLOG_POST_FILE" -A 10 | grep -E "^\s*-" | sed 's/.*- *"\(.*\)"/\1/' | tr '\n' ', ' | sed 's/, $//')

echo "  📌 Title: $TITLE"
echo "  📝 Excerpt: $EXCERPT"
echo "  🏷️  Tags: $TAGS"

# Step 2: Generate AI prompt
echo ""
echo "🤖 Step 2: Generating AI image prompt..."

PROMPT_FILE="$PROMPT_DIR/$BLOG_POST_BASENAME.md"

cat > "$PROMPT_FILE" << EOF
# 🎨 $TITLE - LinkedIn Image Prompt

## 📝 Azure OpenAI DALL-E 3 Optimized Prompt

Create a professional LinkedIn article image with a modern, vibrant educational technology design.

VISUAL STYLE REQUIREMENTS:
- Modern professional design with vibrant colors and gradients
- Primary color scheme: $COLOR_THEME $COLOR_VALUE with complementary colors
- Clean, readable typography for any text elements
- Professional LinkedIn article header format (1792x1024px)
- Modern gradient backgrounds and sophisticated color combinations
- High contrast for excellent readability

CONSISTENT ELEMENTS TO INCLUDE:
- Modern cloud infrastructure symbols (servers, databases, deployment pipelines)
- DevOps and deployment indicators (blue/green environments, arrows, switches)
- Educational technology elements (digital classrooms, learning interfaces, student devices)
- Professional development symbols (code, APIs, monitoring dashboards)
- Network and connectivity patterns (flowing data, seamless connections)

MOOD & ATMOSPHERE:
- Professional and modern technology aesthetic
- Innovation-focused with vibrant energy
- Educational and inspiring with dynamic visuals
- Modern technology with sophisticated design elements
- Collaborative and cutting-edge technology showcase

TEXT PLACEMENT:
- Leave space for article title overlay (top third of image)
- Ensure text readability against background
- Consider LinkedIn mobile and desktop viewing

BRAND CONSISTENCY:
- World Leaders Game project branding
- AI-first development methodology visual representation
- Educational gaming for children theme
- Father-son development partnership narrative

SPECIFIC CONTEXT FOR THIS IMAGE:
$EXCERPT

ARTICLE FOCUS:
$TITLE - Focus on the main themes and technical concepts discussed in this educational technology blog post.

KEY VISUAL METAPHORS:
- Educational technology innovation and development
- AI-assisted learning and development workflows
- Child-friendly educational gaming environments
- Professional development with educational impact
- Technical excellence in service of education

TECHNICAL ELEMENTS TO HIGHLIGHT:
- Educational technology symbols and interfaces
- AI development workflow indicators
- Child-safe educational platform elements
- Professional development tools and processes
- Modern educational technology stack

TARGET AUDIENCE: Educational Technology Directors, Developers, Teachers, AI Enthusiasts

COLOR SCHEME: Vibrant $COLOR_THEME $COLOR_VALUE with modern gradients and complementary colors
EOF

echo "  ✅ Prompt generated: $PROMPT_FILE"

# Step 3: Generate the image
echo ""
echo "🎨 Step 3: Generating LinkedIn image with OpenAI DALL-E 3..."

cd "$SCRIPTS_DIR"
./generate.sh "$BLOG_POST_BASENAME"

if [ $? -ne 0 ]; then
    echo "❌ Image generation failed"
    exit 1
fi

# Step 4: Move image to assets directory
echo ""
echo "📁 Step 4: Moving image to assets directory..."

SOURCE_IMAGE="$OUTPUT_DIR/$BLOG_POST_BASENAME-linkedin.png"
TARGET_IMAGE="$ASSETS_DIR/$BLOG_POST_BASENAME-linkedin.png"

if [ -f "$SOURCE_IMAGE" ]; then
    mv "$SOURCE_IMAGE" "$TARGET_IMAGE"
    echo "  ✅ Image moved to: $TARGET_IMAGE"
else
    echo "❌ Source image not found: $SOURCE_IMAGE"
    exit 1
fi

# Step 5: Update blog post with image reference
echo ""
echo "📝 Step 5: Updating blog post with image reference..."

# Create a temporary file for the updated content
TEMP_FILE=$(mktemp)

# Check if image field already exists
if grep -q "^image:" "$BLOG_POST_FILE"; then
    echo "  🔄 Updating existing image field..."
    # Replace existing image field
    sed "s|^image:.*|image:|" "$BLOG_POST_FILE" > "$TEMP_FILE"
    sed -i '' "/^image:$/a\\
  path: /assets/linkedin-images/$BLOG_POST_BASENAME-linkedin.png\\
  alt: \"Professional LinkedIn image - $TITLE\"" "$TEMP_FILE"
else
    echo "  ➕ Adding new image field..."
    # Add image field after excerpt
    sed "/^excerpt:/a\\
image:\\
  path: /assets/linkedin-images/$BLOG_POST_BASENAME-linkedin.png\\
  alt: \"Professional LinkedIn image - $TITLE\"" "$BLOG_POST_FILE" > "$TEMP_FILE"
fi

# Replace the original file
mv "$TEMP_FILE" "$BLOG_POST_FILE"

echo "  ✅ Blog post updated with image reference"

# Step 6: Summary
echo ""
echo "🎉 SUCCESS! LinkedIn image generation complete!"
echo "=============================================="
echo "📄 Blog post: $BLOG_POST_BASENAME"
echo "🎨 Color theme: $COLOR_THEME ($COLOR_VALUE)"
echo "🖼️  Image: /assets/linkedin-images/$BLOG_POST_BASENAME-linkedin.png"
echo "📝 Prompt: $PROMPT_FILE"
echo "💰 Cost: ~$0.08"
echo ""
echo "📊 Ready for:"
echo "  - LinkedIn sharing"
echo "  - Medium/dev.to publication"
echo "  - Blog display"
echo ""

# Display file size
if [ -f "$TARGET_IMAGE" ]; then
    FILE_SIZE=$(du -h "$TARGET_IMAGE" | cut -f1)
    echo "📐 Image size: $FILE_SIZE"
fi

echo "🚀 Next steps:"
echo "  1. Review the generated image"
echo "  2. Test the blog post display"
echo "  3. Share on social media!"
echo ""
echo "📚 Documentation:"
echo "  - Quick Guide: docs/_technical/quick-blog-image-generation.md"
echo "  - Full Guide: docs/_technical/auto-blog-image-generation.md"

# Optional: Open the image for preview (macOS)
if command -v open >/dev/null 2>&1; then
    echo ""
    read -p "🔍 Open image for preview? (y/n): " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        open "$TARGET_IMAGE"
    fi
fi
