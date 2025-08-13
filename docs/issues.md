---
layout: page
title: "AI-Generated Issues"
subtitle: "Systematic Problem-Solving for Educational Game Development"
date: 2025-08-03
permalink: /issues/
---

# AI-Generated Issues 🎯

**AI-Driven Problem Solving**: Systematic issue generation and resolution for our educational game development project.

---

## 🚀 Active Issues

### Week 8: Advanced Educational Features & Performance (Current Focus) 🚀

**Target Date**: August 20, 2025 • **Duration**: 1 week • **Focus**: Advanced AI agent personalities, cultural sensitivity enhancement, learning analytics, and performance optimization

**AI-Led Excellence**: Building sophisticated educational features with advanced learning adaptation, cultural representation, assessment systems, and production-scale performance optimization.

<div class="issues-grid">
  {% assign week8_issues = site.issues | where: "week", 8 | sort: "issue_number" %}
  {% for issue in week8_issues %}
    <div class="issue-card shadcn-card week8-issue">
      <div class="issue-header">
        <h3><a href="{{ issue.url | relative_url }}">{{ issue.title }}</a></h3>
        <div class="issue-meta">
          <span class="issue-number">{{ issue.issue_number }}</span>
          <span class="priority priority-{{ issue.priority }}">{{ issue.priority | upcase }}</span>
          <span class="effort">{{ issue.estimated_hours }}h</span>
          <span class="ai-autonomy">🤖 {{ issue.ai_autonomy_target }}</span>
          <span class="advanced-features">🎯 ADVANCED</span>
        </div>
      </div>
      <div class="educational-focus">
        <strong>Advanced Focus:</strong>
        {% for focus in issue.production_focus %}
          <span class="advanced-tag">{{ focus }}</span>
        {% endfor %}
      </div>
      <div class="issue-description">
        <p>{{ issue.excerpt | strip_html | truncatewords: 20 }}</p>
      </div>
    </div>
  {% endfor %}
</div>

---

### Week 9: Architecture & Learning Progression (Future Focus) 🏗️

**Target Date**: August 25, 2025 • **Duration**: 1 week • **Focus**: Game architecture coherence and learning progression optimization with human-AI collaboration frameworks

**Architectural Excellence**: Implementing cohesive game architecture that enhances educational progression while maintaining system scalability and human-AI collaboration effectiveness.

<div class="issues-grid">
  {% assign week9_issues = site.issues | where: "week", 9 | sort: "issue_number" %}
  {% for issue in week9_issues %}
    <div class="issue-card shadcn-card week9-issue">
      <div class="issue-header">
        <h3><a href="{{ issue.url | relative_url }}">{{ issue.title }}</a></h3>
        <div class="issue-meta">
          <span class="issue-number">{{ issue.issue_number }}</span>
          <span class="priority priority-{{ issue.priority }}">{{ issue.priority | upcase }}</span>
          <span class="effort">{{ issue.estimated_hours }}h</span>
          <span class="ai-autonomy">🤖 {{ issue.ai_autonomy_target }}</span>
          <span class="architecture-focus">🏗️ ARCHITECTURE</span>
        </div>
      </div>
      <div class="educational-focus">
        <strong>Architecture Focus:</strong>
        {% for focus in issue.production_focus %}
          <span class="architecture-tag">{{ focus }}</span>
        {% endfor %}
      </div>
      <div class="issue-description">
        <p>{{ issue.excerpt | strip_html | truncatewords: 20 }}</p>
      </div>
    </div>
  {% endfor %}
</div>

---

### Week 7: Comprehensive Testing Strategy (Current Focus) 🧪

**Target Date**: August 18, 2025 • **Duration**: 1 week • **Focus**: Complete XUnit testing framework implementation with child safety validation and educational effectiveness measurement

**Testing Excellence**: Implementing comprehensive testing strategy with 90%+ code coverage, child safety validation, and educational outcome measurement systems.

