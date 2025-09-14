#!/usr/bin/env python3
"""
Create Individual HTML Slides for LinkedIn Carousel
Simple approach to generate separate HTML files for easy screenshot capture
"""

import os
from pathlib import Path

# Create individual slides directory
os.makedirs("individual-slides", exist_ok=True)

slides_data = [
    {
        "num": 1,
        "border": "#3B82F6",
        "accent": "#3B82F6",
        "title": "AI-DRIVEN DEVELOPMENT DAY 2025",
        "subtitle": "Game-Changing Insights for Modern Developers",
        "content": '''
            <div class="main-visual">🚀</div>
            <div class="highlight-box">
                <p class="highlight-text">The Future of Development is Here</p>
            </div>
            <div class="metrics">
                <div class="metric-box">
                    <div class="metric-value">5</div>
                    <div class="metric-label">Expert Speakers</div>
                </div>
                <div class="metric-box">
                    <div class="metric-value">100+</div>
                    <div class="metric-label">Attendees</div>
                </div>
            </div>
        ''',
        "footer": "Conference Insights • September 2025"
    },
    {
        "num": 2,
        "border": "#F59E0B",
        "accent": "#F59E0B",
        "title": "EXPERT SPEAKERS",
        "subtitle": "Industry Leaders Sharing Game-Changing Insights",
        "content": '''
            <div class="speaker-grid">
                <div class="speaker-card">
                    <div class="speaker-name">DEBBIE O'BRIEN</div>
                    <div class="speaker-topic">Strategic AI Integration & Developer Experience</div>
                </div>
                <div class="speaker-card">
                    <div class="speaker-name">PHIL NASH</div>
                    <div class="speaker-topic">Testing Revolution with AI-Enhanced Workflows</div>
                </div>
                <div class="speaker-card">
                    <div class="speaker-name">KENT C. DODDS</div>
                    <div class="speaker-topic">Context Engineering & Productivity Patterns</div>
                </div>
                <div class="speaker-card">
                    <div class="speaker-name">TEJAS KUMAR</div>
                    <div class="speaker-topic">AI-First Development Methodology</div>
                </div>
            </div>
            <div class="highlight-box">
                <p class="highlight-text">Real-World Insights from Production Environments</p>
            </div>
        ''',
        "footer": "Expert Knowledge • Proven Strategies"
    },
    {
        "num": 3,
        "border": "#3B82F6",
        "accent": "#3B82F6",
        "title": "STRATEGIC AI INTEGRATION",
        "subtitle": "Beyond Hype: Real Developer Experience Improvements",
        "content": '''
            <div class="main-visual">⚡</div>
            <div class="metrics">
                <div class="metric-box">
                    <div class="metric-value">3x</div>
                    <div class="metric-label">Faster Debugging</div>
                </div>
                <div class="metric-box">
                    <div class="metric-value">60%</div>
                    <div class="metric-label">Less Boilerplate</div>
                </div>
            </div>
            <div class="highlight-box">
                <p class="highlight-text">AI as Collaborative Partner, Not Replacement</p>
            </div>
        ''',
        "footer": "Strategic Adoption • Measurable Results"
    },
    {
        "num": 4,
        "border": "#22c55e",
        "accent": "#22c55e",
        "title": "CONTEXT ENGINEERING",
        "subtitle": "The New Discipline Every Developer Needs",
        "content": '''
            <div class="main-visual">🎯</div>
            <div class="highlight-box">
                <p class="highlight-text">"Context is the new code architecture"</p>
            </div>
            <div class="metrics">
                <div class="metric-box">
                    <div class="metric-value">5x</div>
                    <div class="metric-label">Better AI Responses</div>
                </div>
                <div class="metric-box">
                    <div class="metric-value">90%</div>
                    <div class="metric-label">First-Try Success</div>
                </div>
            </div>
        ''',
        "footer": "Context Engineering • Strategic Prompting"
    },
    {
        "num": 5,
        "border": "#F59E0B",
        "accent": "#F59E0B",
        "title": "THE PRODUCTIVITY PARADOX",
        "subtitle": "Reality Check: The Learning Curve is Real",
        "content": '''
            <div class="main-visual">📊</div>
            <div class="metrics">
                <div class="metric-box">
                    <div class="metric-value">24%</div>
                    <div class="metric-label">Expected Faster ⚡</div>
                </div>
                <div class="metric-box">
                    <div class="metric-value">19%</div>
                    <div class="metric-label">Reality: Slower Initially 🐌</div>
                </div>
            </div>
            <div class="highlight-box">
                <p class="highlight-text">Strategic Adoption is Key to Success</p>
            </div>
        ''',
        "footer": "Honest Assessment • Realistic Expectations"
    },
    {
        "num": 6,
        "border": "#3B82F6",
        "accent": "#3B82F6",
        "title": "YOUR SUCCESS FRAMEWORK",
        "subtitle": "Actionable Steps for AI-Enhanced Development",
        "content": '''
            <div class="framework-steps">
                <div class="framework-step">
                    <div class="step-number">1</div>
                    <div class="step-content">
                        <h3>🎯 STRATEGIC ADOPTION</h3>
                        <p>Choose the right tools • Focus on specific tasks</p>
                    </div>
                </div>
                <div class="framework-step">
                    <div class="step-number">2</div>
                    <div class="step-content">
                        <h3>📚 CONTINUOUS LEARNING</h3>
                        <p>Develop AI literacy • Stay updated on trends</p>
                    </div>
                </div>
                <div class="framework-step">
                    <div class="step-number">3</div>
                    <div class="step-content">
                        <h3>⚡ ITERATIVE IMPROVEMENT</h3>
                        <p>Refine your approach • Measure and optimize</p>
                    </div>
                </div>
            </div>
            <div class="highlight-box">
                <p class="highlight-text">Start Your AI Journey Today!</p>
            </div>
        ''',
        "footer": "Actionable Framework • Proven Results"
    }
]

