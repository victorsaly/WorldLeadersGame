#!/usr/bin/env python3
"""
Professional LinkedIn Carousel - Based on Maxiality Style Guide
Clean, modern, professional design following LinkedIn best practices
"""

import os

# Create professional slides directory
os.makedirs("professional-slides", exist_ok=True)

slides_data = [
    {
        "num": 1,
        "bg_color": "#FFFFFF",
        "accent_color": "#0077B5",
        "title": "AI-Driven Development Day 2025",
        "subtitle": "Key insights from industry experts",
        "main_text": "The Future of Development is Here",
        "content": '''
            <div class="hero-section">
                <div class="hero-icon">🚀</div>
                <div class="hero-stats">
                    <div class="stat">
                        <div class="stat-number">5</div>
                        <div class="stat-label">Expert Speakers</div>
                    </div>
                    <div class="stat">
                        <div class="stat-number">100+</div>
                        <div class="stat-label">Developers</div>
                    </div>
                </div>
            </div>
        ''',
        "footer": "Conference Highlights • September 2025"
    },
    {
        "num": 2,
        "bg_color": "#FFFFFF",
        "accent_color": "#0077B5",
        "title": "Meet the Speakers",
        "subtitle": "Industry leaders sharing practical insights",
        "main_text": "",
        "content": '''
            <div class="speaker-list">
                <div class="speaker-item">
                    <div class="speaker-bullet">•</div>
                    <div class="speaker-details">
                        <div class="speaker-name">Debbie O'Brien</div>
                        <div class="speaker-topic">Strategic AI Integration</div>
                    </div>
                </div>
                <div class="speaker-item">
                    <div class="speaker-bullet">•</div>
                    <div class="speaker-details">
                        <div class="speaker-name">Phil Nash</div>
                        <div class="speaker-topic">Testing with AI</div>
                    </div>
                </div>
                <div class="speaker-item">
                    <div class="speaker-bullet">•</div>
                    <div class="speaker-details">
                        <div class="speaker-name">Kent C. Dodds</div>
                        <div class="speaker-topic">Context Engineering</div>
                    </div>
                </div>
                <div class="speaker-item">
                    <div class="speaker-bullet">•</div>
                    <div class="speaker-details">
                        <div class="speaker-name">Tejas Kumar</div>
                        <div class="speaker-topic">AI-First Development</div>
                    </div>
                </div>
            </div>
        ''',
        "footer": "Real-world expertise from production environments"
    },
    {
        "num": 3,
        "bg_color": "#FFFFFF",
        "accent_color": "#0077B5",
        "title": "Strategic AI Integration",
        "subtitle": "Beyond the hype: real developer improvements",
        "main_text": "AI as a collaborative partner, not a replacement",
        "content": '''
            <div class="benefit-grid">
                <div class="benefit-card">
                    <div class="benefit-metric">3x</div>
                    <div class="benefit-desc">Faster debugging</div>
                </div>
                <div class="benefit-card">
                    <div class="benefit-metric">60%</div>
                    <div class="benefit-desc">Less boilerplate</div>
                </div>
            </div>
        ''',
        "footer": "Focus on strategic adoption for measurable results"
    },
    {
        "num": 4,
        "bg_color": "#FFFFFF",
        "accent_color": "#0077B5",
        "title": "Context Engineering",
        "subtitle": "The new discipline every developer needs",
        "main_text": ""Context is the new code architecture"",
        "content": '''
            <div class="context-benefits">
                <div class="context-item">
                    <div class="context-icon">✓</div>
                    <div class="context-text">5x better AI responses</div>
                </div>
                <div class="context-item">
                    <div class="context-icon">✓</div>
                    <div class="context-text">90% first-try success rate</div>
                </div>
                <div class="context-item">
                    <div class="context-icon">✓</div>
                    <div class="context-text">Reduced iteration cycles</div>
                </div>
            </div>
        ''',
        "footer": "Master context engineering for AI success"
    },
    {
        "num": 5,
        "bg_color": "#FFFFFF",
        "accent_color": "#FF6B35",
        "title": "The Productivity Paradox",
        "subtitle": "Reality check: the learning curve is real",
        "main_text": "Initial productivity dip before the gains",
        "content": '''
            <div class="paradox-stats">
                <div class="paradox-expected">
                    <div class="paradox-label">Expected</div>
                    <div class="paradox-number positive">+24%</div>
                    <div class="paradox-desc">Productivity increase</div>
                </div>
                <div class="paradox-reality">
                    <div class="paradox-label">Reality (initially)</div>
                    <div class="paradox-number negative">-19%</div>
                    <div class="paradox-desc">Slower at first</div>
                </div>
            </div>
        ''',
        "footer": "Strategic adoption is key to overcoming the paradox"
    },
    {
        "num": 6,
        "bg_color": "#FFFFFF",
        "accent_color": "#0077B5",
        "title": "Your Success Framework",
        "subtitle": "3 steps to AI-enhanced development",
        "main_text": "",
        "content": '''
            <div class="framework-steps">
                <div class="step-item">
                    <div class="step-number">1</div>
                    <div class="step-content">
                        <div class="step-title">Strategic Adoption</div>
                        <div class="step-desc">Choose the right tools for specific tasks</div>
                    </div>
                </div>
                <div class="step-item">
                    <div class="step-number">2</div>
                    <div class="step-content">
                        <div class="step-title">Continuous Learning</div>
                        <div class="step-desc">Develop AI literacy and context skills</div>
                    </div>
                </div>
                <div class="step-item">
                    <div class="step-number">3</div>
                    <div class="step-content">
                        <div class="step-title">Iterative Improvement</div>
                        <div class="step-desc">Refine approach based on results</div>
                    </div>
                </div>
            </div>
            <div class="cta-section">
                <div class="cta-text">Ready to start your AI journey?</div>
                <div class="cta-follow">Follow for more insights</div>
            </div>
        ''',
        "footer": "Start implementing today • Follow for more tips"
    }
]