<div class="issues-grid">
  {% assign week7_issues = site.issues | where: "week", 7 | sort: "issue_number" %}
  {% for issue in week7_issues %}
    <div class="issue-card shadcn-card week7-issue">
      <div class="issue-header">
        <h3><a href="{{ issue.url | relative_url }}">{{ issue.title }}</a></h3>
        <div class="issue-meta">
          <span class="issue-number">{{ issue.issue_number }}</span>
          <span class="priority priority-{{ issue.priority }}">{{ issue.priority | upcase }}</span>
          <span class="effort">{{ issue.estimated_hours }}h</span>
          <span class="ai-autonomy">🤖 {{ issue.ai_autonomy_target }}</span>
          <span class="testing-focus">🧪 TESTING</span>
        </div>
      </div>
      <div class="educational-focus">
        <strong>Testing Focus:</strong>
        {% for focus in issue.production_focus %}
          <span class="testing-tag">{{ focus }}</span>
        {% endfor %}
      </div>
      <div class="issue-description">
        <p>{{ issue.excerpt | strip_html | truncatewords: 20 }}</p>
      </div>
    </div>
  {% endfor %}
</div>

---

### Week 6: Retro 32-Bit Transformation (Completed ✅)

**Completion Date**: August 16, 2025 • **Status**: Successfully implemented child designer vision • **AI Autonomy**: 88% (12% child design direction required)

**Child Designer Led**: This week successfully implemented the creative vision of our 12-year-old designer, transforming the game into the retro pixel art style they envisioned.

#### 🎯 Major Achievements

- ✅ **Character Persona Selection**: Privacy-protective character creation system with retro pixel art
- ✅ **Retro Design System**: Complete 32-bit aesthetic with green theme and pixel art fonts
- ✅ **Interactive World Map**: Pixel art world map with territory information and cultural context
- ✅ **Mobile-First UI/UX**: Touch-optimized interface with accessibility compliance and PWA standards
- ✅ **Child Designer Vision**: Successfully implemented 12-year-old creative direction with retro gaming aesthetics

<div class="issues-grid">
  {% assign week6_issues = site.issues | where: "week", 6 | sort: "issue_number" %}
  {% for issue in week6_issues %}
    <div class="issue-card shadcn-card week6-issue completed">
      <div class="issue-header">
        <h3><a href="{{ issue.url | relative_url }}">{{ issue.title }}</a></h3>
        <div class="issue-meta">
          <span class="issue-number">{{ issue.issue_number }}</span>
          <span class="priority priority-{{ issue.priority }}">{{ issue.priority | upcase }}</span>
          <span class="effort">{{ issue.estimated_hours }}h</span>
          <span class="ai-autonomy">🤖 {{ issue.ai_autonomy_target }}</span>
          <span class="status">✅ COMPLETE</span>
        </div>
      </div>
      <div class="educational-focus">
        <strong>Creative Focus:</strong>
        {% for focus in issue.production_focus %}
          <span class="creative-tag">{{ focus }}</span>
        {% endfor %}
      </div>
      <div class="issue-description">
        <p>{{ issue.excerpt | strip_html | truncatewords: 20 }}</p>
      </div>
    </div>
  {% endfor %}
</div>

---

### Week 5: Production Security & Scalability (Completed ✅)

**Completion Date**: August 10, 2025 • **Status**: Successfully implemented production security • **AI Autonomy**: 94% (6% human oversight required)

<div class="issues-grid">
  {% assign week5_issues = site.issues | where: "week", 5 | sort: "issue_number" %}
  {% for issue in week5_issues %}
    <div class="issue-card shadcn-card week5-issue completed">
      <div class="issue-header">
        <h3><a href="{{ issue.url | relative_url }}">{{ issue.title }}</a></h3>
        <div class="issue-meta">
          <span class="issue-number">{{ issue.issue_number }}</span>
          <span class="priority priority-{{ issue.priority }}">{{ issue.priority | upcase }}</span>
          <span class="effort">{{ issue.estimated_hours }}h</span>
          <span class="ai-autonomy">🤖 {{ issue.ai_autonomy_target }}</span>
          <span class="status">✅ COMPLETE</span>
        </div>
      </div>
      <div class="educational-focus">
        <strong>Production Focus:</strong>
        {% for focus in issue.production_focus %}
          <span class="production-tag">{{ focus }}</span>
        {% endfor %}
      </div>
      <div class="issue-description">
        <p>{{ issue.excerpt | strip_html | truncatewords: 20 }}</p>
      </div>
    </div>
  {% endfor %}
