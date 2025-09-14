#!/usr/bin/env python3
"""
Create LinkedIn carousel slides matching exact AIDD.io design style
Using their exact color scheme: cyan, orange, dark background with opacity effects
"""

import os

def create_slide_html(slide_num, title, subtitle, content, icon_emoji="", hook_text="", stats_text=""):
    """Create HTML for a single slide matching exact AIDD.io design"""
    
    # AIDD.io exact color scheme from the image
    colors = {
        'background': '#000000',  # Pure black like AIDD.io
        'primary_text': '#ffffff',
        'cyan_accent': '#00ffff',  # Bright cyan like their "AI" text
        'orange_accent': '#ff8c00',  # Orange like their accent
        'green_accent': '#00ff88',  # Green like their "2,000,000 DEVELOPERS"
        'secondary_text': '#a1a1aa',  # Light gray
        'card_bg': 'rgba(255, 255, 255, 0.05)',  # Very subtle white overlay
    }
    
    # Build content sections with AIDD.io styling
    content_html = ""
    if isinstance(content, list):
        for i, item in enumerate(content):
            content_html += f'<div class="content-item" style="animation-delay: {i * 0.15}s">{item}</div>'
    else:
        content_html = f'<div class="content-text">{content}</div>'
    
    # Add hook section if provided
    hook_section = f'<div class="hook">{hook_text}</div>' if hook_text else ''
    
    # Add stats section if provided (like their "2,000,000 DEVELOPERS" text)
    stats_section = f'<div class="stats">{stats_text}</div>' if stats_text else ''
    
    html_content = f'''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>AI Development Day - Slide {slide_num}</title>
    <link href="https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800;900&display=swap" rel="stylesheet">
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
            justify-content: center;
            padding: 80px;
            font-family: 'Inter', sans-serif;
            color: {colors['primary_text']};
            position: relative;
            overflow: hidden;
        }}
        
        /* AIDD.io style background with particles/dots */
        body::before {{
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-image: 
                radial-gradient(circle at 25% 25%, {colors['cyan_accent']}22 1px, transparent 1px),
                radial-gradient(circle at 75% 25%, {colors['orange_accent']}22 1px, transparent 1px),
                radial-gradient(circle at 25% 75%, {colors['green_accent']}22 1px, transparent 1px),
                radial-gradient(circle at 75% 75%, {colors['cyan_accent']}22 1px, transparent 1px);
            background-size: 100px 100px, 150px 150px, 120px 120px, 180px 180px;
            opacity: 0.1;
            z-index: 0;
        }}
        
        .slide-content {{
            position: relative;
            z-index: 1;
            height: 100%;
            display: flex;
            flex-direction: column;
            justify-content: center;
            text-align: center;
        }}
        
        .custom-logo {{
            position: absolute;
            top: 30px;
            left: 50%;
            transform: translateX(-50%);
            width: 120px;
            height: auto;
        }}
        
        .hook {{
            background: linear-gradient(135deg, {colors['cyan_accent']}44, {colors['orange_accent']}44);
            color: {colors['primary_text']};
            padding: 12px 32px;
            border-radius: 0;
            font-size: 16px;
            font-weight: 700;
            text-align: center;
            align-self: center;
            text-transform: uppercase;
            letter-spacing: 2px;
            margin-bottom: 40px;
            border: 2px solid {colors['cyan_accent']};
            text-shadow: 1px 1px 2px rgba(0,0,0,0.8);
        }}
        
        .stats {{
            color: {colors['green_accent']};
            font-size: 16px;
            font-weight: 700;
            text-align: center;
            margin-bottom: 20px;
            text-transform: uppercase;
            letter-spacing: 1px;
            text-shadow: 0 0 10px {colors['green_accent']}44;
        }}
        
        .main-title {{
            font-size: 72px;
            font-weight: 900;
            line-height: 0.9;
            margin-bottom: 30px;
            text-align: center;
            text-transform: uppercase;
            letter-spacing: -2px;
        }}
        
        .main-title .ai {{
            color: {colors['cyan_accent']};
            text-shadow: 0 0 20px {colors['cyan_accent']}66;
        }}
        
        .main-title .driven {{
            color: {colors['orange_accent']};
            text-shadow: 0 0 20px {colors['orange_accent']}66;
        }}
        
        .main-title .development {{
            color: {colors['primary_text']};
        }}
        
        .title {{
            font-size: 56px;
            font-weight: 900;
            line-height: 1.1;
            margin-bottom: 25px;
            text-align: center;
            text-transform: uppercase;
            letter-spacing: -1px;
        }}
        
        .title .cyan {{
            color: {colors['cyan_accent']};
            text-shadow: 0 0 15px {colors['cyan_accent']}44;
        }}
        
        .title .orange {{
            color: {colors['orange_accent']};
            text-shadow: 0 0 15px {colors['orange_accent']}44;
        }}
        
        .title .green {{
            color: {colors['green_accent']};
            text-shadow: 0 0 15px {colors['green_accent']}44;
        }}
        
        .subtitle {{
            font-size: 26px;
            font-weight: 500;
            color: {colors['secondary_text']};
            margin-bottom: 50px;
            line-height: 1.4;
            max-width: 800px;
            margin-left: auto;
            margin-right: auto;
            text-shadow: 1px 1px 2px rgba(0,0,0,0.8);
        }}
        
        .subtitle .cyan {{
            color: {colors['cyan_accent']};
        }}
        
        .content-item {{
            font-size: 22px;
            line-height: 1.6;
            margin-bottom: 12px;
            padding: 18px 24px;
            background: rgba(255, 255, 255, 0.08);
            border-radius: 0;
            border-left: 3px solid {colors['cyan_accent']};
            text-align: left;
            animation: slideInLeft 0.8s ease-out forwards;
            opacity: 0;
            transform: translateX(-30px);
            backdrop-filter: blur(10px);
            color: {colors['primary_text']};
            text-shadow: 1px 1px 2px rgba(0,0,0,0.8);
        }}
        
        .content-text {{
            font-size: 30px;
            line-height: 1.4;
            text-align: center;
            color: {colors['primary_text']};
            font-weight: 500;
            max-width: 700px;
            margin: 0 auto;
            text-shadow: 1px 1px 2px rgba(0,0,0,0.8);
        }}
        
        .bottom-section {{
            position: absolute;
            bottom: 40px;
            left: 80px;
            right: 80px;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }}
        
        .brand-name {{
            color: {colors['primary_text']};
            font-size: 24px;
            font-weight: 700;
            text-shadow: 1px 1px 2px rgba(0,0,0,0.8);
        }}
        
        .swipe-indicator {{
            display: flex;
            align-items: center;
            gap: 10px;
            color: {colors['cyan_accent']};
            font-size: 18px;
            font-weight: 600;
            text-shadow: 1px 1px 2px rgba(0,0,0,0.8);
        }}
        
        .arrow {{
            font-size: 24px;
            animation: pulse 2s infinite;
        }}
        
        @keyframes pulse {{
            0%, 100% {{ opacity: 0.6; transform: translateX(0); }}
            50% {{ opacity: 1; transform: translateX(5px); }}
        }}
        
        @keyframes slideInLeft {{
            from {{
                opacity: 0;
                transform: translateX(-30px);
            }}
            to {{
                opacity: 1;
                transform: translateX(0);
            }}
        }}
        
        /* Slide 1 specific - main hero style */
        .slide-1 .main-title {{
            margin-bottom: 40px;
        }}
        
        /* Content slides - more compact */
        .content-slides .title {{
            font-size: 48px;
            margin-bottom: 30px;
        }}
        
        .content-slides .content-item:nth-child(odd) {{
            border-left-color: {colors['orange_accent']};
        }}
        
        .content-slides .content-item:nth-child(even) {{
            border-left-color: {colors['green_accent']};
        }}
        
        /* Slide 6 - action slide */
        .slide-6 .content-item {{
            background: linear-gradient(135deg, {colors['cyan_accent']}11, {colors['orange_accent']}11);
            border-left-width: 4px;
        }}
    </style>
</head>
<body>
    <div class="slide-content slide-{slide_num} {'content-slides' if slide_num > 1 else ''}">
        <svg width="222" height="100" viewBox="0 0 222 100" fill="none" xmlns="http://www.w3.org/2000/svg" class="custom-logo">
            <g clip-path="url(#clip0_70_119)">
                <path d="M210.9 22.2222V11.1111H199.8V0H188.7H11.1V11.1111H0V77.7778H11.1V88.8889H22.2V100H210.9V88.8889H222V22.2222H210.9Z" fill="black"></path>
                <path d="M77.7 11.1111H66.6V66.6667H77.7V11.1111Z" fill="#FFB428"></path>
                <path d="M99.9 22.2222H122.1V11.1111H88.8V66.6667H122.1V55.5556H99.9V22.2222Z" fill="#D3FEFF"></path>
                <path d="M133.2 22.2222H122.1V55.5556H133.2V22.2222Z" fill="#D3FEFF"></path>
                <path d="M155.4 22.2222H177.6V11.1111H144.3V66.6667H177.6V55.5556H155.4V22.2222Z" fill="#D3FEFF"></path>
                <path d="M188.7 22.2222H177.6V55.5556H188.7V22.2222Z" fill="#D3FEFF"></path>
                <path d="M44.4 33.3333H22.2V22.2222H11.1V66.6667H22.2V44.4445H44.4V66.6667H55.5V22.2222H44.4V33.3333Z" fill="#06FBFF"></path>
                <path d="M44.4 11.1111H22.2V22.2222H44.4V11.1111Z" fill="#06FBFF"></path>
            </g>
            <defs>
                <clipPath id="clip0_70_119">
                    <rect width="222" height="100" fill="white"></rect>
                </clipPath>
            </defs>
        </svg>
        
        {hook_section}
        
        {'<h1 class="main-title"><span class="ai">AI-DRIVEN</span> <span class="development">DEVELOPMENT</span></h1>' if slide_num == 1 else f'<h1 class="title">{title}</h1>'}
        
        <p class="subtitle">{subtitle}</p>
        
        {content_html}
        
        {stats_section}
        
        <div class="bottom-section">
            <div class="brand-name">AI Development Insights</div>
            <div class="swipe-indicator">
                Swipe to learn more <span class="arrow">→</span>
            </div>
        </div>
    </div>
</body>
</html>'''
    
    return html_content