html_template = '''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Professional LinkedIn Carousel - Slide {num}</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&family=Poppins:wght@400;500;600;700&display=swap');
        
        * {{
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }}

        body {{
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
            background: #f5f5f5;
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
            background: {bg_color};
            border-radius: 12px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.12);
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }}

        .slide-header {{
            background: {accent_color};
            color: white;
            padding: 60px 60px 40px 60px;
            text-align: left;
            position: relative;
        }}

        .slide-number {{
            position: absolute;
            top: 20px;
            right: 20px;
            background: rgba(255,255,255,0.2);
            color: white;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
        }}

        .slide-title {{
            font-family: 'Poppins', sans-serif;
            font-size: 36px;
            font-weight: 700;
            line-height: 1.2;
            margin-bottom: 12px;
        }}

        .slide-subtitle {{
            font-size: 18px;
            opacity: 0.9;
            font-weight: 400;
            line-height: 1.4;
        }}

        .slide-content {{
            flex: 1;
            padding: 60px;
            display: flex;
            flex-direction: column;
            justify-content: center;
        }}

        .main-text {{
            font-family: 'Poppins', sans-serif;
            font-size: 28px;
            font-weight: 600;
            color: #1a1a1a;
            text-align: center;
            margin: 0 0 40px 0;
            line-height: 1.3;
        }}

        .hero-section {{
            text-align: center;
        }}

        .hero-icon {{
            font-size: 80px;
            margin-bottom: 40px;
        }}

        .hero-stats {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 40px;
            max-width: 400px;
            margin: 0 auto;
        }}

        .stat {{
            text-align: center;
        }}

        .stat-number {{
            font-family: 'Poppins', sans-serif;
            font-size: 48px;
            font-weight: 700;
            color: {accent_color};
            line-height: 1;
        }}

        .stat-label {{
            font-size: 16px;
            color: #666;
            margin-top: 8px;
            font-weight: 500;
        }}

        .speaker-list {{
            max-width: 600px;
        }}

        .speaker-item {{
            display: flex;
            align-items: flex-start;
            margin-bottom: 30px;
            padding: 20px;
            background: #f8f9fa;
            border-radius: 8px;
            border-left: 4px solid {accent_color};
        }}

        .speaker-bullet {{
            color: {accent_color};
            font-size: 24px;
            font-weight: 700;
            margin-right: 20px;
            line-height: 1;
        }}

        .speaker-name {{
            font-family: 'Poppins', sans-serif;
            font-size: 20px;
            font-weight: 600;
            color: #1a1a1a;
            margin-bottom: 4px;
        }}

        .speaker-topic {{
            font-size: 16px;
            color: #666;
            line-height: 1.4;
        }}

        .benefit-grid {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 40px;
            max-width: 500px;
            margin: 0 auto;
        }}

        .benefit-card {{
            text-align: center;
            padding: 40px 20px;
            background: #f8f9fa;
            border-radius: 12px;
            border: 2px solid #e9ecef;
        }}

        .benefit-metric {{
            font-family: 'Poppins', sans-serif;
            font-size: 48px;
            font-weight: 700;
            color: {accent_color};
            line-height: 1;
            margin-bottom: 12px;
        }}

        .benefit-desc {{
            font-size: 18px;
            color: #1a1a1a;
            font-weight: 500;
        }}

        .context-benefits {{
            max-width: 500px;
            margin: 0 auto;
        }}

        .context-item {{
            display: flex;
            align-items: center;
            margin-bottom: 25px;
            padding: 20px;
            background: #f8f9fa;
            border-radius: 8px;
        }}

        .context-icon {{
            color: #22c55e;
            font-size: 24px;
            font-weight: 700;
            margin-right: 20px;
            min-width: 30px;
        }}

        .context-text {{
            font-size: 18px;
            color: #1a1a1a;
            font-weight: 500;
        }}

        .paradox-stats {{
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 40px;
            max-width: 600px;
            margin: 0 auto;
        }}

        .paradox-expected, .paradox-reality {{
            text-align: center;
            padding: 30px;
            border-radius: 12px;
            background: #f8f9fa;
        }}

        .paradox-label {{
            font-size: 14px;
            color: #666;
            text-transform: uppercase;
            font-weight: 600;
            margin-bottom: 15px;
            letter-spacing: 0.5px;
        }}

        .paradox-number {{
            font-family: 'Poppins', sans-serif;
            font-size: 42px;
            font-weight: 700;
            line-height: 1;
            margin-bottom: 10px;
        }}

        .paradox-number.positive {{
            color: #22c55e;
        }}

        .paradox-number.negative {{
            color: #ef4444;
        }}

        .paradox-desc {{
            font-size: 16px;
            color: #1a1a1a;
            font-weight: 500;
        }}

        .framework-steps {{
            max-width: 600px;
            margin-bottom: 40px;
        }}

        .step-item {{
            display: flex;
            align-items: flex-start;
            margin-bottom: 30px;
            padding: 25px;
            background: #f8f9fa;
            border-radius: 12px;
            border-left: 4px solid {accent_color};
        }}

        .step-number {{
            background: {accent_color};
            color: white;
            width: 40px;
            height: 40px;
            border-radius: 50%;
            display: flex;
            align-items: center;
            justify-content: center;
            font-family: 'Poppins', sans-serif;
            font-size: 18px;
            font-weight: 700;
            margin-right: 20px;
            flex-shrink: 0;
        }}

        .step-title {{
            font-family: 'Poppins', sans-serif;
            font-size: 20px;
            font-weight: 600;
            color: #1a1a1a;
            margin-bottom: 6px;
        }}

        .step-desc {{
            font-size: 16px;
            color: #666;
            line-height: 1.4;
        }}

        .cta-section {{
            text-align: center;
            padding: 30px;
            background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
            border-radius: 12px;
            margin-top: 20px;
        }}

        .cta-text {{
            font-family: 'Poppins', sans-serif;
            font-size: 22px;
            font-weight: 600;
            color: #1a1a1a;
            margin-bottom: 8px;
        }}

        .cta-follow {{
            font-size: 16px;
            color: {accent_color};
            font-weight: 600;
        }}

        .slide-footer {{
            background: #1a1a1a;
            color: white;
            padding: 25px 60px;
            text-align: center;
            font-size: 16px;
            font-weight: 500;
        }}
    </style>
</head>
<body>
    <div class="carousel-slide">
        <div class="slide-header">
            <div class="slide-number">{num}/6</div>
            <h1 class="slide-title">{title}</h1>
            <p class="slide-subtitle">{subtitle}</p>
        </div>
        <div class="slide-content">
            {main_text_html}
            {content}
        </div>
        <div class="slide-footer">{footer}</div>
    </div>
</body>
</html>'''