</div>

---

### Week 4: AI Integration & Real-World Data (Completed ✅ with 95% Success Rate)

**Completion Date**: August 6, 2025 • **Status**: Successfully deployed to production • **AI Autonomy**: 92% (8% human oversight required)

#### 🎯 Major Achievements

- ✅ **AI Agent Personality System**: 6 distinct educational agents with child-safe interactions
- ✅ **Territory Management**: Complete real-world data integration with World Bank GDP
- ✅ **Speech Recognition**: 12-language pronunciation system with Azure Speech Services
- ✅ **Production Deployment**: Live systems running at worldleadersgame.co.uk
- ✅ **Educational Effectiveness**: Comprehensive child-friendly learning experiences

#### 🚀 Live Production Systems

- **🌐 Frontend**: [https://worldleadersgame.co.uk/](https://worldleadersgame.co.uk/) - Interactive game interface
- **🔧 API**: [https://api.worldleadersgame.co.uk/](https://api.worldleadersgame.co.uk/) - Game backend services
- **📚 Documentation**: [https://docs.worldleadersgame.co.uk/](https://docs.worldleadersgame.co.uk/) - Project documentation and blog

#### ⚠️ Security & Scalability Challenges Identified

- **API Protection**: No authentication or rate limiting currently implemented
- **Performance**: Deployment speed needs optimization for production scale
- **Scalability**: System needs to handle 1000+ daily users efficiently
- **Cost Management**: Azure AI services need usage monitoring and throttling
- **Security**: Production-grade authentication and authorization required

<div class="issues-grid">
  {% assign week4_issues = site.issues | where: "week", 4 | sort: "issue_number" %}
  {% for issue in week4_issues %}
    <div class="issue-card shadcn-card completed">
      <div class="issue-header">
        <h3><a href="{{ issue.url | relative_url }}">{{ issue.title }}</a></h3>
        <div class="issue-meta">
          <span class="issue-number">{{ issue.issue_number }}</span>
          <span class="priority priority-{{ issue.priority }}">{{ issue.priority | upcase }}</span>
          <span class="effort">{{ issue.estimated_hours }}h</span>
          <span class="ai-autonomy">🤖 {{ issue.ai_autonomy_target }}</span>
          <span class="status">✅ COMPLETE</span>
        </div>
      </div>
      <div class="educational-focus">
        <strong>Educational Focus:</strong>
        {% for focus in issue.production_focus %}
          <span class="educational-tag">{{ focus }}</span>
        {% endfor %}
      </div>
      <div class="issue-description">
        <p>{{ issue.excerpt | strip_html | truncatewords: 20 }}</p>
      </div>
    </div>
  {% endfor %}
</div>

---

## 📊 Development Progress

### Quality Assurance

- [ ] **Clear Objectives**: Every issue has specific, measurable goals
- [ ] **Educational Value**: Issues contribute to project's educational mission
- [ ] **Implementation Ready**: Detailed phases and success criteria included
- [ ] **Child Safety**: All issues consider age-appropriate content requirements
- [ ] **AI Collaboration**: Issues designed for 95% AI autonomy execution

### Completion Tracking

---

### Week 3: Core Game Mechanics (Completed ✅ with Technical Refinements)

**Completion Date**: August 4, 2025 • **Status**: Functional with ongoing code review • **AI Autonomy**: 85% (15% human intervention required)

#### 🎯 Major Achievements

- ✅ **Game Engine Architecture**: Complete interactive dice-based career progression
- ✅ **Resource Management**: Income, reputation, happiness tracking systems
- ✅ **Child-Friendly UI**: Large buttons, animations, encouraging feedback
- ✅ **Database Integration**: PostgreSQL with Entity Framework Core
- ✅ **SignalR Real-Time**: Game state synchronization across components

#### ⚠️ Technical Challenges Encountered

- **Blazor Component Complexity**: AI-generated components required human debugging for integration
- **SignalR Hub Configuration**: Service registration conflicts needed manual resolution
- **Entity Framework Migrations**: Database schema evolution required human validation
- **Code Review Process**: Pull request review identifying complexity issues for refinement

#### 🛠️ Current Status

- **Functional Core**: Game mechanics working and tested
- **Refinement Phase**: Code review and optimization in progress
- **Learning Outcome**: Complex framework integrations benefit from human oversight
- **Next Steps**: Apply lessons learned to Week 4 AI integration issues

#### Live GitHub Issues

- **[Issue #21: Week 3 Documentation Infrastructure Overhaul](https://github.com/victorsaly/WorldLeadersGame/issues/21)** - ✅ Complete
- **[Issue #22: Comprehensive Documentation Review & Format Standardization](https://github.com/victorsaly/WorldLeadersGame/issues/22)** - ✅ Complete
- **[Issue #23: GitHub Pages Navigation & Mobile Optimization](https://github.com/victorsaly/WorldLeadersGame/issues/23)** - ✅ Complete
- **[Issue #24: Copilot Instructions Restructuring & Process Documentation](https://github.com/victorsaly/WorldLeadersGame/issues/24)** - ✅ Complete

<div class="issues-grid">
  {% assign week3_issues = site.issues | where: "milestone", "Week 3 Preparation" | sort: "title" %}
  {% for issue in week3_issues %}
    <div class="issue-card shadcn-card completed">
      <div class="issue-header">
        <h3><a href="{{ issue.url | relative_url }}">{{ issue.title }}</a></h3>
        <div class="issue-meta">
          <span class="priority priority-{{ issue.priority }}">{{ issue.priority | upcase }}</span>
          <span class="effort">{{ issue.estimated_effort }}</span>
          <span class="ai-autonomy">🤖 {{ issue.ai_autonomy }}</span>
          <span class="status">✅ COMPLETE</span>
        </div>
      </div>
      <div class="issue-tags">
        {% for tag in issue.tags %}
          <span class="tag">{{ tag }}</span>
        {% endfor %}
      </div>
    </div>
  {% endfor %}
</div>

## 📊 Issue Categories

### 🔧 Infrastructure Issues

Issues focused on project infrastructure, documentation systems, and development workflow improvements.

### 📱 User Experience Issues

Issues addressing mobile optimization, accessibility, and user interface improvements.

### 🤖 AI Collaboration Issues

Issues focused on improving AI-human collaboration, instruction optimization, and automation processes.

### 🎮 Educational Game Issues

Issues related to game mechanics, educational content, and child safety features.

## 📈 AI Issue Generation Process

### How AI Creates These Issues

1. **Problem Identification**: AI analyzes current project state and identifies improvement opportunities
2. **Systematic Breakdown**: Complex problems are decomposed into manageable, focused issues
3. **Educational Context**: Every issue includes educational objectives and child safety considerations
4. **Implementation Planning**: Detailed phases, timelines, and success criteria for each issue
5. **Quality Assurance**: Built-in validation and testing requirements

### Benefits of AI-Generated Issues

- **Comprehensive Coverage**: AI identifies issues humans might overlook
- **Consistent Structure**: Standardized format across all issues
- **Educational Focus**: Every issue maintains focus on learning outcomes
- **Scalable Process**: Issue generation scales with project complexity
- **Documentation Excellence**: Issues serve as detailed implementation guides

## 🎯 Success Metrics

### Issue Quality Standards

- [ ] **Clear Objectives**: Every issue has specific, measurable goals
- [ ] **Educational Value**: Issues contribute to project's educational mission
- [ ] **Implementation Ready**: Detailed phases and success criteria included
- [ ] **Child Safety**: All issues consider age-appropriate content requirements
- [ ] **AI Collaboration**: Issues designed for 95% AI autonomy execution

### Completion Tracking

#### Week 9 Progress (Future - Architecture & Learning Progression)

{% assign issue_9_1 = site.issues | where: "issue_number", "9.1" | first %}

- **Total Week 9 Issues**: {{ site.issues | where: "week", 9 | size }}
- **Game Architecture Coherence & Learning Progression**: {{ issue_9_1.status | default: "planned" }}

#### Week 8 Progress (Current - Advanced Features)

{% assign issue_8_1 = site.issues | where: "issue_number", "8.1" | first %}
{% assign issue_8_2 = site.issues | where: "issue_number", "8.2" | first %}
{% assign issue_8_3 = site.issues | where: "issue_number", "8.3" | first %}
{% assign issue_8_4 = site.issues | where: "issue_number", "8.4" | first %}

- **Total Week 8 Issues**: {{ site.issues | where: "week", 8 | size }}
- **Advanced AI Agent Personalities**: {{ issue_8_1.status | default: "planned" }}
- **Cultural Sensitivity Enhancement**: {{ issue_8_2.status | default: "planned" }}
- **Advanced Assessment & Analytics**: {{ issue_8_3.status | default: "planned" }}
- **Performance Optimization**: {{ issue_8_4.status | default: "planned" }}

#### Week 7 Progress (Current - Testing Framework)

{% assign issue_7 = site.issues | where: "issue_number", "7" | first %}

- **Total Week 7 Issues**: {{ site.issues | where: "week", 7 | size }}
- **Comprehensive Testing Strategy**: {{ issue_7.status | default: "planned" }}

#### Week 6 Progress (Completed ✅)

{% assign issue_6_1 = site.issues | where: "issue_number", "6.1" | first %}
{% assign issue_6_2 = site.issues | where: "issue_number", "6.2" | first %}
{% assign issue_6_3 = site.issues | where: "issue_number", "6.3" | first %}
{% assign issue_6_4 = site.issues | where: "issue_number", "6.4" | first %}

- **Total Week 6 Issues**: {{ site.issues | where: "week", 6 | size }} (All Complete)
- **Character Persona Selection**: {{ issue_6_1.status | default: "completed" }} ✅
- **Retro Design System**: {{ issue_6_2.status | default: "completed" }} ✅
- **Interactive World Map**: {{ issue_6_3.status | default: "completed" }} ✅
- **Mobile-First UI/UX**: {{ issue_6_4.status | default: "completed" }} ✅

#### Week 5 Progress (Completed ✅)

{% assign issue_5_1 = site.issues | where: "issue_number", "5.1" | first %}
{% assign issue_5_2 = site.issues | where: "issue_number", "5.2" | first %}
{% assign issue_5_3 = site.issues | where: "issue_number", "5.3" | first %}
{% assign issue_5_4 = site.issues | where: "issue_number", "5.4" | first %}

- **Total Week 5 Issues**: {{ site.issues | where: "week", 5 | size }} (All Complete)
- **API Security & Authentication**: {{ issue_5_1.status | default: "completed" }} ✅
- **Performance & Scalability**: {{ issue_5_2.status | default: "completed" }} ✅
- **Azure Cost Management**: {{ issue_5_3.status | default: "completed" }} ✅
- **Production Security Hardening**: {{ issue_5_4.status | default: "completed" }} ✅

#### Week 4 Progress (Completed ✅)

{% assign issue_4_1 = site.issues | where: "issue_number", "4.1" | first %}
{% assign issue_4_2 = site.issues | where: "issue_number", "4.2" | first %}
{% assign issue_4_3 = site.issues | where: "issue_number", "4.3" | first %}

- **Total Week 4 Issues**: {{ site.issues | where: "week", 4 | size }} (All Complete)
- **AI Agent System**: {{ issue_4_1.status | default: "completed" }} ✅
- **Territory Management**: {{ issue_4_2.status | default: "completed" }} ✅
- **Speech Recognition**: {{ issue_4_3.status | default: "completed" }} ✅

#### Overall Project Status

- **Total Issues**: {{ site.issues | size }}
- **Week 3 Prep Issues**: {{ week3_issues | size }} (✅ Complete)
- **Critical Priority**: {{ site.issues | where: "priority", "critical" | size }}
- **High Priority**: {{ site.issues | where: "priority", "high" | size }}

---

## 📚 Related Documentation

- **[Journey Documentation]({{ '/journey/' | relative_url }})**: Week-by-week development progress
- **[Technical Guides]({{ '/technical-docs/' | relative_url }})**: Implementation documentation
- **[Posts]({{ '/post/' | relative_url }})**: Development insights and methodology

---

_This issues system demonstrates how AI can systematically identify, document, and structure complex project improvement needs while maintaining focus on educational objectives and child safety requirements._

<style>
.issues-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 1.5rem;
  margin: 2rem 0;
}

.issue-card {
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  padding: 1.5rem;
  background: white;
  transition: all 0.3s ease;
}

.issue-card:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
}

/* Week 9 - Architecture */
.week9-issue {
  border-left: 4px solid #6366f1;
  background: linear-gradient(135deg, #f0f1ff 0%, #fafafa 100%);
}

.architecture-focus, .architecture-tag {
  background: #e0e7ff;
  color: #4338ca;
  padding: 0.2rem 0.5rem;
  margin: 0.2rem 0.2rem 0.2rem 0;
  border-radius: 4px;
  font-size: 0.8rem;
}

/* Week 8 - Advanced Features */
.week8-issue {
  background: linear-gradient(135deg, 
    rgba(139, 92, 246, 0.1) 0%, 
    rgba(59, 130, 246, 0.1) 100%);
  border-left: 4px solid #8b5cf6;
  position: relative;
}

.week8-issue::before {
  content: "🚀";
  position: absolute;
  top: 10px;
  right: 10px;
  font-size: 24px;
  opacity: 0.7;
}

.advanced-features {
  background: linear-gradient(45deg, #8b5cf6, #3b82f6);
  color: white;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: bold;
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.advanced-tag {
  background: rgba(139, 92, 246, 0.1);
  color: #6d28d9;
  padding: 2px 6px;
  border-radius: 8px;
  font-size: 0.75rem;
  margin-right: 4px;
  border: 1px solid rgba(139, 92, 246, 0.2);
}

/* Week 7 - Testing Framework */
.week7-issue {
  background: linear-gradient(135deg, 
    rgba(245, 158, 11, 0.1) 0%, 
    rgba(251, 191, 36, 0.1) 100%);
  border-left: 4px solid #f59e0b;
  position: relative;
}

.week7-issue::before {
  content: "🧪";
  position: absolute;
  top: 10px;
  right: 10px;
  font-size: 24px;
  opacity: 0.7;
}

.testing-focus {
  background: linear-gradient(45deg, #f59e0b, #fbbf24);
  color: white;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: bold;
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.testing-tag {
  background: rgba(245, 158, 11, 0.1);
  color: #d97706;
  padding: 2px 6px;
  border-radius: 8px;
  font-size: 0.75rem;
  margin-right: 4px;
  border: 1px solid rgba(245, 158, 11, 0.2);
}

/* Week 6 - Child Designer Vision */
.week6-issue {
  background: linear-gradient(135deg, 
    rgba(34, 197, 94, 0.1) 0%, 
    rgba(59, 130, 246, 0.1) 100%);
  border-left: 4px solid #22c55e;
  position: relative;
}

.week6-issue::before {
  content: "🎨";
  position: absolute;
  top: 10px;
  right: 10px;
  font-size: 24px;
  opacity: 0.7;
}

.child-vision {
  background: linear-gradient(45deg, #22c55e, #10b981);
  color: white;
  padding: 2px 8px;
  border-radius: 12px;
  font-size: 0.75rem;
  font-weight: bold;
  display: inline-flex;
  align-items: center;
  gap: 4px;
}

.creative-tag {
  background: rgba(34, 197, 94, 0.1);
  color: #047857;
  padding: 2px 6px;
  border-radius: 8px;
  font-size: 0.75rem;
  margin-right: 4px;
  border: 1px solid rgba(34, 197, 94, 0.2);
}

.week4-issue {
  border-left: 4px solid #3b82f6;
  background: linear-gradient(135deg, #f8fafc 0%, #e2e8f0 100%);
}

.completed {
  border-left: 4px solid #10b981;
  background: linear-gradient(135deg, #f0fdf4 0%, #dcfce7 100%);
  opacity: 0.8;
}

.issue-header h3 {
  margin: 0 0 0.5rem 0;
  font-size: 1.1rem;
}

.issue-header h3 a {
  text-decoration: none;
  color: #1e293b;
}

.issue-header h3 a:hover {
  color: #3b82f6;
}

.issue-meta {
  display: flex;
  gap: 0.5rem;
  flex-wrap: wrap;
  margin-bottom: 1rem;
}

.issue-number {
  background: #3b82f6;
  color: white;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: bold;
}

.priority {
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: bold;
}

.priority-high {
  background: #dc2626;
  color: white;
}

.priority-medium {
  background: #d97706;
  color: white;
}

.priority-critical {
  background: #7c2d12;
  color: white;
}

.effort, .ai-autonomy {
  background: #f1f5f9;
  color: #475569;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  font-size: 0.8rem;
}

.status {
  background: #10b981;
  color: white;
  padding: 0.2rem 0.5rem;
  border-radius: 4px;
  font-size: 0.8rem;
  font-weight: bold;
}

.educational-focus {
  margin-bottom: 1rem;
  font-size: 0.9rem;
}

.educational-tag {
  display: inline-block;
  background: #e0f2fe;
  color: #0369a1;
  padding: 0.2rem 0.5rem;
  margin: 0.2rem 0.2rem 0.2rem 0;
  border-radius: 4px;
  font-size: 0.8rem;
}

.issue-description {
  color: #64748b;
  font-size: 0.9rem;
  line-height: 1.4;
}

.tag {
  background: #f8fafc;
  color: #475569;
  padding: 0.2rem 0.5rem;
  margin: 0.2rem;
  border-radius: 4px;
  font-size: 0.8rem;
}

.issue-card {
padding: 1.5rem;
border-radius: 8px;
border: 1px solid var(--border);
transition: all 0.2s ease;
}

.issue-card:hover {
transform: translateY(-2px);
box-shadow: 0 4px 12px rgba(0,0,0,0.1);
}

.issue-header h3 {
margin: 0 0 0.5rem 0;
font-size: 1.1rem;
}

.issue-header a {
text-decoration: none;
color: var(--foreground);
}

.issue-header a:hover {
color: var(--primary);
}

.issue-meta {
display: flex;
gap: 0.5rem;
flex-wrap: wrap;
margin-bottom: 1rem;
font-size: 0.875rem;
}

.priority {
padding: 0.25rem 0.5rem;
border-radius: 4px;
font-weight: 600;
font-size: 0.75rem;
}

.priority-critical {
background: #fee2e2;
color: #dc2626;
}

.priority-high {
background: #fef3c7;
color: #d97706;
}

.priority-medium {
background: #dbeafe;
color: #2563eb;
}

.effort, .ai-autonomy {
background: var(--muted);
color: var(--muted-foreground);
padding: 0.25rem 0.5rem;
border-radius: 4px;
font-size: 0.75rem;
}

.issue-tags {
display: flex;
gap: 0.25rem;
flex-wrap: wrap;
}

.tag {
background: var(--accent);
color: var(--accent-foreground);
padding: 0.125rem 0.375rem;
border-radius: 3px;
font-size: 0.7rem;
font-weight: 500;
}

@media (max-width: 768px) {
.issues-grid {
grid-template-columns: 1fr;
}

.issue-meta {
flex-direction: column;
gap: 0.25rem;
}
}
</style>
