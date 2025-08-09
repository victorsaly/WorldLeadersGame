---
layout: default
title: "All Development Posts"
subtitle: "Complete AI-Led Game Creation Journey Documentation"
permalink: /post/
---

## 📝 **All Posts** ({{ site.posts.size }} total)

<div class="post-list">
  {% for post in site.posts %}
  <article class="post-item">
    <h2><a href="{{ post.url | relative_url }}">{{ post.title }}</a></h2>
    {% if post.subtitle %}
    <h3 class="post-subtitle">{{ post.subtitle }}</h3>
    {% endif %}
    <p class="post-meta">
      <time datetime="{{ post.date | date_to_xmlschema }}">{{ post.date | date: "%B %d, %Y" }}</time>
      {% if post.categories %}
        {% for category in post.categories %}
          • <span class="category">{{ category }}</span>
        {% endfor %}
      {% endif %}
    </p>
    {% if post.tags %}
    <div class="tags">
      {% for tag in post.tags %}
        <span class="tag">{{ tag }}</span>
      {% endfor %}
    </div>
    {% endif %}
    <div class="post-excerpt">
      {{ post.excerpt | strip_html | truncatewords: 50 }}
    </div>
    <a href="{{ post.url | relative_url }}" class="read-more">Read Full Post →</a>
  </article>
  {% unless forloop.last %}<hr class="post-divider">{% endunless %}
  {% endfor %}
</div>

---

## 📊 **Posts by Category**

<div class="category-grid">
  {% assign categories = site.posts | map: 'categories' | flatten | uniq | sort %}
  {% for category in categories %}
  <div class="category-section">
    <h3>{{ category | capitalize }}</h3>
    <ul>
      {% assign category_posts = site.posts | where: 'categories', category %}
      {% for post in category_posts limit: 10 %}
      <li><a href="{{ post.url | relative_url }}">{{ post.title }}</a> <span class="date">({{ post.date | date: "%b %Y" }})</span></li>
      {% endfor %}
      {% if category_posts.size > 10 %}
      <li><em>... and {{ category_posts.size | minus: 10 }} more posts</em></li>
      {% endif %}
    </ul>
  </div>
  {% endfor %}
</div>

---

## Follow Our Journey

- **RSS Feed**: [Subscribe to updates](/feed.xml)
- **GitHub**: [Watch repository for real-time commits](https://github.com/victorsaly/WorldLeadersGame)
- **Community**: Share insights and ask questions in GitHub Discussions

---

<div class="newsletter-signup">
  <h3>Stay Updated</h3>
  <p>Want to follow our AI-first development experiment? <strong>Star our GitHub repository</strong> and <strong>watch for updates</strong> to get notified of major milestones and breakthrough insights.</p>
  <a href="https://github.com/victorsaly/WorldLeadersGame" class="cta-button">Follow on GitHub →</a>
</div>
