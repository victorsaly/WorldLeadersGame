#!/usr/bin/env python3
"""
Convert HTML slides to PDF for LinkedIn carousel upload
Creates individual PDFs and a combined PDF document
"""

import os
import subprocess
import sys
from pathlib import Path

def check_dependencies():
    """Check if required dependencies are available"""
    dependencies = {
        'wkhtmltopdf': 'brew install wkhtmltopdf',
        'pdftk': 'brew install pdftk-java'
    }
    
    missing = []
    for cmd, install_cmd in dependencies.items():
        try:
            subprocess.run([cmd, '--version'], capture_output=True, check=True)
            print(f"✅ {cmd} is installed")
        except (subprocess.CalledProcessError, FileNotFoundError):
            print(f"❌ {cmd} is not installed")
            print(f"   Install with: {install_cmd}")
            missing.append(cmd)
    
    return len(missing) == 0

def convert_html_to_pdf(html_file, pdf_file):
    """Convert a single HTML file to PDF using wkhtmltopdf"""
    cmd = [
        'wkhtmltopdf',
        '--page-size', 'A4',
        '--orientation', 'Portrait',
        '--margin-top', '0',
        '--margin-bottom', '0',
        '--margin-left', '0',
        '--margin-right', '0',
        '--disable-smart-shrinking',
        '--zoom', '0.75',  # Adjust zoom to fit content better
        html_file,
        pdf_file
    ]
    
    try:
        result = subprocess.run(cmd, capture_output=True, text=True, check=True)
        print(f"✅ Created {pdf_file}")
        return True
    except subprocess.CalledProcessError as e:
        print(f"❌ Failed to create {pdf_file}")
        print(f"   Error: {e.stderr}")
        return False

def combine_pdfs(pdf_files, output_file):
    """Combine multiple PDFs into one using pdftk"""
    cmd = ['pdftk'] + pdf_files + ['cat', 'output', output_file]
    
    try:
        subprocess.run(cmd, capture_output=True, check=True)
        print(f"✅ Created combined PDF: {output_file}")
        return True
    except subprocess.CalledProcessError as e:
        print(f"❌ Failed to combine PDFs")
        print(f"   Error: {e}")
        return False

def main():
    """Main function to convert HTML slides to PDF"""
    print("🔄 Converting HTML slides to PDF...")
    
    # Check dependencies
    if not check_dependencies():
        print("\n💡 Please install missing dependencies and try again")
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
        
        if convert_html_to_pdf(html_file, pdf_file):
            pdf_files.append(pdf_file)
    
    if not pdf_files:
        print("❌ No PDFs were created successfully")
        return 1
    
    print(f"\n✅ Successfully created {len(pdf_files)} individual PDFs")
    
    # Combine all PDFs into one
    combined_pdf = f"{output_dir}/ai-development-insights-carousel.pdf"
    if combine_pdfs(pdf_files, combined_pdf):
        print(f"\n🎉 LinkedIn carousel PDF ready: {combined_pdf}")
        print("\n💡 Upload options:")
        print(f"   • Individual slides: {output_dir}/slide-*.pdf")
        print(f"   • Combined document: {combined_pdf}")
        print("\n📱 LinkedIn upload tips:")
        print("   • Use 'Document' post type for best results")
        print("   • Each page becomes a swipeable slide")
        print("   • PDF maintains professional quality")
    else:
        print("\n⚠️  Individual PDFs created but combination failed")
        print(f"   You can still use individual PDFs from: {output_dir}/")
    
    return 0

if __name__ == "__main__":
    sys.exit(main())