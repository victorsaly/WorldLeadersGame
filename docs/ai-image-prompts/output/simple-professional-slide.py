#!/usr/bin/env python3
import os

# Create directory
os.makedirs("professional-slides", exist_ok=True)

# Slide 1
html1 = '''<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>LinkedIn Carousel - Slide 1</title>
    <style>
        @import url('https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&family=Poppins:wght@400;500;600;700&display=swap');
        
        * {
            margin: 0;
            padding: 0;
            box-sizing: border-box;
        }

        body {
            font-family: 'Inter', -apple-system, BlinkMacSystemFont, sans-serif;
            background: #f5f5f5;
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
            background: #FFFFFF;
            border-radius: 12px;
            box-shadow: 0 8px 32px rgba(0,0,0,0.12);
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }

        .slide-header {
            background: #0077B5;
            color: white;
            padding: 60px 60px 40px 60px;
            text-align: left;
            position: relative;
        }

        .slide-number {
            position: absolute;
            top: 20px;
            right: 20px;
            background: rgba(255,255,255,0.2);
            color: white;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 600;
        }

        .slide-title {
            font-family: 'Poppins', sans-serif;
            font-size: 36px;
            font-weight: 700;
            line-height: 1.2;
            margin-bottom: 12px;
        }

        .slide-subtitle {
            font-size: 18px;
            opacity: 0.9;
            font-weight: 400;
            line-height: 1.4;
        }

        .slide-content {
            flex: 1;
            padding: 60px;
            display: flex;
            flex-direction: column;
            justify-content: center;
            text-align: center;
        }

        .main-text {
            font-family: 'Poppins', sans-serif;
            font-size: 28px;
            font-weight: 600;
            color: #1a1a1a;
            text-align: center;
            margin: 0 0 40px 0;
            line-height: 1.3;
        }

        .hero-icon {
            font-size: 80px;
            margin-bottom: 40px;
        }

        .hero-stats {
            display: grid;
            grid-template-columns: 1fr 1fr;
            gap: 40px;
            max-width: 400px;
            margin: 0 auto;
        }

        .stat {
            text-align: center;
            background: #f8f9fa;
            border-radius: 12px;
            padding: 30px;
            border: 2px solid #e9ecef;
        }

        .stat-number {
            font-family: 'Poppins', sans-serif;
            font-size: 48px;
            font-weight: 700;
            color: #0077B5;
            line-height: 1;
        }

        .stat-label {
            font-size: 16px;
            color: #666;
            margin-top: 8px;
            font-weight: 500;
        }

        .slide-footer {
            background: #1a1a1a;
            color: white;
            padding: 25px 60px;
            text-align: center;
            font-size: 16px;
            font-weight: 500;
        }
    </style>
</head>
<body>
    <div class="carousel-slide">
        <div class="slide-header">
            <div class="slide-number">1/6</div>
            <h1 class="slide-title">AI-Driven Development Day 2025</h1>
            <p class="slide-subtitle">Key insights from industry experts</p>
        </div>
        <div class="slide-content">
            <div class="main-text">The Future of Development is Here</div>
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
        <div class="slide-footer">Conference Highlights • September 2025</div>
    </div>
</body>
</html>'''

with open('professional-slides/slide-1.html', 'w', encoding='utf-8') as f:
    f.write(html1)

print("✅ Created professional-slides/slide-1.html")
print("🎉 Professional LinkedIn carousel slide 1 created!")
print("📁 Location: professional-slides/")
print("📸 Open in Chrome → F12 → Device Toggle → 1080x1080 → Screenshot")
print("💡 Clean, modern LinkedIn style with professional fonts and colors")