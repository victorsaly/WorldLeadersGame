#!/usr/bin/env python3
"""
Simple HTML to Image Converter using html2image
Alternative to Selenium for generating carousel slides
"""

try:
    from html2image import Html2Image
except ImportError:
    print("❌ html2image not installed")
    print("Install with: pip install html2image")
    print("Also install ChromeDriver: brew install chromedriver")
    exit(1)

from pathlib import Path
import os

def generate_carousel_images():
    """Generate LinkedIn carousel images from HTML"""
    print("🎨 HTML to Image Carousel Generator")
    print("=" * 50)
    
    # Check if HTML file exists
    html_file = Path("linkedin-carousel-html.html")
    if not html_file.exists():
        print("❌ HTML file not found: linkedin-carousel-html.html")
        return False
    
    # Initialize html2image
    hti = Html2Image(
        size=(1080, 1080),  # LinkedIn optimal size
        output_path="./",
        browser_executable="/Applications/Google Chrome.app/Contents/MacOS/Google Chrome"  # macOS Chrome path
    )
    
    # Read HTML content
    with open(html_file, 'r', encoding='utf-8') as f:
        html_content = f.read()
    
    print("📸 Generating carousel slides...")
    
    # Add custom CSS to show only one slide at a time
    slide_css = """
    <style>
        body { background: #1a1a1a; }
        .carousel-slide { margin: 20px auto; }
        .carousel-slide:not(.active) { display: none; }
    </style>
    """
    
    successful_captures = 0
    
    for slide_num in range(1, 7):
        try:
            print(f"🎯 Generating slide {slide_num}/6...")
            
            # Create HTML with only current slide active
            slide_html = html_content.replace(
                f'<div class="carousel-slide slide-{slide_num}">',
                f'<div class="carousel-slide slide-{slide_num} active">'
            )
            
            # Add CSS to show only active slide
            slide_html = slide_html.replace('<head>', f'<head>{slide_css}')
            
            # Generate image
            hti.screenshot(
                html_str=slide_html,
                save_as=f"slide-{slide_num}-html.png",
                size=(1080, 1080)
            )
            
            successful_captures += 1
            print(f"✅ Slide {slide_num} generated successfully")
            
        except Exception as e:
            print(f"❌ Failed to generate slide {slide_num}: {e}")
    
    print("\n" + "=" * 50)
    print(f"📊 Results: {successful_captures}/6 slides generated")
    
    if successful_captures == 6:
        print("🎉 Complete LinkedIn carousel ready!")
        print("📱 Format: 1080x1080px (LinkedIn optimized)")
        print("🎨 Retro brand design with pixel-art aesthetic")
    else:
        print("⚠️  Some slides failed. Try manual screenshot approach below.")
    
    return successful_captures == 6

def main():
    """Main function with fallback instructions"""
    success = False
    
    try:
        success = generate_carousel_images()
    except Exception as e:
        print(f"❌ Automated generation failed: {e}")
    
    if not success:
        print("\n" + "🔧 MANUAL SCREENSHOT INSTRUCTIONS" + "\n" + "=" * 50)
        print("1. Open linkedin-carousel-html.html in Chrome")
        print("2. Press F12 to open DevTools")
        print("3. Click the device toggle (📱 icon)")
        print("4. Set custom size: 1080 x 1080")
        print("5. Scroll to each slide and take screenshots")
        print("6. Use browser screenshot tools or:")
        print("   - macOS: Cmd+Shift+4, then drag to select slide")
        print("   - Save as slide-1-html.png, slide-2-html.png, etc.")
        print("\n💡 Pro tip: Use Chrome's full page screenshot feature")
        print("   DevTools → Command Menu (Cmd+Shift+P) → 'Capture node screenshot'")

if __name__ == "__main__":
    main()