html_template = '''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>LinkedIn Carousel - Slide {num}</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Press+Start+2P&family=Orbitron:wght@400;700;900&display=swap');
        
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: 'Orbitron', monospace;
            background: #1a1a1a;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            padding: 20px;
        }}

        .carousel-slide {{
            width: 1080px;
            height: 1080px;
            position: relative;
            border: 4px solid {border_color};
            box-shadow: 8px 8px 0 #000;
            image-rendering: pixelated;
            background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }}

        .slide-header {{
            background: linear-gradient(135deg, #1f2937 0%, #374151 100%);
            color: white;
            padding: 40px;
            text-align: center;
            position: relative;
        }}

        .slide-header::after {{
            content: '';
            position: absolute;
            bottom: 0;
            left: 0;
            right: 0;
            height: 4px;
            background: {accent_color};
        }}

        .slide-title {{
            font-family: 'Press Start 2P', monospace;
            font-size: 24px;
            line-height: 1.4;
            text-shadow: 2px 2px 0 #000;
            margin-bottom: 20px;
        }}

        .slide-subtitle {{
            font-size: 16px;
            opacity: 0.9;
            font-weight: 400;
        }}

        .slide-content {{
            flex: 1;
            padding: 60px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
        }}

        .main-visual {{
            font-size: 120px;
            margin: 40px 0;
            filter: drop-shadow(4px 4px 0 rgba(0,0,0,0.3));
        }}

        .highlight-box {{
            background: white;
            border: 4px solid #000;
            padding: 30px;
            margin: 20px 0;
            box-shadow: 4px 4px 0 rgba(0,0,0,0.2);
            min-width: 80%;
        }}

        .highlight-text {{
            font-family: 'Press Start 2P', monospace;
            font-size: 20px;
            color: #1f2937;
            line-height: 1.6;
        }}

        .metrics {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 30px;
            width: 100%;
            margin: 40px 0;
        }}

        .metric-box {{
            background: white;
            border: 4px solid #000;
            padding: 30px;
            text-align: center;
            box-shadow: 4px 4px 0 rgba(0,0,0,0.2);
        }}

        .metric-value {{
            font-family: 'Press Start 2P', monospace;
            font-size: 36px;
            color: {accent_color};
            margin-bottom: 15px;
        }}

        .metric-label {{
            font-size: 14px;
            font-weight: 700;
            color: #374151;
            text-transform: uppercase;
        }}

        .speaker-grid {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            width: 100%;
            margin: 30px 0;
        }}

        .speaker-card {{
            background: white;
            border: 3px solid #000;
            padding: 20px;
            text-align: center;
            box-shadow: 3px 3px 0 rgba(0,0,0,0.2);
        }}

        .speaker-name {{
            font-family: 'Press Start 2P', monospace;
            font-size: 14px;
            color: #F59E0B;
            margin-bottom: 10px;
        }}

        .speaker-topic {{
            font-size: 12px;
            color: #374151;
            line-height: 1.4;
        }}

        .framework-steps {{
            width: 100%;
            margin: 30px 0;
        }}

        .framework-step {{
            display: flex;
            align-items: center;
            background: white;
            border: 3px solid #000;
            margin: 20px 0;
            padding: 25px;
            box-shadow: 3px 3px 0 rgba(0,0,0,0.2);
        }}

        .step-number {{
            font-family: 'Press Start 2P', monospace;
            font-size: 32px;
            color: {accent_color};
            margin-right: 30px;
            min-width: 60px;
        }}

        .step-content h3 {{
            font-family: 'Press Start 2P', monospace;
            font-size: 16px;
            color: #1f2937;
            margin-bottom: 10px;
        }}

        .step-content p {{
            font-size: 14px;
            color: #374151;
            line-height: 1.4;
        }}

        .slide-footer {{
            background: #1f2937;
            color: white;
            padding: 20px;
            text-align: center;
            font-size: 14px;
            font-weight: 700;
        }}

        .slide-number {{
            position: absolute;
            top: 20px;
            right: 20px;
            background: rgba(0,0,0,0.8);
            color: white;
            padding: 10px 15px;
            font-family: 'Press Start 2P', monospace;
            font-size: 12px;
            border: 2px solid #fff;
        }}
    </style>
</head>
<body>
    <div class="carousel-slide">
        <div class="slide-number">{num}/6</div>
        <div class="slide-header">
            <h1 class="slide-title">{title}</h1>
            <p class="slide-subtitle">{subtitle}</p>
        </div>
        <div class="slide-content">
            {content}
        </div>
        <div class="slide-footer">{footer}</div>
    </div>
</body>
</html>'''

