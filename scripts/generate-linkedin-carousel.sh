#!/bin/bash

# LinkedIn Carousel Slide Generator
# Generates all 6 slides for the "5 AI Development Game-Changers" carousel

set -e

echo "🎨 LinkedIn Carousel Slide Generator"
echo "==================================="
echo ""

# Check if we're in the right directory
if [[ ! -d "docs/ai-image-prompts/scripts" ]]; then
    echo "❌ Please run this script from the project root directory"
    exit 1
fi

# Create output directory
OUTPUT_DIR="docs/ai-image-prompts/output/linkedin-carousel-$(date +%Y%m%d)"
mkdir -p "$OUTPUT_DIR"

echo "📁 Output directory: $OUTPUT_DIR"
echo ""

# Check if Python script exists
GENERATOR_SCRIPT="docs/ai-image-prompts/scripts/generate-image.py"
if [[ ! -f "$GENERATOR_SCRIPT" ]]; then
    echo "❌ Generator script not found: $GENERATOR_SCRIPT"
    exit 1
fi

# Array of slide files
declare -a slides=(
    "slide-1-title:Title Slide"
    "slide-2-strategic-agents:Strategic Agents"
    "slide-3-testing-revolution:Testing Revolution"
    "slide-4-context-engineering:Context Engineering"
    "slide-5-reality-check:Reality Check"
    "slide-6-leadership-framework:Leadership Framework"
)

echo "🚀 Generating LinkedIn carousel slides..."
echo ""

# Generate each slide
for slide_info in "${slides[@]}"; do
    IFS=':' read -r slide_file slide_name <<< "$slide_info"
    
    echo "🎨 Generating: $slide_name"
    
    # Input prompt file
    PROMPT_FILE="docs/ai-image-prompts/linkedin-carousel/${slide_file}.md"
    
    # Output image file
    OUTPUT_FILE="$OUTPUT_DIR/${slide_file}.png"
    
    # Check if prompt file exists
    if [[ ! -f "$PROMPT_FILE" ]]; then
        echo "❌ Prompt file not found: $PROMPT_FILE"
        continue
    fi
    
    # Generate the image
    echo "   📝 Using prompt: $PROMPT_FILE"
    echo "   💾 Output: $OUTPUT_FILE"
    
    cd docs/ai-image-prompts/scripts
    python3 generate-image.py --prompt-file "../linkedin-carousel/${slide_file}.md" --output "../../../$OUTPUT_FILE" --size "1024x1024"
    cd ../../../
    
    if [[ -f "$OUTPUT_FILE" ]]; then
        echo "   ✅ Generated successfully!"
    else
        echo "   ❌ Generation failed!"
    fi
    echo ""
done

echo "🎉 Carousel generation complete!"
echo ""
echo "📋 Next steps:"
echo "1. Review generated slides in: $OUTPUT_DIR"
echo "2. Create PDF carousel: Use PowerPoint or online PDF merger"
echo "3. Test mobile readability: Check text visibility on phone"
echo "4. Upload to LinkedIn: Use as document carousel post"
echo ""
echo "💡 Pro tips:"
echo "• LinkedIn carousels can have up to 20 slides"
echo "• PDF format allows text selection and better quality"
echo "• Test posting with a small audience first"
echo "• Include engaging copy to introduce the carousel"
echo ""
echo "📊 Success metrics to track:"
echo "• Carousel completion rate (views of last slide)"
echo "• Save rate (professionals bookmarking)"
echo "• Comment quality (strategic discussions)"
echo "• Profile visits (professional interest)"