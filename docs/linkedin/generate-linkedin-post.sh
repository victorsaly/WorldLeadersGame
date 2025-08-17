#!/bin/bash

# LinkedIn Post Generation Script
# Usage: ./generate-linkedin-post.sh <devto-article-path>
# Example: ./generate-linkedin-post.sh devto/articles/2025-08-03-ai-project-manager-systematic-issue-generation-devto.md

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Check if article path is provided
if [ $# -eq 0 ]; then
    echo -e "${RED}Error: Please provide the path to the dev.to article${NC}"
    echo "Usage: $0 <devto-article-path>"
    echo "Example: $0 devto/articles/2025-08-03-ai-project-manager-systematic-issue-generation-devto.md"
    exit 1
fi

ARTICLE_PATH="$1"
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
DOCS_DIR="$(dirname "$SCRIPT_DIR")"

# Check if article exists
if [ ! -f "$DOCS_DIR/$ARTICLE_PATH" ]; then
    echo -e "${RED}Error: Article not found at $DOCS_DIR/$ARTICLE_PATH${NC}"
    exit 1
fi

echo -e "${BLUE}🔍 Analyzing dev.to article for LinkedIn post generation...${NC}"

# Extract article metadata
ARTICLE_FILE="$DOCS_DIR/$ARTICLE_PATH"
ARTICLE_TITLE=$(grep "^title:" "$ARTICLE_FILE" | sed 's/title: *"//' | sed 's/"$//')
ARTICLE_DESCRIPTION=$(grep "^description:" "$ARTICLE_FILE" | sed 's/description: *"//' | sed 's/"$//')
ARTICLE_TAGS=$(grep "^tags:" "$ARTICLE_FILE" | sed 's/tags: *//')
ARTICLE_CANONICAL=$(grep "^canonical_url:" "$ARTICLE_FILE" | sed 's/canonical_url: *//')

# Generate output filename
BASENAME=$(basename "$ARTICLE_PATH" .md)
OUTPUT_FILE="$SCRIPT_DIR/post/${BASENAME}-linkedin.md"

echo -e "${YELLOW}📊 Article Analysis:${NC}"
echo "Title: $ARTICLE_TITLE"
echo "Description: $ARTICLE_DESCRIPTION"
echo "Tags: $ARTICLE_TAGS"
echo "Canonical URL: $ARTICLE_CANONICAL"

# Determine target audience and strategy based on tags
determine_strategy() {
    local tags="$1"
    case "$tags" in
        *ai*|*machinelearning*|*automation*|*artificial*)
            echo "ai-technical"
            ;;
        *projectmanagement*|*productivity*|*business*|*leadership*)
            echo "project-management"
            ;;
        *edtech*|*education*|*learning*|*child*)
            echo "educational-innovation"
            ;;
        *startup*|*growth*|*business*|*entrepreneur*)
            echo "startup-business"
            ;;
        *)
            echo "broad-professional"
            ;;
    esac
}

STRATEGY=$(determine_strategy "$ARTICLE_TAGS")
TODAY=$(date +%Y-%m-%d)
TRACKING_CODE="linkedin-$(date +%Y%m%d)"

echo -e "${YELLOW}🎯 Strategy Selection: $STRATEGY${NC}"

# Extract key metrics and insights from article content
echo -e "${BLUE}🔍 Extracting key metrics and insights...${NC}"

# Look for percentages, time savings, specific numbers
KEY_METRICS=$(grep -E "([0-9]+%|[0-9]+ hours?|[0-9]+ minutes?)" "$ARTICLE_FILE" | head -3 | sed 's/^[[:space:]]*//' | sed 's/[[:space:]]*$//')

# Look for compelling quotes or results
COMPELLING_QUOTES=$(grep -E "(Result|Results|achieved|reduced|improved)" "$ARTICLE_FILE" | head -2 | sed 's/^[[:space:]]*//' | sed 's/[[:space:]]*$//')

echo -e "${YELLOW}📈 Key Metrics Found:${NC}"
echo "$KEY_METRICS"

echo -e "${YELLOW}💡 Compelling Insights:${NC}"
echo "$COMPELLING_QUOTES"

