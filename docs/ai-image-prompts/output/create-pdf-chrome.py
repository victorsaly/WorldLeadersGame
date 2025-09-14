#!/usr/bin/env python3
"""
Simple PDF generator using Chrome headless mode
Creates PDFs from HTML slides for LinkedIn carousel
"""

import os
import subprocess
import sys
from pathlib import Path

def create_pdf_with_chrome(html_file, pdf_file):
    """Create PDF using Chrome headless mode"""
    cmd = [
        '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome',
        '--headless',
        '--disable-gpu',
        '--run-all-compositor-stages-before-draw',
        '--virtual-time-budget=5000',
        '--print-to-pdf=' + pdf_file,
        '--print-to-pdf-no-header',
        '--disable-pdf-tagging',
        '--no-pdf-header-footer',
        '--no-margins',
        html_file
    ]
    
    try:
        result = subprocess.run(cmd, capture_output=True, text=True, timeout=30)
        if os.path.exists(pdf_file):
            print(f"✅ Created {pdf_file}")
            return True
        else:
            print(f"❌ Failed to create {pdf_file}")
            if result.stderr:
                print(f"   Error: {result.stderr}")
            return False
    except subprocess.TimeoutExpired:
        print(f"⏰ Timeout creating {pdf_file}")
        return False
    except Exception as e:
        print(f"❌ Error creating {pdf_file}: {e}")
        return False

def main():
    """Main function to convert HTML slides to PDF"""
    print("🔄 Converting HTML slides to PDF using Chrome...")
    
    # Check if Chrome is available
    chrome_path = '/Applications/Google Chrome.app/Contents/MacOS/Google Chrome'
    if not os.path.exists(chrome_path):
        print("❌ Google Chrome not found at expected location")
        print("   Please install Google Chrome or update the path in the script")
        return 1
    
    # Define paths
    slides_dir = "aidd-exact-style-slides"
    output_dir = "pdf-output"
    
    # Create output directory
    os.makedirs(output_dir, exist_ok=True)
    
    # Check if slides directory exists
    if not os.path.exists(slides_dir):
        print(f"❌ Slides directory '{slides_dir}' not found")
        print("   Make sure you've generated the slides first")
        return 1
    
    # Get all HTML slide files
    html_files = []
    for i in range(1, 7):
        html_file = f"{slides_dir}/slide-{i}.html"
        if os.path.exists(html_file):
            html_files.append(html_file)
        else:
            print(f"⚠️  Warning: {html_file} not found")
    
    if not html_files:
        print("❌ No slide files found")
        return 1
    
    print(f"📄 Found {len(html_files)} slides to convert")
    
    # Convert each HTML to PDF
    pdf_files = []
    for html_file in html_files:
        slide_name = os.path.basename(html_file).replace('.html', '')
        pdf_file = os.path.abspath(f"{output_dir}/{slide_name}.pdf")
        html_file_abs = os.path.abspath(html_file)
        
        print(f"🔄 Converting {html_file} -> {slide_name}.pdf")
        
        if create_pdf_with_chrome(f"file://{html_file_abs}", pdf_file):
            pdf_files.append(pdf_file)
    
    if not pdf_files:
        print("❌ No PDFs were created successfully")
        return 1
    
    print(f"\n✅ Successfully created {len(pdf_files)} individual PDFs")
    print(f"📁 PDFs saved to: {os.path.abspath(output_dir)}/")
    
    # List created files
    print("\n📄 Created files:")
    for pdf_file in pdf_files:
        filename = os.path.basename(pdf_file)
        file_size = os.path.getsize(pdf_file) / 1024  # KB
        print(f"   • {filename} ({file_size:.1f} KB)")
    
    print("\n💡 Upload to LinkedIn:")
    print("   1. Go to LinkedIn and create a new post")
    print("   2. Click 'Add media' → 'Upload document'")
    print("   3. Select individual PDF files or create a combined PDF")
    print("   4. LinkedIn will automatically create a carousel from multiple pages")
    print("\n🎯 Pro tip: Upload slides in order (slide-1.pdf, slide-2.pdf, etc.)")
    
    return 0

if __name__ == "__main__":
    sys.exit(main())