#!/usr/bin/env python3
"""
Create professional LinkedIn carousel slides matching Maxiality style guide
Based on AI Driven Development Day 2025 conference insights
"""

import os

def create_slide_html(slide_num, title, subtitle, content, icon_emoji="💡", hook_text=""):
    """Create HTML for a single slide matching Maxiality design patterns"""
    
    # Color scheme matching Maxiality examples
    colors = {
        'background': 'linear-gradient(135deg, #1a1a1a 0%, #2d2d2d 100%)',
        'primary_text': '#ffffff',
        'accent_color': '#e91e63',  # Pink accent like Maxiality
        'secondary_text': '#b0b0b0',
        'hook_bg': '#e91e63'
    }
    
    # Build content sections
    content_html = ""
    if isinstance(content, list):
        for item in content:
            content_html += f'<div class="content-item">{item}</div>'
    else:
        content_html = f'<div class="content-text">{content}</div>'
    
    # Add hook section if provided
    hook_section = f'<div class="hook">{hook_text}</div>' if hook_text else ''
    
    html_content = f'''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>AI Development Day - Slide {slide_num}</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;600;700;800&family=Poppins:wght@400;600;700;800&display=swap" rel="stylesheet">
    <style>
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}
        
        body {{
            width: 1080px;
            height: 1080px;
            background: {colors['background']};
            display: flex;
            flex-direction: column;
            justify-content: space-between;
            padding: 60px;
            font-family: 'Inter', 'Poppins', sans-serif;
            color: {colors['primary_text']};
            position: relative;
            overflow: hidden;
        }}
        
        .hook {{
            background: {colors['hook_bg']};
            color: white;
            padding: 15px 30px;
            border-radius: 30px;
            font-size: 18px;
            font-weight: 600;
            text-align: center;
            margin-bottom: 30px;
            align-self: center;
            text-transform: uppercase;
            letter-spacing: 1px;
        }}
        
        .main-content {{
            flex: 1;
            display: flex;
            flex-direction: column;
            justify-content: center;
            text-align: center;
        }}
        
        .icon {{
            font-size: 80px;
            margin-bottom: 40px;
            opacity: 0.9;
        }}
        
        .title {{
            font-size: 52px;
            font-weight: 800;
            line-height: 1.1;
            margin-bottom: 20px;
            font-family: 'Poppins', sans-serif;
        }}
        
        .title .accent {{
            color: {colors['accent_color']};
        }}
        
        .subtitle {{
            font-size: 28px;
            font-weight: 600;
            color: {colors['secondary_text']};
            margin-bottom: 40px;
            line-height: 1.3;
        }}
        
        .content-item {{
            font-size: 24px;
            line-height: 1.4;
            margin-bottom: 20px;
            padding: 15px;
            background: rgba(255, 255, 255, 0.05);
            border-radius: 15px;
            border-left: 4px solid {colors['accent_color']};
        }}
        
        .content-text {{
            font-size: 26px;
            line-height: 1.4;
            text-align: center;
            color: {colors['secondary_text']};
        }}
        
        .branding {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 40px;
            padding-top: 30px;
            border-top: 2px solid rgba(255, 255, 255, 0.1);
        }}
        
        .brand-name {{
            font-size: 32px;
            font-weight: 700;
            color: {colors['primary_text']};
        }}
        
        .swipe-indicator {{
            display: flex;
            align-items: center;
            gap: 10px;
            color: {colors['accent_color']};
            font-size: 18px;
            font-weight: 600;
        }}
        
        .arrow {{
            font-size: 24px;
            animation: pulse 2s infinite;
        }}
        
        @keyframes pulse {{
            0%, 100% {{ opacity: 0.6; transform: translateX(0); }}
            50% {{ opacity: 1; transform: translateX(5px); }}
        }}
        
        /* Slide-specific styling */
        .slide-{slide_num} {{
            /* Add slide-specific styles if needed */
        }}
    </style>
</head>
<body class="slide-{slide_num}">
    {hook_section}
    
    <div class="main-content">
        <div class="icon">{icon_emoji}</div>
        <h1 class="title">{title}</h1>
        <p class="subtitle">{subtitle}</p>
        {content_html}
    </div>
    
    <div class="branding">
        <div class="brand-name">AI Development Insights</div>
        <div class="swipe-indicator">
            Swipe to learn more <span class="arrow">→</span>
        </div>
    </div>
</body>
</html>'''
    
    return html_content