# Generate each slide
print("🎨 Creating professional LinkedIn carousel slides...")

for slide in slides_data:
    main_text_html = f'<div class="main-text">{slide["main_text"]}</div>' if slide["main_text"] else ''
    
    html_content = html_template.format(
        num=slide["num"],
        bg_color=slide["bg_color"],
        accent_color=slide["accent_color"],
        title=slide["title"],
        subtitle=slide["subtitle"],
        main_text_html=main_text_html,
        content=slide["content"],
        footer=slide["footer"]
    )
    
    filename = f'professional-slides/slide-{slide["num"]}.html'
    with open(filename, 'w', encoding='utf-8') as f:
        f.write(html_content)
    
    print(f'✅ Created {filename}')

print('\n🎉 Professional LinkedIn carousel slides created!')
print('📁 Location: professional-slides/')
print('\n📸 SCREENSHOT INSTRUCTIONS:')
print('1. Open professional-slides/slide-N.html in Chrome')
print('2. Press F12 to open DevTools')
print('3. Click device toggle icon (📱)')
print('4. Set custom size: 1080 x 1080')
print('5. Right-click on slide → "Capture node screenshot"')
print('6. Or use Cmd+Shift+4 on macOS for precise selection')
print('\n🎨 Professional Features:')
print('• Clean, modern LinkedIn-style design')
print('• Inter + Poppins professional fonts')
print('• LinkedIn blue (#0077B5) primary accent')
print('• Minimal, scannable content')
print('• Mobile-optimized typography')
print('• Professional color scheme')
print('• Clean visual hierarchy')
print('\n💡 Style matches successful LinkedIn carousels:')
print('• High contrast for mobile readability')
print('• Consistent branding throughout')
print('• Clear call-to-action on final slide')
print('• Professional visual elements')
print('• Easy-to-digest information chunks')