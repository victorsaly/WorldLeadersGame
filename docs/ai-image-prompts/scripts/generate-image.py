#!/usr/bin/env python3
"""
OpenAI Direct API Image Generator for LinkedIn Posts
Alternative to Azure OpenAI when regional restrictions apply
"""

import os
import requests
import json
from datetime import datetime
from pathlib import Path
from dotenv import load_dotenv

# Load environment variables
load_dotenv()

def generate_image_openai_direct(prompt_text, output_path="linkedin_image.png"):
    """
    Generate image using OpenAI Direct API (not Azure)
    """
    api_key = os.getenv("OPENAI_API_KEY")
    if not api_key:
        print("❌ Error: OPENAI_API_KEY not found in .env file")
        print("💡 Get your API key from: https://platform.openai.com/api-keys")
        return False
    
    print(f"🎨 Generating LinkedIn image using OpenAI Direct API...")
    print(f"📐 Size: 1792x1024 (LinkedIn optimized)")
    print(f"💰 Cost: ~$0.08")
    
    # OpenAI Direct API endpoint
    url = "https://api.openai.com/v1/images/generations"
    
    headers = {
        "Authorization": f"Bearer {api_key}",
        "Content-Type": "application/json"
    }
    
    payload = {
        "model": "dall-e-3",
        "prompt": prompt_text,
        "size": "1792x1024",  # LinkedIn landscape format
        "quality": "standard",  # or "hd" for higher quality (+cost)
        "n": 1
    }
    
    try:
        print("🔄 Sending request to OpenAI...")
        response = requests.post(url, headers=headers, json=payload, timeout=60)
        
        if response.status_code == 200:
            result = response.json()
            image_url = result["data"][0]["url"]
            
            print("✅ Image generated successfully!")
            print(f"🔗 Image URL: {image_url}")
            
            # Download the image
            print("💾 Downloading image...")
            image_response = requests.get(image_url)
            
            if image_response.status_code == 200:
                # Create output directory if it doesn't exist
                output_path = Path(output_path)
                output_path.parent.mkdir(parents=True, exist_ok=True)
                
                # Save the image
                with open(output_path, "wb") as f:
                    f.write(image_response.content)
                
                print(f"✅ Image saved: {output_path}")
                print(f"📊 File size: {len(image_response.content) // 1024} KB")
                return True
            else:
                print(f"❌ Failed to download image: {image_response.status_code}")
                return False
        else:
            print(f"❌ API Error: {response.status_code}")
            print(f"📝 Response: {response.text}")
            return False
            
    except requests.exceptions.Timeout:
        print("❌ Request timed out. Please try again.")
        return False
    except requests.exceptions.RequestException as e:
        print(f"❌ Network error: {e}")
        return False
    except Exception as e:
        print(f"❌ Unexpected error: {e}")
        return False

def main():
    """Main function to generate image from command line arguments"""
    import sys
    
    if len(sys.argv) < 2:
        print("Usage: python generate_image_openai.py <blog-post-name> [output-path]")
        print("Example: python generate_image_openai.py voice-memo-to-production")
        sys.exit(1)
    
    blog_post = sys.argv[1]
    output_path = sys.argv[2] if len(sys.argv) > 2 else f"../output/{blog_post}-linkedin.png"
    
    # Load the prompt for the specified blog post
    prompt_file = f"../blog-post-prompts/{blog_post}.md"
    
    if not os.path.exists(prompt_file):
        print(f"❌ Prompt file not found: {prompt_file}")
        print("Available prompts:")
        prompt_dir = Path("../blog-post-prompts/")
        if prompt_dir.exists():
            for f in prompt_dir.glob("*.md"):
                print(f"  - {f.stem}")
        sys.exit(1)
    
    print(f"📖 Loading prompt from: {prompt_file}")
    
    # Read and extract the prompt from the markdown file
    try:
        with open(prompt_file, 'r', encoding='utf-8') as f:
            content = f.read()
            
        # Extract the prompt section (between ## 📝 Azure OpenAI DALL-E 3 Optimized Prompt and next ##)
        lines = content.split('\n')
        prompt_lines = []
        in_prompt_section = False
        
        for line in lines:
            if '## 📝 Azure OpenAI DALL-E 3 Optimized Prompt' in line:
                in_prompt_section = True
                continue
            elif line.startswith('## ') and in_prompt_section:
                break
            elif in_prompt_section and line.strip():
                prompt_lines.append(line)
        
        prompt_text = '\n'.join(prompt_lines).strip()
        
        if not prompt_text:
            print("❌ Could not extract prompt from file")
            sys.exit(1)
            
        print(f"✅ Prompt loaded ({len(prompt_text)} characters)")
        print(f"🎯 Blog post: {blog_post}")
        print(f"📁 Output: {output_path}")
        
        # Generate the image
        success = generate_image_openai_direct(prompt_text, output_path)
        
        if success:
            print(f"\n🎉 LinkedIn image generated successfully!")
            print(f"📄 Blog post: {blog_post}")
            print(f"🖼️  Image: {output_path}")
            print(f"💡 Ready to upload to LinkedIn!")
        else:
            print(f"\n❌ Failed to generate image")
            sys.exit(1)
            
    except Exception as e:
        print(f"❌ Error reading prompt file: {e}")
        sys.exit(1)

if __name__ == "__main__":
    main()