# Generate LinkedIn post based on strategy
generate_linkedin_post() {
    local strategy="$1"
    local title="$2"
    local description="$3"
    local canonical_url="$4"
    local tracking_code="$5"
    
    # Add UTM tracking to URL
    local tracked_url="${canonical_url}?utm_source=linkedin&utm_medium=social&utm_campaign=${tracking_code}"
    
    case "$strategy" in
        "ai-technical")
            cat > "$OUTPUT_FILE" << EOF
---
article_source: "$ARTICLE_PATH"
publish_date: "$TODAY"
target_audience: ["ai-engineers", "ml-developers", "tech-leads"]
hashtag_strategy: "ai-technical"
tracking_code: "$tracking_code"
estimated_reach: "1500-3000"
post_type: "technical"
---

🚀 AI achieved [EXTRACT KEY METRIC] that typically takes human experts [TRADITIONAL TIME/EFFORT].

Building AI systems usually requires [TRADITIONAL COMPLEX APPROACH].

We implemented [YOUR INNOVATIVE APPROACH] using [SPECIFIC TOOLS/TECHNIQUES]. The key breakthrough was [SPECIFIC TECHNICAL INSIGHT].

📊 Technical Results:
• [PERFORMANCE METRIC]
• [EFFICIENCY GAIN] 
• [TECHNICAL QUALITY METRIC]

The implementation details, code patterns, and architectural decisions are covered in the full technical deep-dive.

What AI engineering challenges are you currently working through?

Read the complete technical guide: $tracked_url

#ArtificialIntelligence #MachineLearning #TechStack #Engineering #Innovation
EOF
            ;;
        "project-management")
            cat > "$OUTPUT_FILE" << EOF
---
article_source: "$ARTICLE_PATH"
publish_date: "$TODAY"
target_audience: ["project-managers", "team-leads", "operations"]
hashtag_strategy: "process-optimization"
tracking_code: "$tracking_code"
estimated_reach: "2000-4000"
post_type: "business"
---

📈 [EXTRACT TIME/EFFICIENCY SAVINGS] by systematically [PROCESS IMPROVEMENT AREA].

Project teams spend [TIME AMOUNT] on [MANUAL PROCESS] that creates [BUSINESS PAIN POINT].

We systematized [PROCESS AREA] using [METHODOLOGY/TOOLS] with [KEY INSIGHT]. The framework emphasizes [CRITICAL SUCCESS FACTOR].

📊 Process Results:
• [TIME SAVINGS PERCENTAGE/HOURS]
• [QUALITY IMPROVEMENT METRIC]
• [TEAM EFFICIENCY GAIN]

The complete framework includes step-by-step implementation and lessons learned from real project scenarios.

How are you optimizing project workflows in your organization?

Full process guide: $tracked_url

#ProjectManagement #ProcessOptimization #Productivity #Leadership #Efficiency
EOF
            ;;
        "educational-innovation")
            cat > "$OUTPUT_FILE" << EOF
---
article_source: "$ARTICLE_PATH"
publish_date: "$TODAY"
target_audience: ["edtech-leaders", "educators", "learning-designers"]
hashtag_strategy: "educational-innovation"
tracking_code: "$tracking_code"
estimated_reach: "1000-2500"
post_type: "educational"
---

🎓 [EDUCATIONAL INNOVATION RESULT] while maintaining [SAFETY/COMPLIANCE STANDARDS].

Educational technology must balance [LEARNING EFFECTIVENESS] with [SAFETY REQUIREMENTS] for [TARGET LEARNER GROUP].

We developed [EDUCATIONAL APPROACH] that integrates [TECHNOLOGY] with [LEARNING PRINCIPLES] while ensuring [SAFETY STANDARDS]. The key insight was [EDUCATIONAL BREAKTHROUGH].

📊 Educational Results:
• [LEARNING OUTCOME IMPROVEMENT]
• [ENGAGEMENT/RETENTION METRIC]
• [SAFETY/COMPLIANCE ACHIEVEMENT]

The methodology shows how to preserve educational integrity while leveraging advanced technology capabilities.

What educational technology challenges are you solving for your learners?

Complete educational case study: $tracked_url

#EdTech #LearningTechnology #EducationalInnovation #ChildSafety #GameBasedLearning
EOF
            ;;
        "startup-business")
            cat > "$OUTPUT_FILE" << EOF
