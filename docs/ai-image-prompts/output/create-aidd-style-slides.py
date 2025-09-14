#!/usr/bin/env python3
"""
Create LinkedIn carousel slides inspired by AIDD.io design style
Clean, professional, results-focused approach
"""

import os

def create_slide_html(slide_num, title, subtitle, content, icon_emoji="", hook_text="", stats_text=""):
    """Create HTML for a single slide matching AIDD.io design style"""
    
    # AIDD.io inspired color scheme - clean and professional
    colors = {
        'background': 'linear-gradient(135deg, #0f172a 0%, #1e293b 100%)',  # Dark blue-gray like tech sites
        'primary_text': '#ffffff',
        'accent_color': '#3b82f6',  # Clean blue accent
        'secondary_text': '#cbd5e1',
        'highlight': '#06b6d4',  # Cyan highlight
        'success': '#10b981'   # Green for results
    }
    
    # Build content sections
    content_html = ""
    if isinstance(content, list):
        for i, item in enumerate(content):
            content_html += f'<div class="content-item" style="animation-delay: {i * 0.1}s">{item}</div>'
    else:
        content_html = f'<div class="content-text">{content}</div>'
    
    # Add hook section if provided
    hook_section = f'<div class="hook">{hook_text}</div>' if hook_text else ''
    
    # Add stats section if provided
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
            justify-content: space-between;
            padding: 60px;
            font-family: 'Inter', sans-serif;
            color: {colors['primary_text']};
            position: relative;
            overflow: hidden;
        }}
        
        /* Subtle background pattern */
        body::before {{
            content: '';
            position: absolute;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background: 
                radial-gradient(circle at 20% 50%, rgba(59, 130, 246, 0.1) 0%, transparent 50%),
                radial-gradient(circle at 80% 20%, rgba(6, 182, 212, 0.1) 0%, transparent 50%),
                radial-gradient(circle at 40% 80%, rgba(16, 185, 129, 0.1) 0%, transparent 50%);
            z-index: 0;
        }}
        
        .slide-content {{
            position: relative;
            z-index: 1;
            height: 100%;
            display: flex;
            flex-direction: column;
            justify-content: space-between;
        }}
        
        .hook {{
            background: {colors['accent_color']};
            color: white;
            padding: 12px 24px;
            border-radius: 6px;
            font-size: 16px;
            font-weight: 600;
            text-align: center;
            align-self: flex-start;
            text-transform: uppercase;
            letter-spacing: 1px;
            margin-bottom: 30px;
            box-shadow: 0 4px 20px rgba(59, 130, 246, 0.3);
        }}
        
        .stats {{
            background: rgba(16, 185, 129, 0.1);
            border: 1px solid rgba(16, 185, 129, 0.2);
            color: {colors['success']};
            padding: 15px 25px;
            border-radius: 8px;
            font-size: 18px;
            font-weight: 600;
            text-align: center;
            margin-bottom: 30px;
            box-shadow: 0 4px 20px rgba(16, 185, 129, 0.1);
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
            margin-bottom: 30px;
            opacity: 0.9;
            filter: drop-shadow(0 4px 20px rgba(59, 130, 246, 0.3));
        }}
        
        .title {{
            font-size: 48px;
            font-weight: 900;
            line-height: 1.1;
            margin-bottom: 20px;
            text-align: center;
        }}
        
        .title .accent {{
            color: {colors['accent_color']};
            text-shadow: 0 0 20px rgba(59, 130, 246, 0.5);
        }}
        
        .title .highlight {{
            color: {colors['highlight']};
            text-shadow: 0 0 20px rgba(6, 182, 212, 0.5);
        }}
        
        .subtitle {{
            font-size: 24px;
            font-weight: 500;
            color: {colors['secondary_text']};
            margin-bottom: 40px;
            line-height: 1.4;
            opacity: 0.9;
        }}
        
        .content-item {{
            font-size: 22px;
            line-height: 1.5;
            margin-bottom: 16px;
            padding: 20px;
            background: rgba(255, 255, 255, 0.05);
            border-radius: 12px;
            border-left: 3px solid {colors['accent_color']};
            text-align: left;
            animation: fadeInUp 0.6s ease-out forwards;
            opacity: 0;
            transform: translateY(20px);
            box-shadow: 0 4px 15px rgba(0, 0, 0, 0.1);
        }}
        
        .content-text {{
            font-size: 28px;
            line-height: 1.4;
            text-align: center;
            color: {colors['secondary_text']};
            font-weight: 500;
        }}
        
        .branding {{
            display: flex;
            justify-content: space-between;
            align-items: center;
            margin-top: 40px;
            padding-top: 30px;
            border-top: 1px solid rgba(255, 255, 255, 0.1);
        }}
        
        .brand-name {{
            font-size: 24px;
            font-weight: 700;
            color: {colors['primary_text']};
            display: flex;
            align-items: center;
            gap: 10px;
        }}
        
        .brand-name::before {{
            content: '🚀';
            font-size: 20px;
        }}
        
        .slide-number {{
            background: rgba(255, 255, 255, 0.1);
            color: {colors['secondary_text']};
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
        }}
        
        @keyframes fadeInUp {{
            from {{
                opacity: 0;
                transform: translateY(20px);
            }}
            to {{
                opacity: 1;
                transform: translateY(0);
            }}
        }}
        
        /* Slide-specific styling */
        .slide-1 .title {{
            background: linear-gradient(135deg, {colors['accent_color']} 0%, {colors['highlight']} 100%);
            -webkit-background-clip: text;
            -webkit-text-fill-color: transparent;
            background-clip: text;
        }}
        
        .slide-6 .content-item {{
            background: linear-gradient(135deg, rgba(16, 185, 129, 0.1) 0%, rgba(59, 130, 246, 0.1) 100%);
            border-left-color: {colors['success']};
        }}
    </style>