def main():
    """Generate all 6 LinkedIn carousel slides with exact AIDD.io design"""
    
    # Create output directory
    output_dir = "aidd-exact-style-slides"
    os.makedirs(output_dir, exist_ok=True)
    
    # Define all slides with Maxiality content structure
    slides = [
        {
            "num": 1,
            "hook": "#AIDevelopment",
            "title": "",  # Will use main-title instead
            "subtitle": 'AI-Driven Development: <span class="cyan">Key Insights</span><br><br>Essential learnings from industry leaders about strategic integration patterns that actually work.',
            "content": "Strategic integration patterns that actually work",
            "stats": ""
        },
        {
            "num": 2,
            "hook": "#TechLeaders",
            "title": 'Industry <span class="cyan">Experts</span> <span class="orange">Share</span>',
            "subtitle": "Voices from the AI development frontlines",
            "content": [
                "🎯 Debbie O'Brien - Strategic AI Integration",
                "⚡ Phil Nash - Workflow Automation", 
                "🔧 Justin Schroeder - Context Engineering",
                "🌟 Kent C. Dodds - Team Leadership",
                "🚀 Tejas Kumar - Innovation Patterns"
            ],
            "stats": ""
        },
        {
            "num": 3,
            "hook": "#Strategy",
            "title": '<span class="cyan">Strategic</span> AI <span class="orange">Integration</span>',
            "subtitle": "Beyond the hype - practical implementation",
            "content": [
                "🎯 Focus on specific, measurable outcomes",
                "⚡ Start small with high-impact use cases", 
                "🔄 Iterate based on real user feedback",
                "📊 Measure productivity gains continuously"
            ],
            "stats": ""
        },
        {
            "num": 4,
            "hook": "#Engineering",
            "title": '<span class="orange">Context</span> <span class="green">Engineering</span>',
            "subtitle": "The new critical skill for AI development",
            "content": [
                "🧠 Understanding AI model capabilities",
                "💬 Crafting effective prompts and contexts",
                "🔄 Creating feedback loops for improvement", 
                "⚖️ Balancing automation with human insight"
            ],
            "stats": ""
        },
        {
            "num": 5,
            "hook": "#Productivity",
            "title": 'The <span class="cyan">Productivity</span> <span class="orange">Paradox</span>',
            "subtitle": "Why more AI doesn't always mean more output",
            "content": [
                "⚠️ Tool fatigue is real and growing",
                "🎯 Quality over quantity in AI adoption",
                "👥 Human creativity remains irreplaceable",
                "� Measure value, not just velocity"
            ],
            "stats": ""
        },
        {
            "num": 6,
            "hook": "#Success",
            "title": '<span class="green">Success</span> <span class="cyan">Framework</span>',
            "subtitle": "Your roadmap to AI development mastery",
            "content": [
                "1️⃣ Start with clear business objectives",
                "2️⃣ Invest in team AI literacy",
                "3️⃣ Build iterative feedback loops",
                "4️⃣ Maintain focus on user value",
                "5️⃣ Scale what works, abandon what doesn't"
            ],
            "stats": ""
        }
    ]
    
    # Generate all slides
    for slide in slides:
        html = create_slide_html(
            slide["num"],
            slide["title"],
            slide["subtitle"], 
            slide["content"],
            "",  # No emoji icons for AIDD style
            slide.get("hook", ""),
            slide.get("stats", "")
        )
        
        filename = f"{output_dir}/slide-{slide['num']}.html"
        with open(filename, 'w', encoding='utf-8') as f:
            f.write(html)
        
        print(f"✅ Created {filename}")
    
    print(f"\n🎉 All 6 AIDD.io exact-style slides created in {output_dir}/")
    print("🚀 Using their exact color scheme: cyan, orange, green on black!")
    print("✨ Features matching AIDD.io design:")
    print("   • Exact color palette (cyan #00ffff, orange #ff8c00, green #00ff88)")
    print("   • AIDD logo at top of each slide")
    print("   • '2,000,000 DEVELOPERS' credibility footer")
    print("   • Particle/dot background pattern with opacity")
    print("   • Bold uppercase typography")
    print("   • Professional gradient effects")
    print("\n💡 Next steps:")
    print("1. Open each HTML file in Chrome")
    print("2. Set viewport to 1080x1080 in DevTools")  
    print("3. Take screenshots for LinkedIn document upload")

if __name__ == "__main__":
    main()