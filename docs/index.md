---
layout: home
title: "World Leaders Game"
---

<div class="hero-section">
  <h1>🎮 World Leaders Game</h1>
  <p>AI-first educational game development: Father-son experiment with <strong>85% AI autonomy</strong></p>
  <div class="hero-badge">
    Week 6 • Retro 32-Bit Transformation • 12-Year-Old Creative Director
  </div>
</div>

---

## 📊 **Live Project Stats**

<div class="stats-grid">
  <div class="stat-card">
    <div class="stat-icon">🤖</div>
    <div class="stat-number">85%</div>
    <div class="stat-label">AI Autonomy</div>
    <div class="stat-description">Architecture, coding, documentation</div>
  </div>
  
  <div class="stat-card">
    <div class="stat-icon">⚡</div>
    <div class="stat-number">10x</div>
    <div class="stat-label">Development Speed</div>
    <div class="stat-description">Faster than traditional methods</div>
  </div>
  
  <div class="stat-card">
    <div class="stat-icon">🎯</div>
    <div class="stat-number">100%</div>
    <div class="stat-label">Educational Focus</div>
    <div class="stat-description">Age-appropriate for 12-year-olds</div>
  </div>
  
  <div class="stat-card">
    <div class="stat-icon">🛡️</div>
    <div class="stat-number">5</div>
    <div class="stat-label">Safety Layers</div>
    <div class="stat-description">Child protection validation</div>
  </div>
</div>

---

## 📰 **Latest Updates**

<div class="recent-posts">
  {% for post in site.posts limit:3 %}
  <article class="post-preview-card">
    <div class="post-meta">
      <span class="post-date">{{ post.date | date: "%b %d" }}</span>
      {% if post.categories %}
        <span class="post-category">{{ post.categories[0] | capitalize }}</span>
      {% endif %}
    </div>
    <h3 class="post-title"><a href="{{ post.url | relative_url }}">{{ post.title }}</a></h3>
    <p class="post-excerpt">{{ post.excerpt | strip_html | truncate: 140 }}</p>
    <a href="{{ post.url | relative_url }}" class="read-more-btn">Read More →</a>
  </article>
  {% endfor %}
</div>

<div class="text-center">
  <a href="{{ '/post/' | relative_url }}" class="btn-primary">📖 View All Blog Posts</a>
</div>

---

## 🎨 **What We're Building**

<!-- Bullet point fix v3.0 - Force refresh -->
<div class="project-overview">
  <div class="overview-content">
    <p class="overview-description">Educational strategy game where <strong>12-year-olds</strong> learn geography, economics, and languages through:</p>
    
    <div class="features-grid">
      <div class="feature-item">
        <div class="feature-icon">🎲</div>
        <div class="feature-content">
          <h4>Dice-based Career Progression</h4>
          <p>Peasant → World Leader journey</p>
        </div>
      </div>
      
      <div class="feature-item">
        <div class="feature-icon">🌍</div>
        <div class="feature-content">
          <h4>Real-world Territory Acquisition</h4>
          <p>Using actual GDP data</p>
        </div>
      </div>
      
      <div class="feature-item">
        <div class="feature-icon">🤖</div>
        <div class="feature-content">
          <h4>AI Tutoring Agents</h4>
          <p>With child safety validation</p>
        </div>
      </div>
      
      <div class="feature-item">
        <div class="feature-icon">🗣️</div>
        <div class="feature-content">
          <h4>Language Learning</h4>
          <p>Via speech recognition</p>
        </div>
      </div>
    </div>    <div class="current-focus">
      <strong>Current Focus:</strong> <span class="focus-highlight">Retro pixel art transformation</span> inspired by our 12-year-old designer!
    </div>
  </div>
</div>

---

## 🚀 **Quick Navigation**