</head>
<body>
    <div class="slide-content slide-{slide_num}">
        {hook_section}
        {stats_section}
        
        <div class="main-content">
            <div class="icon">{icon_emoji}</div>
            <h1 class="title">{title}</h1>
            <p class="subtitle">{subtitle}</p>
            {content_html}
        </div>
        
        <div class="branding">
            <div class="brand-name">AI Development Day 2025</div>
            <div class="slide-number">{slide_num}/6</div>
        </div>
    </div>
</body>
</html>'''
    
    return html_content

def main():
    """Generate all 6 LinkedIn carousel slides with AIDD.io inspired design"""
    
    # Create output directory
    output_dir = "aidd-style-slides"
    os.makedirs(output_dir, exist_ok=True)
    
    # Define all slides with AIDD.io style structure
    slides = [
        {
            "num": 1,
            "hook": "AI DEVELOPMENT INSIGHTS",
            "stats": "FROM INDUSTRY LEADERS AT TOP COMPANIES",
            "title": 'Ship Features in <span class="accent">Hours</span>,<br>Not <span class="highlight">Weeks</span>',
            "subtitle": "Key insights from AI-Driven Development Day 2025",
            "content": "Learn how teams are using AI to operate 20x their size",
            "icon": "🚀"
        },
        {
            "num": 2,
            "hook": "EXPERT SPEAKERS",
            "title": 'Industry Leaders <span class="accent">Share</span>',
            "subtitle": "Insights from developers at companies that trained 2M+ professionals",
            "content": [
                "🎯 Debbie O'Brien - Strategic AI Integration Patterns",
                "⚡ Phil Nash - Workflow Automation at Scale", 
                "🔧 Justin Schroeder - Advanced Context Engineering",
                "🌟 Kent C. Dodds - AI-Powered Team Leadership",
                "🚀 Tejas Kumar - Innovation & Implementation"
            ],
            "icon": "👥"
        },
        {
            "num": 3,
            "hook": "STRATEGIC IMPLEMENTATION",
            "title": '<span class="accent">Strategic</span> AI Integration',
            "subtitle": "Move beyond experimentation to production-ready AI workflows",
            "content": [
                "🎯 Define specific, measurable success metrics",
                "⚡ Start with high-impact, low-risk use cases", 
                "🔄 Build iterative feedback loops for improvement",
                "📊 Measure productivity gains, not just velocity",
                "🎪 Scale proven patterns across your organization"
            ],
            "icon": "🎯"
        },
        {
            "num": 4,
            "hook": "CORE SKILL",
            "title": '<span class="highlight">Context</span> Engineering',
            "subtitle": "The critical skill that separates AI-native teams from everyone else",
            "content": [
                "🧠 Understanding AI model capabilities and limitations",
                "💬 Crafting effective prompts and context windows",
                "🔄 Creating continuous improvement feedback loops", 
                "⚖️ Balancing automation with human creativity",
                "📈 Measuring and optimizing AI-human collaboration"
            ],
            "icon": "🧠"
        },
        {
            "num": 5,
            "hook": "PRODUCTIVITY REALITY",
            "title": 'The <span class="accent">Productivity</span> Paradox',
            "subtitle": "Why more AI tools don't always equal better outcomes",
            "content": [
                "⚠️ Tool fatigue is real - teams are overwhelmed",
                "🎯 Quality over quantity in AI tool adoption",
                "👥 Human creativity and judgment remain essential",
                "📈 Focus on value delivery, not feature velocity",
                "🔍 Measure what matters: user impact, not output"
            ],
            "icon": "⚖️"
        },
        {
            "num": 6,
            "hook": "ACTION FRAMEWORK",
            "stats": "READY TO BUILD FASTER & SMARTER?",
            "title": 'Your <span class="accent">AI Success</span> Framework',
            "subtitle": "5 steps to transform your development workflow",
            "content": [
                "1️⃣ Start with clear business objectives and success metrics",
                "2️⃣ Invest in team AI literacy and context engineering",
                "3️⃣ Build rapid feedback loops for continuous improvement",
                "4️⃣ Maintain laser focus on delivering user value",
                "5️⃣ Scale proven patterns, abandon failed experiments"
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
            slide.get("hook", ""),
            slide.get("stats", "")
        )
        
        filename = f"{output_dir}/slide-{slide['num']}.html"
        with open(filename, 'w', encoding='utf-8') as f:
            f.write(html)
        
        print(f"✅ Created {filename}")
    
    print(f"\n🎉 All 6 AIDD.io-style slides created in {output_dir}/")
    print("🚀 Professional, results-focused LinkedIn carousel ready!")
    print("\n💡 Next steps:")
    print("1. Open each HTML file in Chrome")
    print("2. Set viewport to 1080x1080 in DevTools")  
    print("3. Take screenshots for LinkedIn document upload")
    print("4. Upload as document carousel for maximum professional impact")

if __name__ == "__main__":
    main()