---
article_source: "$ARTICLE_PATH"
publish_date: "$TODAY"
target_audience: ["startup-founders", "entrepreneurs", "small-teams"]
hashtag_strategy: "startup-efficiency"
tracking_code: "$tracking_code"
estimated_reach: "1500-3500"
post_type: "business"
---

💡 [BUSINESS EFFICIENCY ACHIEVEMENT] with [MINIMAL RESOURCE INVESTMENT].

Early-stage teams need [BUSINESS OUTCOME] but typically lack [RESOURCES/EXPERTISE] for [TRADITIONAL ENTERPRISE APPROACH].

We achieved [BUSINESS RESULT] using [LEAN APPROACH] with [RESOURCE CONSTRAINT]. The secret was [KEY BUSINESS INSIGHT].

📊 Business Impact:
• [ROI/COST SAVINGS METRIC]
• [TIME TO MARKET/SPEED]
• [SCALE/GROWTH ACHIEVEMENT]

The complete playbook shows how small teams can deliver enterprise-level results without enterprise budgets.

What operational bottlenecks are limiting your startup's growth potential?

Full business methodology: $tracked_url

#Startups #Efficiency #GrowthHacking #Innovation #BusinessStrategy
EOF
            ;;
        *)
            cat > "$OUTPUT_FILE" << EOF
---
article_source: "$ARTICLE_PATH"
publish_date: "$TODAY"
target_audience: ["professionals", "innovators", "leaders"]
hashtag_strategy: "broad-professional"
tracking_code: "$tracking_code"
estimated_reach: "2000-5000"
post_type: "standard"
---

🚀 [EXTRACT COMPELLING RESULT OR INSIGHT]

Most professionals still [TRADITIONAL APPROACH PROBLEM]. There's a systematic way to improve this.

We implemented [YOUR METHODOLOGY] that [UNIQUE APPROACH BENEFIT]. The key insight was [BREAKTHROUGH REALIZATION].

📊 Results:
• [SPECIFIC METRIC 1]
• [SPECIFIC METRIC 2]
• [SPECIFIC METRIC 3]

The complete methodology and implementation guide shows exactly how we achieved these outcomes.

What [RELATED CHALLENGE] are you working on in your organization?

Full methodology: $tracked_url

#Innovation #Technology #Leadership #Productivity #BusinessStrategy
EOF
            ;;
    esac
}

echo -e "${BLUE}✍️ Generating LinkedIn post...${NC}"

generate_linkedin_post "$STRATEGY" "$ARTICLE_TITLE" "$ARTICLE_DESCRIPTION" "$ARTICLE_CANONICAL" "$TRACKING_CODE"

echo -e "${GREEN}✅ LinkedIn post generated successfully!${NC}"
echo -e "${YELLOW}📍 Location: $OUTPUT_FILE${NC}"

# Display next steps
echo -e "${BLUE}📋 Next Steps:${NC}"
echo "1. Review and customize the generated post:"
echo "   - Replace [PLACEHOLDER] values with actual content from article"
echo "   - Verify metrics and insights are accurate"
echo "   - Ensure target audience alignment"
echo ""
echo "2. Quality check using the checklist:"
echo "   - Hook strength (first 1-2 lines)"
echo "   - Mobile readability"
echo "   - Value proposition clarity"
echo "   - Call-to-action effectiveness"
echo ""
echo "3. Schedule posting for optimal engagement:"
echo "   - Tuesday-Thursday 8-10 AM EST"
echo "   - Prepare for immediate engagement responses"
echo ""
echo "4. Track performance with UTM code: $TRACKING_CODE"

# Offer to open the file for editing
read -p "Would you like to open the generated post for editing? (y/n): " -n 1 -r
echo
if [[ $REPLY =~ ^[Yy]$ ]]; then
    if command -v code &> /dev/null; then
        code "$OUTPUT_FILE"
    elif command -v nano &> /dev/null; then
        nano "$OUTPUT_FILE"
    else
        echo "Please edit the file manually: $OUTPUT_FILE"
    fi
fi

echo -e "${GREEN}🎉 LinkedIn post generation complete!${NC}"