<div class="nav-grid">
  <div class="nav-card enhanced">
    <div class="nav-icon">📅</div>
    <h3>Development Journey</h3>
    <p>Week-by-week AI collaboration insights and progress tracking</p>
    <a href="{{ '/journey/' | relative_url }}" class="nav-btn">Explore Journey →</a>
  </div>
  
  <div class="nav-card enhanced">
    <div class="nav-icon">🔧</div>
    <h3>Technical Documentation</h3>
    <p>Implementation guides, patterns, and architectural decisions</p>
    <a href="{{ '/technical-docs/' | relative_url }}" class="nav-btn">Read Docs →</a>
  </div>
  
  <div class="nav-card enhanced">
    <div class="nav-icon">🏆</div>
    <h3>Project Milestones</h3>
    <p>Achievement tracking, metrics, and milestone summaries</p>
    <a href="{{ '/milestones/' | relative_url }}" class="nav-btn">Track Progress →</a>
  </div>
  
  <div class="nav-card enhanced">
    <div class="nav-icon">🐛</div>
    <h3>Development Issues</h3>
    <p>Systematic planning and structured problem-solving</p>
    <a href="{{ '/issues/' | relative_url }}" class="nav-btn">Monitor Issues →</a>
  </div>
</div>

---

## 🌟 **Why This Matters**

<div class="importance-section">
  <div class="importance-header">
    <h3>Revolutionary AI-First Development</h3>
    <p>This project demonstrates the future of educational software creation through human-AI collaboration.</p>
  </div>
  
  <div class="benefits-grid">
    <div class="benefit-item">
      <div class="benefit-icon">🤖</div>
      <h4>High AI Autonomy</h4>
      <p>85% AI-generated architecture, code, and documentation with strategic human guidance</p>
    </div>
    
    <div class="benefit-item">
      <div class="benefit-icon">👶</div>
      <h4>Child-Centered Design</h4>
      <p>Authentic 12-year-old feedback driving design decisions and creative direction</p>
    </div>
    
    <div class="benefit-item">
      <div class="benefit-icon">📚</div>
      <h4>Complete Methodology</h4>
      <p>Comprehensive documentation enabling replication by other educational teams</p>
    </div>
    
    <div class="benefit-item">
      <div class="benefit-icon">🎓</div>
      <h4>Real Learning Outcomes</h4>
      <p>Measurable educational impact for geography, economics, and language learning</p>
    </div>
  </div>
  
  <div class="audience-note">
    <strong>Perfect for:</strong> Educators, parents, AI researchers, and developers exploring human-AI collaboration in educational technology.
  </div>
</div>

---

<style>
/* Enhanced landing page styles matching blog design */
.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 1.5rem;
  margin: 2rem 0;
}

.stat-card {
  background: var(--card);
  border: 1px solid var(--border);
  border-radius: calc(var(--radius) + 4px);
  padding: 2rem 1.5rem;
  text-align: center;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
}

.stat-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 25px rgba(0, 0, 0, 0.15);
  border-color: var(--primary);
}

.stat-icon {
  font-size: 2.5rem;
  margin-bottom: 1rem;
  display: block;
}

.stat-number {
  font-size: 2.5rem;
  font-weight: 800;
  color: var(--primary);
  margin-bottom: 0.5rem;
  line-height: 1;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.stat-label {
  font-weight: 700;
  color: var(--foreground);
  margin-bottom: 0.5rem;
  font-size: 1rem;
}

.stat-description {
  font-size: 0.875rem;
  color: var(--muted-foreground);
  line-height: 1.4;
}

.recent-posts {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 2rem;
  margin: 2rem 0;
}

.post-preview-card {
  background: var(--card);
  border: 1px solid var(--border);
  border-radius: calc(var(--radius) + 4px);
  padding: 2rem;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  position: relative;
  overflow: hidden;
}

.post-preview-card::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(90deg, transparent, rgba(0,0,0,0.05), transparent);
  transition: left 0.5s ease;
}

.post-preview-card:hover::before {
  left: 100%;
}

.post-preview-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.15);
  border-color: var(--primary);
}

.post-meta {
  display: flex;
  gap: 1rem;
  margin-bottom: 1rem;
  font-size: 0.875rem;
  align-items: center;
}

.post-date {
  color: var(--muted-foreground);
  font-weight: 500;
}

.post-category {
  background: var(--primary);
  color: var(--primary-foreground);
  padding: 0.25rem 0.75rem;
  border-radius: 1rem;
  font-weight: 600;
  font-size: 0.75rem;
  text-transform: uppercase;
  letter-spacing: 0.05em;
}

