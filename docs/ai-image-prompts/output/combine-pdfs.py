#!/usr/bin/env python3
"""
Combine individual PDF slides into a single LinkedIn carousel document
Uses system tools available on macOS
"""

import os
import subprocess
import sys

def combine_with_python():
    """Try to combine PDFs using Python (requires PyPDF2)"""
    try:
        # First try to use PyPDF2 if available
        subprocess.run([sys.executable, '-c', 'import PyPDF2'], check=True, capture_output=True)
        
        # PyPDF2 is available, use it
        import PyPDF2
        
        merger = PyPDF2.PdfMerger()
        
        # Add slides in order
        for i in range(1, 7):
            pdf_file = f"slide-{i}.pdf"
            if os.path.exists(pdf_file):
                print(f"📄 Adding {pdf_file}")
                merger.append(pdf_file)
        
        output_file = "ai-development-insights-carousel.pdf"
        merger.write(output_file)
        merger.close()
        
        print(f"✅ Combined PDF created: {output_file}")
        return True
        
    except (subprocess.CalledProcessError, ImportError):
        return False

def combine_with_system():
    """Try to combine PDFs using system tools"""
    output_file = "ai-development-insights-carousel.pdf"
    
    # Try using macOS's built-in Python script approach
    python_combine_script = '''
import sys
try:
    from PyPDF2 import PdfMerger
    merger = PdfMerger()
    for i in range(1, 7):
        file = f"slide-{i}.pdf"
        if os.path.exists(file):
            merger.append(file)
    merger.write("ai-development-insights-carousel.pdf")
    merger.close()
    print("✅ Combined using PyPDF2")
    sys.exit(0)
except ImportError:
    print("PyPDF2 not available")
    sys.exit(1)
'''
    
    # Try the Python approach first
    try:
        result = subprocess.run([
            sys.executable, '-c', python_combine_script
        ], capture_output=True, text=True, cwd=os.getcwd())
        
        if result.returncode == 0 and os.path.exists(output_file):
            print(result.stdout)
            return True
    except Exception:
        pass
    
    # Fallback: create a simple instruction file
    create_manual_instructions()
    return False

def create_manual_instructions():
    """Create instructions for manual combination"""
    instructions = """
# LinkedIn Carousel PDF - Manual Combination Instructions

## Option 1: Use Preview (macOS)
1. Open slide-1.pdf in Preview
2. View → Thumbnails (⌘⌥2)
3. Drag slide-2.pdf into the thumbnails area
4. Repeat for slides 3-6
5. File → Export as PDF → Save as "ai-development-insights-carousel.pdf"

## Option 2: Use online tool
1. Go to https://smallpdf.com/merge-pdf
2. Upload slide-1.pdf through slide-6.pdf in order
3. Download the merged PDF

## Option 3: Individual upload to LinkedIn
Upload slides individually to LinkedIn - it will automatically create a carousel!

## Files ready for upload:
"""
    
    # Add file list
    for i in range(1, 7):
        pdf_file = f"slide-{i}.pdf"
        if os.path.exists(pdf_file):
            size = os.path.getsize(pdf_file) / 1024
            instructions += f"• {pdf_file} ({size:.1f} KB)\n"
    
    with open("LINKEDIN_UPLOAD_INSTRUCTIONS.txt", "w") as f:
        f.write(instructions)
    
    print("📝 Created LINKEDIN_UPLOAD_INSTRUCTIONS.txt")

def main():
    """Main function"""
    print("🔗 Combining PDF slides for LinkedIn carousel...")
    
    # Check if we're in the right directory
    if not os.path.exists("slide-1.pdf"):
        print("❌ No slide PDFs found in current directory")
        print("   Please run this from the pdf-output directory")
        return 1
    
    # Count available slides
    slide_count = 0
    for i in range(1, 7):
        if os.path.exists(f"slide-{i}.pdf"):
            slide_count += 1
    
    print(f"📄 Found {slide_count} slides to combine")
    
    # Try to combine PDFs
    if combine_with_python():
        file_size = os.path.getsize("ai-development-insights-carousel.pdf") / 1024
        print(f"📊 Combined PDF size: {file_size:.1f} KB")
        print("\n🎉 LinkedIn carousel PDF ready!")
        print("\n💡 Upload options:")
        print("   • Combined PDF: ai-development-insights-carousel.pdf")
        print("   • Individual slides: slide-1.pdf through slide-6.pdf")
    else:
        print("\n⚠️  Automatic combination not available")
        print("   Using individual slides or manual combination")
        create_manual_instructions()
    
    print("\n📱 LinkedIn upload tips:")
    print("   • Use 'Document' post type for best carousel results")
    print("   • Each page becomes a swipeable slide")
    print("   • PDF maintains professional quality and branding")
    print("   • Add engaging caption with relevant hashtags")
    
    return 0

if __name__ == "__main__":
    os.chdir("pdf-output")  # Change to PDF output directory
    sys.exit(main())