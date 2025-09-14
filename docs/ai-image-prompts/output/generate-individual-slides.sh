#!/bin/bash
# Generate Individual Slide HTML Files
# Creates separate HTML files for each slide for easy screenshotting

echo "🎨 Creating individual slide HTML files..."

# Create output directory for individual slides
mkdir -p individual-slides

# Base HTML template for single slides
cat > individual-slides/slide-template.html << 'EOF'
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>SLIDE_TITLE</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Press+Start+2P&family=Orbitron:wght@400;700;900&display=swap');
        
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Orbitron', monospace;
            background: #1a1a1a;
            display: flex;
            justify-content: center;
            align-items: center;
            min-height: 100vh;
            padding: 20px;
        }

        .carousel-slide {
            width: 1080px;
            height: 1080px;
            position: relative;
            border: 4px solid BORDER_COLOR;
            box-shadow: 8px 8px 0 #000;
            image-rendering: pixelated;
            background: linear-gradient(135deg, #f8f9fa 0%, #e9ecef 100%);
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }

        .slide-header {
            background: linear-gradient(135deg, #1f2937 0%, #374151 100%);
            color: white;
            padding: 40px;
            text-align: center;
            position: relative;
        }

        .slide-header::after {
            content: '';
            position: absolute;
            bottom: 0;
            left: 0;
            right: 0;
            height: 4px;
            background: ACCENT_COLOR;
        }

        .slide-title {
            font-family: 'Press Start 2P', monospace;
            font-size: 24px;
            line-height: 1.4;
            text-shadow: 2px 2px 0 #000;
            margin-bottom: 20px;
        }

        .slide-subtitle {
            font-size: 16px;
            opacity: 0.9;
            font-weight: 400;
        }

        .slide-content {
            flex: 1;
            padding: 60px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            align-items: center;
            text-align: center;
        }

        .main-visual {
            font-size: 120px;
            margin: 40px 0;
            filter: drop-shadow(4px 4px 0 rgba(0,0,0,0.3));
        }

        .highlight-box {
            background: white;
            border: 4px solid #000;
            padding: 30px;
            margin: 20px 0;
            box-shadow: 4px 4px 0 rgba(0,0,0,0.2);
            min-width: 80%;
        }

        .highlight-text {
            font-family: 'Press Start 2P', monospace;
            font-size: 20px;
            color: #1f2937;
            line-height: 1.6;
        }

        .metrics {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 30px;
            width: 100%;
            margin: 40px 0;
        }

        .metric-box {
            background: white;
            border: 4px solid #000;
            padding: 30px;
            text-align: center;
            box-shadow: 4px 4px 0 rgba(0,0,0,0.2);
        }

        .metric-value {
            font-family: 'Press Start 2P', monospace;
            font-size: 36px;
            color: ACCENT_COLOR;
            margin-bottom: 15px;
        }

        .metric-label {
            font-size: 14px;
            font-weight: 700;
            color: #374151;
            text-transform: uppercase;
        }

        .speaker-grid {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 20px;
            width: 100%;
            margin: 30px 0;
        }

        .speaker-card {
            background: white;
            border: 3px solid #000;
            padding: 20px;
            text-align: center;
            box-shadow: 3px 3px 0 rgba(0,0,0,0.2);
        }

        .speaker-name {
            font-family: 'Press Start 2P', monospace;
            font-size: 14px;
            color: #F59E0B;
            margin-bottom: 10px;
        }

        .speaker-topic {
            font-size: 12px;
            color: #374151;
            line-height: 1.4;
        }

        .framework-steps {
            width: 100%;
            margin: 30px 0;
        }

        .framework-step {
            display: flex;
            align-items: center;
            background: white;
            border: 3px solid #000;
            margin: 20px 0;
            padding: 25px;
            box-shadow: 3px 3px 0 rgba(0,0,0,0.2);
        }

        .step-number {
            font-family: 'Press Start 2P', monospace;
            font-size: 32px;
            color: ACCENT_COLOR;
            margin-right: 30px;
            min-width: 60px;
        }

        .step-content h3 {
            font-family: 'Press Start 2P', monospace;
            font-size: 16px;
            color: #1f2937;
            margin-bottom: 10px;
        }

        .step-content p {
            font-size: 14px;
            color: #374151;
            line-height: 1.4;
        }

        .slide-footer {
            background: #1f2937;
            color: white;
            padding: 20px;
            text-align: center;
            font-size: 14px;
            font-weight: 700;
        }

        .slide-number {
            position: absolute;
            top: 20px;
            right: 20px;
            background: rgba(0,0,0,0.8);
            color: white;
            padding: 10px 15px;
            font-family: 'Press Start 2P', monospace;
            font-size: 12px;
            border: 2px solid #fff;
        }
    </style>
</head>
<body>
SLIDE_CONTENT
</body>
</html>
EOF

# Create Slide 1
echo "📄 Creating slide 1..."
sed 's/SLIDE_TITLE/LinkedIn Carousel - Slide 1/g; s/BORDER_COLOR/#3B82F6/g; s/ACCENT_COLOR/#3B82F6/g' individual-slides/slide-template.html > individual-slides/temp-slide-1.html

cat >> individual-slides/temp-slide-1.html << 'EOF'
<div class="carousel-slide">
    <div class="slide-number">1/6</div>
    <div class="slide-header">
        <h1 class="slide-title">AI-DRIVEN DEVELOPMENT DAY 2025</h1>
        <p class="slide-subtitle">Game-Changing Insights for Modern Developers</p>
    </div>
    <div class="slide-content">
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
    </div>
    <div class="slide-footer">Conference Insights • September 2025</div>
</div>
EOF

# Replace placeholder with actual content for slide 1
sed 's/SLIDE_CONTENT//g' individual-slides/temp-slide-1.html > individual-slides/slide-1.html
rm individual-slides/temp-slide-1.html

# Create the remaining slides...
echo "📄 Creating remaining slides..."

# I'll create a simple Python script to generate all slides
cat > individual-slides/create-all-slides.py << 'EOF'
#!/usr/bin/env python3

import os

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

# Read template
with open('slide-template.html', 'r') as f:
    template = f.read()

# Generate each slide
for slide in slides_data:
    html_content = template.replace('SLIDE_TITLE', f'LinkedIn Carousel - Slide {slide["num"]}')
    html_content = html_content.replace('BORDER_COLOR', slide['border'])
    html_content = html_content.replace('ACCENT_COLOR', slide['accent'])
    
    slide_html = f'''
    <div class="carousel-slide">
        <div class="slide-number">{slide["num"]}/6</div>
        <div class="slide-header">
            <h1 class="slide-title">{slide["title"]}</h1>
            <p class="slide-subtitle">{slide["subtitle"]}</p>
        </div>
        <div class="slide-content">
            {slide["content"]}
        </div>
        <div class="slide-footer">{slide["footer"]}</div>
    </div>
    '''
    
    html_content = html_content.replace('SLIDE_CONTENT', slide_html)
    
    with open(f'slide-{slide["num"]}.html', 'w') as f:
        f.write(html_content)
    
    print(f'✅ Created slide-{slide["num"]}.html')

print('\n🎉 All 6 individual slide HTML files created!')
print('📸 Open each file in Chrome and screenshot at 1080x1080 resolution')
EOF

python3 individual-slides/create-all-slides.py

echo ""
echo "🎉 HTML Carousel Generation Complete!"
echo "=" * 50
echo "📁 Files created:"
echo "   • linkedin-carousel-html.html (full carousel)"
echo "   • individual-slides/slide-1.html through slide-6.html"
echo ""
echo "📸 SCREENSHOT INSTRUCTIONS:"
echo "1. Open each slide-N.html file in Chrome"
echo "2. Press F12, click device toggle (📱)"
echo "3. Set custom size: 1080 x 1080"
echo "4. Take screenshot of each slide"
echo "5. Save as slide-N-final.png"
echo ""
echo "💡 Alternative: Use the main carousel file for manual cropping"
echo "🎨 All slides use consistent retro branding with pixel-art aesthetic"