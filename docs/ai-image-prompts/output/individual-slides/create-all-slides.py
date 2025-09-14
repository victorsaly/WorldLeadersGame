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
