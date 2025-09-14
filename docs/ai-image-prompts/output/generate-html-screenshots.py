#!/usr/bin/env python3
"""
HTML Carousel to PNG Screenshots
Generate high-quality LinkedIn carousel images from HTML
"""

import os
import time
from pathlib import Path
try:
    from selenium import webdriver
    from selenium.webdriver.chrome.options import Options
    from selenium.webdriver.common.by import By
    from selenium.webdriver.support.ui import WebDriverWait
    from selenium.webdriver.support import expected_conditions as EC
except ImportError:
    print("❌ Selenium not installed. Install with: pip install selenium")
    exit(1)

def setup_driver():
    """Setup Chrome driver for high-quality screenshots"""
    chrome_options = Options()
    chrome_options.add_argument("--headless")
    chrome_options.add_argument("--no-sandbox")
    chrome_options.add_argument("--disable-dev-shm-usage")
    chrome_options.add_argument("--disable-gpu")
    chrome_options.add_argument("--window-size=1200,1200")
    chrome_options.add_argument("--force-device-scale-factor=2")  # High DPI
    
    try:
        driver = webdriver.Chrome(options=chrome_options)
        return driver
    except Exception as e:
        print(f"❌ Chrome driver setup failed: {e}")
        print("💡 Install ChromeDriver: brew install chromedriver")
        return None

def capture_slide(driver, slide_index):
    """Capture a specific slide as PNG"""
    try:
        # Find the specific slide
        slide = driver.find_element(By.CSS_SELECTOR, f".slide-{slide_index + 1}")
        
        # Scroll to slide
        driver.execute_script("arguments[0].scrollIntoView({block: 'center'});", slide)
        time.sleep(1)  # Wait for scroll animation
        
        # Get slide position and size
        location = slide.location
        size = slide.size
        
        # Take screenshot of full page
        driver.save_screenshot(f"temp_full_{slide_index + 1}.png")
        
        # Crop to slide using PIL
        from PIL import Image
        
        # Open full screenshot
        full_image = Image.open(f"temp_full_{slide_index + 1}.png")
        
        # Calculate crop box (account for device scale factor)
        scale_factor = 2
        left = location['x'] * scale_factor
        top = location['y'] * scale_factor
        right = left + (size['width'] * scale_factor)
        bottom = top + (size['height'] * scale_factor)
        
        # Crop to slide
        slide_image = full_image.crop((left, top, right, bottom))
        
        # Resize to LinkedIn optimized size (1080x1080)
        slide_image = slide_image.resize((1080, 1080), Image.Resampling.LANCZOS)
        
        # Save final slide
        output_path = f"slide-{slide_index + 1}-html.png"
        slide_image.save(output_path, "PNG", quality=95, optimize=True)
        
        # Cleanup temp file
        os.remove(f"temp_full_{slide_index + 1}.png")
        
        print(f"✅ Slide {slide_index + 1} saved: {output_path}")
        return True
        
    except Exception as e:
        print(f"❌ Failed to capture slide {slide_index + 1}: {e}")
        return False

def main():
    """Generate all carousel slides from HTML"""
    print("🎨 HTML Carousel Screenshot Generator")
    print("=" * 50)
    
    # Check if HTML file exists
    html_file = Path("linkedin-carousel-html.html")
    if not html_file.exists():
        print("❌ HTML file not found: linkedin-carousel-html.html")
        return
    
    # Setup WebDriver
    print("🔧 Setting up Chrome WebDriver...")
    driver = setup_driver()
    if not driver:
        return
    
    try:
        # Load HTML file
        html_path = html_file.absolute().as_uri()
        print(f"📖 Loading HTML: {html_path}")
        driver.get(html_path)
        
        # Wait for page to load
        WebDriverWait(driver, 10).until(
            EC.presence_of_element_located((By.CLASS_NAME, "carousel-slide"))
        )
        
        # Give time for fonts and styling to load
        time.sleep(3)
        
        print("📸 Capturing slides...")
        
        successful_captures = 0
        total_slides = 6
        
        for i in range(total_slides):
            print(f"\n🎯 Capturing slide {i + 1}/{total_slides}...")
            if capture_slide(driver, i):
                successful_captures += 1
            time.sleep(1)  # Brief pause between captures
        
        print("\n" + "=" * 50)
        print(f"📊 Results: {successful_captures}/{total_slides} slides captured")
        
        if successful_captures == total_slides:
            print("🎉 All carousel slides generated successfully!")
            print("📱 LinkedIn-ready format: 1080x1080px")
            print("🎨 High-quality retro design with consistent branding")
            print("\n💡 Next steps:")
            print("   1. Upload to LinkedIn as carousel post")
            print("   2. Add engaging caption")
            print("   3. Use hashtags: #AIDrivenDevelopment #DeveloperExperience")
        else:
            print("⚠️  Some slides failed to capture")
            
    except Exception as e:
        print(f"❌ Screenshot generation failed: {e}")
        
    finally:
        driver.quit()
        print("🔧 WebDriver closed")

if __name__ == "__main__":
    main()