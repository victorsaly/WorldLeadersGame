#!/usr/bin/env python3
"""
Download LinkedIn Carousel Slides
Handles SSL certificate verification issues by disabling SSL verification for these specific downloads
"""

import requests
import urllib3
from pathlib import Path

# Disable SSL warnings for this specific download task
urllib3.disable_warnings(urllib3.exceptions.InsecureRequestWarning)

# Carousel slide URLs (expire in ~2 hours from generation)
slides = {
    "slide-1-title": "https://oaidalleapiprodscus.blob.core.windows.net/private/org-M1Xd7CdmA74jylXQPwDC5Wfi/user-3CsZOA2eyyQXJvWWAwnVc7g5/img-M71keFAoqutFfKZLtKASrEyb.png",
    "slide-2-speakers": "https://oaidalleapiprodscus.blob.core.windows.net/private/org-M1Xd7CdmA74jylXQPwDC5Wfi/user-3CsZOA2eyyQXJvWWAwnVc7g5/img-WTCYnyNhrnN6nYKo7YmS9gCX.png",
    "slide-3-integration": "https://oaidalleapiprodscus.blob.core.windows.net/private/org-M1Xd7CdmA74jylXQPwDC5Wfi/user-3CsZOA2eyyQXJvWWAwnVc7g5/img-9G4IqWS7Ng4VTJDJVWFEzfTq.png",
    "slide-4-context": "https://oaidalleapiprodscus.blob.core.windows.net/private/org-M1Xd7CdmA74jylXQPwDC5Wfi/user-3CsZOA2eyyQXJvWWAwnVc7g5/img-3p0nHTuFfcF4WjqMV9t8Cz24.png",
    "slide-5-reality": "https://oaidalleapiprodscus.blob.core.windows.net/private/org-M1Xd7CdmA74jylXQPwDC5Wfi/user-3CsZOA2eyyQXJvWWAwnVc7g5/img-ScCF6RIbEeBNwzhaj796b19i.png",
    "slide-6-framework": "https://oaidalleapiprodscus.blob.core.windows.net/private/org-M1Xd7CdmA74jylXQPwDC5Wfi/user-3CsZOA2eyyQXJvWWAwnVc7g5/img-I1AHaOob9EEFQxlZbMofTGse.png"
}

def download_slide(url, filename):
    """Download a single slide with SSL verification disabled"""
    try:
        print(f"📥 Downloading {filename}...")
        
        # Use verify=False to bypass SSL certificate verification
        response = requests.get(url, verify=False, timeout=30)
        
        if response.status_code == 200:
            # Create output directory if it doesn't exist
            output_dir = Path("ai-image-prompts/output")
            output_dir.mkdir(parents=True, exist_ok=True)
            
            # Save the image
            output_path = output_dir / f"{filename}.png"
            with open(output_path, "wb") as f:
                f.write(response.content)
            
            file_size = len(response.content) // 1024
            print(f"✅ {filename} saved ({file_size} KB)")
            return True
        else:
            print(f"❌ Failed to download {filename}: HTTP {response.status_code}")
            return False
            
    except requests.exceptions.RequestException as e:
        print(f"❌ Network error downloading {filename}: {e}")
        return False
    except Exception as e:
        print(f"❌ Unexpected error downloading {filename}: {e}")
        return False

def main():
    """Download all carousel slides"""
    print("🎨 LinkedIn Carousel Slide Downloader")
    print("=" * 50)
    
    successful_downloads = 0
    total_slides = len(slides)
    
    for filename, url in slides.items():
        if download_slide(url, filename):
            successful_downloads += 1
        print()  # Add spacing between downloads
    
    print("=" * 50)
    print(f"📊 Download Summary: {successful_downloads}/{total_slides} successful")
    
    if successful_downloads == total_slides:
        print("🎉 All carousel slides downloaded successfully!")
        print("📁 Location: docs/ai-image-prompts/output/")
        print("💡 Ready to upload to LinkedIn as a carousel!")
    else:
        print("⚠️  Some downloads failed. Check the URLs and try again.")
        print("⏰ Note: URLs expire 2 hours after generation")

if __name__ == "__main__":
    main()