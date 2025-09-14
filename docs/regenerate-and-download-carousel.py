#!/usr/bin/env python3
"""
Regenerate and Download LinkedIn Carousel Slides
Generates fresh URLs and downloads immediately to avoid expiration
"""

import os
import sys
import subprocess
import requests
import urllib3
from pathlib import Path
import time

# Disable SSL warnings for downloads
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# Slide names in order
slides = [
    "linkedin-carousel-slide-1-title",
    "linkedin-carousel-slide-2-speakers", 
    "linkedin-carousel-slide-3-integration",
    "linkedin-carousel-slide-4-context-engineering",
    "linkedin-carousel-slide-5-reality-check",
    "linkedin-carousel-slide-6-success-framework"
]

def extract_url_from_output(output):
    """Extract the image URL from the generation script output"""
    lines = output.split('\n')
    for line in lines:
        if line.startswith('🔗 Image URL: '):
            return line.replace('🔗 Image URL: ', '').strip()
    return None

def download_image(url, filename):
    """Download image with SSL verification disabled"""
    try:
        print(f"📥 Downloading {filename}...")
        response = requests.get(url, verify=False, timeout=30)
        
        if response.status_code == 200:
            # Create output directory
            output_dir = Path("../output")
            output_dir.mkdir(parents=True, exist_ok=True)
            
            # Save the image
            output_path = output_dir / f"{filename}.png"
            with open(output_path, "wb") as f:
                f.write(response.content)
            
            file_size = len(response.content) // 1024
            print(f"✅ {filename} downloaded ({file_size} KB)")
            return True
        else:
            print(f"❌ Download failed: HTTP {response.status_code}")
            return False
            
    except Exception as e:
        print(f"❌ Download error: {e}")
        return False

def generate_and_download_slide(slide_name):
    """Generate a slide and download it immediately"""
    print(f"\n🎨 Processing {slide_name}...")
    print("-" * 50)
    
    try:
        # Run the generation script and capture output
        result = subprocess.run(
            ["python3", "generate-image.py", slide_name],
            cwd="/Users/victorsaly/Documents/StormDev/ConquerTheWorldGame/docs/ai-image-prompts/scripts",
            capture_output=True,
            text=True,
            timeout=120
        )
        
        print(result.stdout)
        
        if result.stderr:
            print("Stderr:", result.stderr)
        
        # Extract URL from output
        url = extract_url_from_output(result.stdout)
        
        if url:
            print(f"🔗 Fresh URL obtained: {url[:80]}...")
            
            # Download immediately
            success = download_image(url, slide_name)
            return success
        else:
            print("❌ Could not extract URL from generation output")
            return False
            
    except subprocess.TimeoutExpired:
        print("❌ Generation timed out")
        return False
    except Exception as e:
        print(f"❌ Generation error: {e}")
        return False

def main():
    """Generate and download all carousel slides"""
    print("🚀 LinkedIn Carousel Generator & Downloader")
    print("=" * 60)
    print("This will generate fresh URLs and download all 6 slides")
    print("Estimated time: 3-5 minutes")
    print("Cost: ~$0.48 (6 slides × $0.08 each)")
    print("=" * 60)
    
    successful = 0
    total = len(slides)
    
    for i, slide in enumerate(slides, 1):
        print(f"\n🎯 Slide {i}/{total}: {slide}")
        
        if generate_and_download_slide(slide):
            successful += 1
            print(f"✅ Slide {i} complete!")
        else:
            print(f"❌ Slide {i} failed")
        
        # Small delay between generations to be respectful to the API
        if i < total:
            print("⏳ Waiting 5 seconds before next generation...")
            time.sleep(5)
    
    print("\n" + "=" * 60)
    print(f"📊 Final Results: {successful}/{total} slides successful")
    
    if successful == total:
        print("🎉 All carousel slides generated and downloaded!")
        print("📁 Location: docs/ai-image-prompts/output/")
        print("📱 Ready to upload to LinkedIn as carousel!")
        print("\n💡 Next steps:")
        print("   1. Open LinkedIn")
        print("   2. Create new post")
        print("   3. Upload all 6 images as carousel")
        print("   4. Add engaging caption")
        print("   5. Use hashtags: #AIDrivenDevelopment #DeveloperExperience")
    else:
        print(f"⚠️  {total - successful} slides failed. You can retry those individually.")
    
    print("=" * 60)

if __name__ == "__main__":
    main()