.post-title {
  margin: 0 0 1rem 0;
  font-size: 1.375rem;
  font-weight: 700;
  line-height: 1.3;
}

.post-title a {
  color: var(--foreground);
  text-decoration: none;
  transition: all 0.3s ease;
}

.post-title a:hover {
  color: var(--primary);
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.post-excerpt {
  color: var(--muted-foreground);
  line-height: 1.6;
  margin-bottom: 1.5rem;
}

.read-more-btn {
  color: var(--primary);
  text-decoration: none;
  font-weight: 600;
  font-size: 0.875rem;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 0.25rem;
}

.read-more-btn:hover {
  color: var(--primary);
  transform: translateX(3px);
  text-decoration: none;
}

.project-overview {
  background: var(--card);
  border: 1px solid var(--border);
  border-radius: calc(var(--radius) + 6px);
  padding: 2.5rem;
  margin: 2rem 0;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
}

.overview-description {
  font-size: 1.125rem;
  color: var(--foreground);
  margin-bottom: 2rem;
  text-align: center;
  line-height: 1.6;
}

.features-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 1.5rem;
  margin: 2rem 0;
  list-style: none;
  padding: 0;
}

.features-grid::before {
  content: none;
}

.feature-item {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  padding: 1.5rem;
  background: var(--background);
  border: 1px solid var(--border);
  border-radius: var(--radius);
  transition: all 0.3s ease;
  list-style: none;
  list-style-type: none;
}

.feature-item::before {
  content: none;
}

.feature-item:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
  border-color: var(--primary);
}

/* Remove bullet points from all lists within features section */
.features ul,
.features ol,
.features li {
  list-style: none !important;
  margin: 0;
  padding: 0;
}

.features ul::before,
.features ol::before,
.features li::before {
  content: none !important;
}

/* Remove any list styling from feature grid and items */
.features .features-grid,
.features .features-grid li,
.features .feature-item {
  list-style: none !important;
  list-style-type: none !important;
}

.features .features-grid::before,
.features .features-grid li::before,
.features .feature-item::before {
  content: none !important;
  display: none !important;
}

/* NUCLEAR OPTION: Remove ALL list styling from feature items */
.project-overview .features-grid .feature-item,
.project-overview .features-grid .feature-item *,
div.feature-item,
div.feature-item * {
  list-style: none !important;
  list-style-type: none !important;
  list-style-image: none !important;
  list-style-position: outside !important;
  margin-left: 0 !important;
  padding-left: 0 !important;
  text-indent: 0 !important;
}

.project-overview .features-grid .feature-item::before,
.project-overview .features-grid .feature-item *::before,
div.feature-item::before,
div.feature-item *::before {
  content: none !important;
  display: none !important;
}

/* Force display properties */
.project-overview .features-grid {
  display: grid !important;
  list-style: none !important;
}

.project-overview .features-grid .feature-item {
  display: flex !important;
  list-style: none !important;
}

/* Global override for the entire project-overview section */
.project-overview,
.project-overview *,
.project-overview div,
.project-overview ul,
.project-overview ol,
.project-overview li {
  list-style: none !important;
  list-style-type: none !important;
  padding-left: 0 !important;
}

.project-overview *::before,
.project-overview div::before,
.project-overview ul::before,
.project-overview ol::before,
.project-overview li::before {
  content: none !important;
  display: none !important;
}

.feature-icon {
  font-size: 2rem;
  flex-shrink: 0;
}

.feature-content h4 {
  margin: 0 0 0.5rem 0;
  font-size: 1rem;
  font-weight: 600;
  color: var(--foreground);
}

.feature-content p {
  margin: 0;
  font-size: 0.875rem;
  color: var(--muted-foreground);
}

.current-focus {
  text-align: center;
  margin-top: 2rem;
  padding: 1rem;
  background: linear-gradient(135deg, rgba(0,0,0,0.05), rgba(0,0,0,0.02));
  border-radius: var(--radius);
  border: 1px solid var(--border);
}

.focus-highlight {
  color: var(--primary);
  font-weight: 600;
}

