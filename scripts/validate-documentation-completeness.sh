#!/bin/bash
# validate-documentation-completeness.sh - Week 6 Retro Transformation Validation
# Comprehensive validation for all Week 6 documentation and planning

echo "🔍 Validating Week 6 Retro Transformation Documentation Completeness..."

# Color codes for output
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Validation counters
PASSED=0
FAILED=0

validate_file() {
    local file=$1
    local description=$2
    
    if [ -f "$file" ]; then
        echo -e "${GREEN}✅ ${description}${NC}"
        ((PASSED++))
        return 0
    else
        echo -e "${RED}❌ ${description}${NC}"
        ((FAILED++))
        return 1
    fi
}

validate_content() {
    local file=$1
    local pattern=$2
    local description=$3
    
    if [ -f "$file" ] && grep -q "$pattern" "$file"; then
        echo -e "${GREEN}✅ ${description}${NC}"
        ((PASSED++))
        return 0
    else
        echo -e "${RED}❌ ${description}${NC}"
        ((FAILED++))
        return 1
    fi
}

echo ""
echo "📋 GitHub Issues Validation (Should be created in GitHub repository)"
echo "====================================================================="
echo -e "${YELLOW}ℹ️  Issue 6.1: Visual Foundation Transformation (Create in GitHub)${NC}"
echo -e "${YELLOW}ℹ️  Issue 6.2: Character Persona System (Create in GitHub)${NC}"
echo -e "${YELLOW}ℹ️  Issue 6.3: Interactive World Map (Create in GitHub)${NC}"
echo -e "${YELLOW}ℹ️  Issue 6.4: Mobile-First Retro UI (Create in GitHub)${NC}"

echo ""
echo "🤖 Copilot Instructions Validation"
echo "=================================="
validate_file ".github/copilot-instructions/retro-design-standards.md" "Retro Design Standards Module"
validate_file ".github/copilot-instructions/pwa-standards.md" "PWA Standards Module"
validate_content ".github/copilot-instructions.md" "retro-design-standards.md" "Master instructions reference retro module"
validate_content ".github/copilot-instructions.md" "pwa-standards.md" "Master instructions reference PWA module"

echo ""
echo "📚 Technical Documentation Validation"
echo "====================================="
validate_file "docs/_technical/retro-32bit-design-implementation.md" "Retro Design Implementation Guide"
validate_file "docs/_technical/pwa-standards-brand-validation.md" "PWA Standards & Brand Validation Guide"
validate_file "docs/_technical/pixel-art-asset-creation-guidelines.md" "Pixel Art Asset Creation Guidelines"
validate_content "docs/technical-docs.md" "pixel-art-asset-creation-guidelines" "Technical index includes asset guidelines"

echo ""
echo "🎯 Journey Documentation Validation"
echo "==================================="
validate_file "docs/_journey/week-06-retro-transformation.md" "Week 6 Journey Documentation"
validate_content "docs/_journey/week-06-retro-transformation.md" "85%" "AI autonomy percentage documented"
validate_content "docs/_journey/week-06-retro-transformation.md" "Child Designer Vision" "Child designer vision documented"

echo ""
echo "📰 Blog Content Validation"
echo "=========================="
validate_file "docs/_posts/2025-08-09-week-6-retro-transformation.md" "Week 6 Blog Post"
validate_content "docs/_posts/2025-08-09-week-6-retro-transformation.md" "12-year-old" "Target audience mentioned"
validate_content "docs/_posts/2025-08-09-week-6-retro-transformation.md" "pixel art" "Retro design focus documented"

echo ""
echo "🎨 Retro Design Validation"
echo "=========================="
validate_content "docs/_technical/retro-32bit-design-implementation.md" "retro-green-primary" "Green theme color palette defined"
validate_content "docs/_technical/retro-32bit-design-implementation.md" "image-rendering: pixelated" "Pixel art rendering specified"
validate_content "docs/_technical/pixel-art-asset-creation-guidelines.md" "32x32px" "Asset size specifications"

echo ""
echo "📱 PWA Standards Validation"
echo "==========================="
validate_content "docs/_technical/pwa-standards-brand-validation.md" "manifest.json" "PWA manifest requirements"
validate_content "docs/_technical/pwa-standards-brand-validation.md" "icon-192x192.png" "PWA icon specifications"
validate_content "docs/_technical/pwa-standards-brand-validation.md" "service-worker" "Service worker requirements"

echo ""
echo "🛡️ Child Safety Validation"
echo "=========================="
validate_content "docs/_technical/retro-32bit-design-implementation.md" "Child-friendly" "Child-friendly design mentioned"
validate_content "docs/_technical/pixel-art-asset-creation-guidelines.md" "Cultural Sensitivity" "Cultural sensitivity guidelines"
validate_content "docs/_journey/week-06-retro-transformation.md" "Child Safety" "Child safety standards maintained"

echo ""
echo "📊 Documentation Status Validation"
echo "=================================="
validate_content "docs/_data/documentation-status.yml" "week_6_retro_transformation" "Week 6 status tracked"
validate_content "docs/_data/documentation-status.yml" "technical_documented: true" "Technical documentation marked complete"
validate_content "docs/_data/documentation-status.yml" "blog_post_created: true" "Blog post creation tracked"

echo ""
echo "📋 Summary"
echo "=========="
echo -e "Validation Results: ${GREEN}${PASSED} passed${NC}, ${RED}${FAILED} failed${NC}"

if [ $FAILED -eq 0 ]; then
    echo -e "${GREEN}🎉 All Week 6 Retro Transformation documentation is complete!${NC}"
    echo "Ready for implementation phase."
    exit 0
else
    echo -e "${RED}⚠️  Some documentation items are missing or incomplete.${NC}"
    echo "Please address the failed validations before proceeding."
    exit 1
fi
