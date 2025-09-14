#!/usr/bin/env python3
"""
Convert HTML slides to PDF using Playwright (modern alternative)
Creates individual PDFs and a combined PDF document for LinkedIn carousel
"""

import os
import sys
import subprocess
from pathlib import Path
import asyncio

def check_playwright():
    """Check if playwright is available and install if needed"""
    try:
        import playwright
        print("✅ Playwright is available")
        return True
    except ImportError:
        print("❌ Playwright not found, installing...")
        try:
            subprocess.run([sys.executable, '-m', 'pip', 'install', 'playwright'], check=True)
            subprocess.run([sys.executable, '-m', 'playwright', 'install', 'chromium'], check=True)
            print("✅ Playwright installed successfully")
            return True
        except subprocess.CalledProcessError as e:
            print(f"❌ Failed to install Playwright: {e}")
            return False

async def convert_html_to_pdf_playwright(html_file, pdf_file):
    """Convert HTML to PDF using Playwright"""
    try:
        from playwright.async_api import async_playwright
        
        async with async_playwright() as p:
            browser = await p.chromium.launch()
            page = await browser.new_page()
            
            # Set viewport for LinkedIn carousel (square format)
            await page.set_viewport_size({"width": 1080, "height": 1080})
            
            # Load the HTML file
            file_url = f"file://{os.path.abspath(html_file)}"
            await page.goto(file_url)
            
            # Wait for any animations or loading
            await page.wait_for_timeout(1000)
            
            # Generate PDF with specific settings for LinkedIn
            await page.pdf(
                path=pdf_file,
                format='A4',
                print_background=True,
                margin={
                    'top': '0px',
                    'bottom': '0px',
                    'left': '0px',
                    'right': '0px'
                }
            )
            
            await browser.close()
            
        print(f"✅ Created {pdf_file}")
        return True
        
    except Exception as e:
        print(f"❌ Failed to create {pdf_file}: {e}")
        return False

def combine_pdfs_pypdf(pdf_files, output_file):
    """Combine PDFs using PyPDF2"""
    try:
        import PyPDF2
        
        merger = PyPDF2.PdfMerger()
        
        for pdf_file in pdf_files:
            merger.append(pdf_file)
        
        merger.write(output_file)
        merger.close()
        
        print(f"✅ Created combined PDF: {output_file}")
        return True
        
    except ImportError:
        print("❌ PyPDF2 not found, installing...")
        try:
            subprocess.run([sys.executable, '-m', 'pip', 'install', 'PyPDF2'], check=True)
            # Retry with PyPDF2 now installed
            import PyPDF2
            merger = PyPDF2.PdfMerger()
            for pdf_file in pdf_files:
                merger.append(pdf_file)
            merger.write(output_file)
            merger.close()
            print(f"✅ Created combined PDF: {output_file}")
            return True
        except Exception as e:
            print(f"❌ Failed to install/use PyPDF2: {e}")
            return False
    except Exception as e:
        print(f"❌ Failed to combine PDFs: {e}")
        return False

async def main():
    """Main async function to convert HTML slides to PDF"""
    print("🔄 Converting HTML slides to PDF using Playwright...")
    
    # Check and install Playwright if needed
    if not check_playwright():
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
        pdf_file = f"{output_dir}/{slide_name}.pdf"
        
        if await convert_html_to_pdf_playwright(html_file, pdf_file):
            pdf_files.append(pdf_file)
    
    if not pdf_files:
        print("❌ No PDFs were created successfully")
        return 1
    
    print(f"\n✅ Successfully created {len(pdf_files)} individual PDFs")
    
    # Combine all PDFs into one
    combined_pdf = f"{output_dir}/ai-development-insights-carousel.pdf"
    if combine_pdfs_pypdf(pdf_files, combined_pdf):
        print(f"\n🎉 LinkedIn carousel PDF ready: {combined_pdf}")
        print("\n💡 Upload options:")
        print(f"   • Individual slides: {output_dir}/slide-*.pdf")
        print(f"   • Combined document: {combined_pdf}")
        print("\n📱 LinkedIn upload tips:")
        print("   • Use 'Document' post type for best results")
        print("   • Each page becomes a swipeable slide")
        print("   • PDF maintains professional quality")
        print("   • LinkedIn supports up to 300 pages per document")
    else:
        print("\n⚠️  Individual PDFs created but combination failed")
        print(f"   You can still use individual PDFs from: {output_dir}/")
    
    return 0

def run_main():
    """Wrapper to run async main function"""
    return asyncio.run(main())

if __name__ == "__main__":
    sys.exit(run_main())