.nav-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(280px, 1fr));
  gap: 2rem;
  margin: 2rem 0;
}

.nav-card.enhanced {
  background: var(--card);
  border: 1px solid var(--border);
  border-radius: calc(var(--radius) + 6px);
  padding: 2.5rem;
  text-align: center;
  transition: all 0.3s ease;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.05);
  position: relative;
  overflow: hidden;
}

.nav-card.enhanced::after {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 4px;
  background: linear-gradient(90deg, var(--primary), var(--accent));
  transform: translateX(-100%);
  transition: transform 0.3s ease;
}

.nav-card.enhanced:hover::after {
  transform: translateX(0);
}

.nav-card.enhanced:hover {
  transform: translateY(-6px);
  box-shadow: 0 12px 30px rgba(0, 0, 0, 0.15);
  border-color: var(--primary);
}

.nav-icon {
  font-size: 3rem;
  margin-bottom: 1rem;
  display: block;
}

.nav-card.enhanced h3 {
  margin: 0 0 1rem 0;
  font-size: 1.25rem;
  font-weight: 700;
  color: var(--foreground);
}

.nav-card.enhanced p {
  color: var(--muted-foreground);
  margin-bottom: 2rem;
  line-height: 1.5;
}

.nav-btn {
  background: var(--primary);
  color: var(--primary-foreground);
  padding: 0.875rem 2rem;
  border-radius: calc(var(--radius) + 2px);
  text-decoration: none;
  font-weight: 600;
  font-size: 0.875rem;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.nav-btn:hover {
  background: var(--primary);
  opacity: 0.9;
  transform: translateY(-1px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
  text-decoration: none;
  color: var(--primary-foreground);
}

.importance-section {
  background: var(--card);
  border: 1px solid var(--border);
  border-radius: calc(var(--radius) + 6px);
  padding: 3rem;
  margin: 3rem 0;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.05);
}

.importance-header {
  text-align: center;
  margin-bottom: 3rem;
}

.importance-header h3 {
  margin: 0 0 1rem 0;
  font-size: 2rem;
  font-weight: 700;
  color: var(--foreground);
}

.importance-header p {
  font-size: 1.125rem;
  color: var(--muted-foreground);
  max-width: 600px;
  margin: 0 auto;
  line-height: 1.6;
}

.benefits-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(260px, 1fr));
  gap: 2rem;
  margin: 2rem 0;
}

.benefit-item {
  padding: 2rem;
  background: var(--background);
  border: 1px solid var(--border);
  border-radius: calc(var(--radius) + 2px);
  text-align: center;
  transition: all 0.3s ease;
}

.benefit-item:hover {
  transform: translateY(-3px);
  box-shadow: 0 8px 20px rgba(0, 0, 0, 0.1);
  border-color: var(--primary);
}

.benefit-icon {
  font-size: 2.5rem;
  margin-bottom: 1rem;
  display: block;
}

.benefit-item h4 {
  margin: 0 0 1rem 0;
  font-size: 1.125rem;
  font-weight: 600;
  color: var(--foreground);
}

.benefit-item p {
  margin: 0;
  color: var(--muted-foreground);
  line-height: 1.5;
  font-size: 0.875rem;
}

.audience-note {
  text-align: center;
  margin-top: 2rem;
  padding: 1.5rem;
  background: linear-gradient(135deg, rgba(0,0,0,0.03), rgba(0,0,0,0.01));
  border-radius: var(--radius);
  border: 1px solid var(--border);
  font-size: 1rem;
  color: var(--foreground);
}

.cta-section {
  background: linear-gradient(135deg, var(--primary), var(--accent));
  color: var(--primary-foreground);
  border-radius: calc(var(--radius) + 8px);
  padding: 3rem;
  margin: 3rem 0;
  text-align: center;
  box-shadow: 0 8px 30px rgba(0, 0, 0, 0.15);
  position: relative;
  overflow: hidden;
}

.cta-section::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: radial-gradient(circle at 70% 20%, rgba(255,255,255,0.1) 0%, transparent 50%);
  pointer-events: none;
}

.cta-content {
  position: relative;
  z-index: 1;
}