def main():
    """Generate all 6 LinkedIn carousel slides"""
    
    # Create output directory
    output_dir = "maxiality-style-slides"
    os.makedirs(output_dir, exist_ok=True)
    
    # Define all slides with Maxiality-style structure
    slides = [
        {
            "num": 1,
            "hook": "#AIDevelopment",
            "title": 'AI-Driven Development:<br><span class="accent">Key Insights</span>',
            "subtitle": "Essential learnings from industry leaders",
            "content": "Strategic integration patterns that actually work",
            "icon": "🚀"
        },
        {
            "num": 2,
            "hook": "#TechLeaders",
            "title": 'Industry <span class="accent">Experts</span> Share',
            "subtitle": "Voices from the AI development frontlines",
            "content": [
                "🎯 Debbie O'Brien - Strategic AI Integration",
                "⚡ Phil Nash - Workflow Automation", 
                "🔧 Justin Schroeder - Context Engineering",
                "🌟 Kent C. Dodds - Team Leadership",
                "🚀 Tejas Kumar - Innovation Patterns"
            ],
            "icon": "👥"
        },
        {
            "num": 3,
            "hook": "#Strategy",
            "title": '<span class="accent">Strategic</span> AI Integration',
            "subtitle": "Beyond the hype - practical implementation",
            "content": [
                "🎯 Focus on specific, measurable outcomes",
                "⚡ Start small with high-impact use cases", 
                "🔄 Iterate based on real user feedback",
                "📊 Measure productivity gains continuously"
            ],
            "icon": "🎯"
        },
        {
            "num": 4,
            "hook": "#Engineering",
            "title": '<span class="accent">Context</span> Engineering',
            "subtitle": "The new critical skill for AI development",
            "content": [
                "🧠 Understanding AI model capabilities",
                "💬 Crafting effective prompts and contexts",
                "🔄 Creating feedback loops for improvement", 
                "⚖️ Balancing automation with human insight"
            ],
            "icon": "🧠"
        },
        {
            "num": 5,
            "hook": "#Productivity",
            "title": 'The <span class="accent">Productivity</span> Paradox',
            "subtitle": "Why more AI doesn't always mean more output",
            "content": [
                "⚠️ Tool fatigue is real and growing",
                "🎯 Quality over quantity in AI adoption",
                "👥 Human creativity remains irreplaceable",
                "📈 Measure value, not just velocity"
            ],
            "icon": "⚖️"
        },
        {
            "num": 6,
            "hook": "#Success",
            "title": '<span class="accent">Success</span> Framework',
            "subtitle": "Your roadmap to AI development mastery",
            "content": [
                "1️⃣ Start with clear business objectives",
                "2️⃣ Invest in team AI literacy",
                "3️⃣ Build iterative feedback loops",
                "4️⃣ Maintain focus on user value",
                "5️⃣ Scale what works, abandon what doesn't"
            ],
            "icon": "🏆"
        }
    ]
    
    # Generate all slides
    for slide in slides:
        html = create_slide_html(
            slide["num"],
            slide["title"],
            slide["subtitle"], 
            slide["content"],
            slide["icon"],
            slide["hook"]
        )
        
        filename = f"{output_dir}/slide-{slide['num']}.html"
        with open(filename, 'w', encoding='utf-8') as f:
            f.write(html)
        
        print(f"✅ Created {filename}")
    
    print(f"\n🎉 All 6 Maxiality-style slides created in {output_dir}/")
    print("📱 Ready for LinkedIn carousel screenshots!")
    print("\n💡 Next steps:")
    print("1. Open each HTML file in Chrome")
    print("2. Set viewport to 1080x1080 in DevTools")
    print("3. Take screenshots for LinkedIn document upload")

if __name__ == "__main__":
    main()