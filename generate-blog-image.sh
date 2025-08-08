#!/bin/bash

# Quick Blog Image Generator
# Simple wrapper script for the main auto-generation script
# Place this in your PATH or run from project root

SCRIPT_DIR="/Users/victorsaly/Documents/StormDev/ConquerTheWorldGame/docs/ai-image-prompts/scripts"

echo "🎨 Quick Blog Image Generator"
echo "============================="

if [ $# -eq 0 ]; then
    echo ""
    echo "Usage: $0 <blog-post-name> [color-theme]"
    echo ""
    echo "🎯 This will:"
    echo "  1. Find your blog post"
    echo "  2. Generate AI prompt based on content"
    echo "  3. Create LinkedIn image with DALL-E 3"
    echo "  4. Update blog post with image reference"
    echo ""
    echo "🎨 Available color themes:"
    echo "  - electric-blue (default)"
    echo "  - royal-purple"
    echo "  - educational-green"
    echo "  - warm-orange"
    echo "  - pink"
    echo "  - ocean-blue"
    echo "  - gold"
    echo "  - teal"
    echo ""
    echo "📝 Examples:"
    echo "  $0 child-safe-authentication electric-blue"
    echo "  $0 ai-development royal-purple"
    echo "  $0 my-new-post"
    echo ""
    exit 1
fi

# Pass all arguments to the main script
exec "$SCRIPT_DIR/auto-generate-blog-image.sh" "$@"