.cta-section h3 {
  margin: 0 0 1rem 0;
  font-size: 1.75rem;
  font-weight: 700;
  color: white;
}

.cta-section p {
  font-size: 1.125rem;
  margin-bottom: 2rem;
  opacity: 0.95;
  max-width: 600px;
  margin-left: auto;
  margin-right: auto;
  line-height: 1.6;
  color: white;
}

.cta-section a {
  color: black;
  text-decoration: underline;
  text-decoration-color: rgba(0,0,0,0.3);
  transition: all 0.3s ease;
  font-weight: 600;
}

.cta-section a:hover {
  text-decoration-color: black;
  color: black;
}

.cta-buttons {
  display: flex;
  gap: 1rem;
  justify-content: center;
  flex-wrap: wrap;
}

.btn-primary, .btn-secondary {
  padding: 1rem 2rem;
  border-radius: calc(var(--radius) + 4px);
  text-decoration: none;
  font-weight: 600;
  font-size: 1rem;
  transition: all 0.3s ease;
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.btn-primary.large, .btn-secondary.large {
  padding: 1.25rem 2.5rem;
  font-size: 1.125rem;
}

.btn-primary {
  background: white;
  color: var(--primary);
  border: 2px solid white;
}

.btn-primary:hover {
  background: transparent;
  color: white;
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.3);
  text-decoration: none;
}

.btn-secondary {
  background: transparent;
  color: white;
  border: 2px solid rgba(255,255,255,0.5);
}

.btn-secondary:hover {
  background: white;
  color: var(--primary);
  border-color: white;
  transform: translateY(-2px);
  box-shadow: 0 6px 20px rgba(0, 0, 0, 0.3);
  text-decoration: none;
}

.text-center {
  text-align: center;
  margin: 2rem 0;
}

/* Mobile responsiveness */
@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
  }
  
  .stat-card {
    padding: 1.5rem 1rem;
  }
  
  .stat-number {
    font-size: 2rem;
  }
  
  .recent-posts {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
  
  .features-grid {
    grid-template-columns: 1fr;
    gap: 1rem;
  }
  
  .nav-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
  
  .benefits-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
  
  .cta-buttons {
    flex-direction: column;
    align-items: center;
  }
  
  .btn-primary.large, .btn-secondary.large {
    width: 100%;
    max-width: 280px;
    justify-content: center;
  }
  
  .project-overview,
  .importance-section,
  .cta-section {
    padding: 2rem 1.5rem;
  }
}

@media (max-width: 480px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }
  
  .stat-card {
    padding: 1.25rem;
  }
}
</style>

## 📊 **Live Project Stats**

<div class="stats-grid">
  <div class="stat-card">
    <h3>🤖 AI Autonomy</h3>
    <div class="stat-number">85%</div>
    <p>Architecture, coding, documentation</p>
  </div>
  
  <div class="stat-card">
    <h3>⏱️ Development Speed</h3>
    <div class="stat-number">10x</div>
    <p>Faster than traditional methods</p>
  </div>
  
  <div class="stat-card">
    <h3>🎯 Educational Focus</h3>
    <div class="stat-number">100%</div>
    <p>Age-appropriate for 12-year-olds</p>
  </div>
  
  <div class="stat-card">
    <h3>�️ Child Safety</h3>
    <div class="stat-number">5</div>
    <p>Content validation layers</p>
  </div>
</div>

---

## 🎨 **What We're Building**

Educational strategy game where **12-year-olds** learn geography, economics, and languages through:

- **🎲 Dice-based career progression** (Peasant → World Leader)
- **🌍 Real-world territory acquisition** using GDP data
- **🤖 AI tutoring agents** with child safety validation
- **🗣️ Language learning** via speech recognition

**Current Focus**: **Retro pixel art transformation** inspired by our 12-year-old designer!

---

## 🌟 **Why This Matters**

**Revolutionary AI-First Development**: This project demonstrates:

- **🤖 High AI autonomy** in educational software development
- **👶 Child-centered design** with authentic user feedback
- **📚 Complete methodology** documentation for replication
- **🎓 Real learning outcomes** for 12-year-old players

**Perfect for**: Educators, parents, AI researchers, and developers exploring human-AI collaboration.

---
