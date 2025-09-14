#!/bin/bash

# OpenAI Direct API Image Generator - Shell Wrapper
# Alternative to Azure OpenAI when regional restrictions apply

set -e

echo "🎨 OpenAI Direct API - LinkedIn Image Generator"
echo "=============================================="

# Check if blog post name provided
if [ $# -eq 0 ]; then
    echo "❌ Error: Blog post name required"
    echo "Usage: ./generate-openai.sh <blog-post-name> [output-path]"
    echo ""
    echo "Available blog posts:"
    ls -1 ../blog-post-prompts/*.md 2>/dev/null | sed 's|.*/||; s|\.md$||' | sed 's/^/  - /'
    exit 1
fi

BLOG_POST=$1
OUTPUT_PATH=${2:-"../output/${BLOG_POST}-linkedin.png"}

# Check if prompt file exists
PROMPT_FILE="../blog-post-prompts/${BLOG_POST}.md"
if [ ! -f "$PROMPT_FILE" ]; then
    echo "❌ Error: Prompt file not found: $PROMPT_FILE"
    echo ""
    echo "Available blog posts:"
    ls -1 ../blog-post-prompts/*.md 2>/dev/null | sed 's|.*/||; s|\.md$||' | sed 's/^/  - /'
    exit 1
fi

# Check if .env file exists
if [ ! -f ".env" ]; then
    echo "❌ Error: .env file not found"
    echo "💡 Create .env file with:"
    echo "   OPENAI_API_KEY=your_openai_api_key_here"
    echo ""
    echo "🔑 Get your API key from: https://platform.openai.com/api-keys"
    exit 1
fi

# Check if OPENAI_API_KEY is set
if ! grep -q "OPENAI_API_KEY=" .env; then
    echo "❌ Error: OPENAI_API_KEY not found in .env file"
    echo "💡 Add to .env file:"
    echo "   OPENAI_API_KEY=your_openai_api_key_here"
    echo ""
    echo "🔑 Get your API key from: https://platform.openai.com/api-keys"
    exit 1
fi

# Create output directory
mkdir -p ../output

echo "📖 Blog post: $BLOG_POST"
echo "📁 Output: $OUTPUT_PATH"
echo "💰 Estimated cost: ~$0.08"
echo ""

# Run the Python script
/Users/victorsaly/Documents/StormDev/ConquerTheWorldGame/.venv/bin/python generate-image.py "$BLOG_POST" "$OUTPUT_PATH"

if [ $? -eq 0 ]; then
    echo ""
    echo "🎉 Success! LinkedIn image generated"
    echo "📄 Blog post: $BLOG_POST"
    echo "🖼️  Image: $OUTPUT_PATH"
    echo "📊 Ready for LinkedIn upload!"
    
    # Check if file was actually created
    if [ -f "$OUTPUT_PATH" ]; then
        FILE_SIZE=$(du -h "$OUTPUT_PATH" | cut -f1)
        echo "📐 File size: $FILE_SIZE"
        
        # Try to open the image (macOS)
        if command -v open >/dev/null 2>&1; then
            echo "🔍 Opening image preview..."
            open "$OUTPUT_PATH"
        fi
    fi
else
    echo "❌ Failed to generate image"
    exit 1
fi