# Generate each slide
print("🎨 Creating individual LinkedIn carousel slides...")

for slide in slides_data:
    html_content = html_template.format(
        num=slide["num"],
        border_color=slide["border"],
        accent_color=slide["accent"],
        title=slide["title"],
        subtitle=slide["subtitle"],
        content=slide["content"],
        footer=slide["footer"]
    )
    
    filename = f'individual-slides/slide-{slide["num"]}.html'
    with open(filename, 'w', encoding='utf-8') as f:
        f.write(html_content)
    
    print(f'✅ Created {filename}')

print('\n🎉 All 6 individual slide HTML files created!')
print('📁 Location: individual-slides/')
print('\n📸 MANUAL SCREENSHOT INSTRUCTIONS:')
print('1. Open individual-slides/slide-N.html in Chrome')
print('2. Press F12 to open DevTools')
print('3. Click device toggle icon (📱)')
print('4. Set custom size: 1080 x 1080')
print('5. Take screenshot of each slide')
print('6. Save as slide-N-final.png')
print('\n💡 Alternative methods:')
print('• Use Chrome\'s "Capture node screenshot" in DevTools')
print('• Use macOS: Cmd+Shift+4 to select slide area')
print('• Use any screenshot tool with precise selection')
print('\n🎨 Features:')
print('• Perfect 1080x1080 LinkedIn carousel format')
print('• Retro pixel-art aesthetic with consistent branding')
print('• Each slide has distinct accent color')
print('• Professional typography and layout')
print('• Ready for direct LinkedIn upload')