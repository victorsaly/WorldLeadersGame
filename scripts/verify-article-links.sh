#!/bin/bash

# Article Link Verification Script
# Verifies all links in markdown articles and reports broken ones with remediation suggestions
# Usage: ./verify-article-links.sh [article-path]

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Configuration
TIMEOUT=10
USER_AGENT="Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36"
LOG_FILE="link-verification-$(date +%Y%m%d-%H%M%S).log"

# Function to print colored output
print_status() {
    local color=$1
    local message=$2
    echo -e "${color}${message}${NC}"
}

# Function to extract links from markdown file
extract_links() {
    local file=$1
    # Extract both [text](url) and bare URLs, excluding images
    grep -oE '\[([^]]+)\]\(([^)]+)\)' "$file" | grep -v '!\[' | sed 's/\[.*\](\([^)]*\))/\1/' | sort -u
    # Also extract bare URLs that start with http
    grep -oE 'https?://[^\s<>"\[\]{}|\\^`]+' "$file" | sort -u
}

# Function to check if URL is accessible
check_url() {
    local url=$1
    local status_code
    
    # Handle special cases that might need different handling
    case "$url" in
        *linkedin.com*)
            # LinkedIn often blocks automated requests, try with specific headers
            status_code=$(curl -s -o /dev/null -w "%{http_code}" \
                --max-time $TIMEOUT \
                --user-agent "$USER_AGENT" \
                --header "Accept: text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8" \
                --header "Accept-Language: en-US,en;q=0.5" \
                --header "Accept-Encoding: gzip, deflate" \
                --header "Connection: keep-alive" \
                --header "Upgrade-Insecure-Requests: 1" \
                "$url" 2>/dev/null || echo "000")
            ;;
        *github.com*)
            # GitHub sometimes needs authentication for certain endpoints
            status_code=$(curl -s -o /dev/null -w "%{http_code}" \
                --max-time $TIMEOUT \
                --user-agent "$USER_AGENT" \
                "$url" 2>/dev/null || echo "000")
            ;;
        *)
            # Standard check for other URLs
            status_code=$(curl -s -o /dev/null -w "%{http_code}" \
                --max-time $TIMEOUT \
                --user-agent "$USER_AGENT" \
                --location \
                "$url" 2>/dev/null || echo "000")
            ;;
    esac
    
    echo "$status_code"
}

# Function to suggest alternative URLs
suggest_alternatives() {
    local broken_url=$1
    
    echo "    ${YELLOW}Suggested alternatives:${NC}"
    
    case "$broken_url" in
        *linkedin.com/in/debbie-obrien* | *linkedin.com/in/debs-obrien*)
            echo "      - https://github.com/debs-obrien"
            echo "      - https://twitter.com/debs_obrien"
            echo "      - https://dev.to/debs_obrien"
            ;;
        *linkedin.com/in/philnash*)
            echo "      - https://philna.sh/"
            echo "      - https://github.com/philnash"
            echo "      - https://twitter.com/philnash"
            ;;
        *dev.to/justinschroeder*)
            echo "      - https://github.com/jpschroeder"
            echo "      - https://twitter.com/jpschroeder"
            echo "      - https://justinschroeder.com/"
            ;;
        *github.com/debs-obrien*)
            echo "      - https://linkedin.com/in/debbie-obrien"
            echo "      - https://twitter.com/debs_obrien"
            ;;
        *github.com/philnash*)
            echo "      - https://philna.sh/"
            echo "      - https://linkedin.com/in/philnash"
            ;;
        *github.com/jpschroeder*)
            echo "      - https://dev.to/justinschroeder"
            echo "      - https://justinschroeder.com/"
            ;;
        *tej.as*)
            echo "      - https://github.com/tejasq"
            echo "      - https://twitter.com/tejaskumar_"
            echo "      - https://linkedin.com/in/tejaskumar"
            ;;
        *kentcdodds.com*)
            echo "      - https://github.com/kentcdodds"
            echo "      - https://twitter.com/kentcdodds"
            ;;
        *compute-sdk.com*)
            echo "      - https://github.com/compute-sdk"
            echo "      - https://linkedin.com/in/garrison-snelling"
            ;;
        *langflow.org*)
            echo "      - https://github.com/langflow-ai/langflow"
            echo "      - https://docs.langflow.org/"
            ;;
        *anysphere.com*)
            echo "      - https://cursor.sh/"
            echo "      - https://github.com/anysphere"
            ;;
        *github.com*)
            echo "      - Check if repository was renamed or moved"
            echo "      - Try searching GitHub for the project name"
            ;;
        *linkedin.com*)
            echo "      - LinkedIn profiles may have privacy restrictions"
            echo "      - Try finding their GitHub or personal website"
            ;;
        *dev.to*)
            echo "      - Check if username changed on dev.to"
            echo "      - Look for their GitHub profile instead"
            ;;
        *)
            echo "      - Try using web.archive.org to find cached version"
            echo "      - Search for the resource with different domain"
            echo "      - Check if the website has moved to a new URL"
            ;;
    esac
}

# Function to verify all links in a file
verify_article_links() {
    local article_file=$1
    
    if [[ ! -f "$article_file" ]]; then
        print_status $RED "Error: Article file '$article_file' not found"
        exit 1
    fi
    
    print_status $BLUE "🔍 Verifying links in: $article_file"
    print_status $BLUE "📝 Log file: $LOG_FILE"
    echo ""
    
    # Extract all links
    local links_temp=$(mktemp)
    extract_links "$article_file" > "$links_temp"
    
    local total_links=$(wc -l < "$links_temp")
    
    if [[ $total_links -eq 0 ]]; then
        print_status $YELLOW "No links found in the article"
        rm "$links_temp"
        return 0
    fi
    
    print_status $BLUE "Found $total_links unique links to verify"
    echo ""
    
    # Initialize counters
    local working_links=0
    local broken_links=0
    local skipped_links=0
    
    # Temporary files to store results
    local broken_urls_temp=$(mktemp)
    
    # Log header
    {
        echo "========================================="
        echo "Link Verification Report"
        echo "Article: $article_file"
        echo "Date: $(date)"
        echo "========================================="
        echo ""
    } > "$LOG_FILE"
    
    # Check each link
    local current=1
    while IFS= read -r url; do
        # Skip empty URLs or malformed ones
        if [[ -z "$url" || "$url" == *"#"* ]]; then
            print_status $YELLOW "[$current/$total_links] Skipping anchor/fragment: $url"
            ((skipped_links++))
            ((current++))
            continue
        fi
        
        printf "[$current/$total_links] Checking: %-60s " "$url"
        
        local status_code=$(check_url "$url")
        
        # Log the attempt
        echo "[$current/$total_links] $url -> HTTP $status_code" >> "$LOG_FILE"
        
        case "$status_code" in
            200|301|302|303|307|308)
                print_status $GREEN "✅ OK ($status_code)"
                ((working_links++))
                ;;
            000)
                print_status $RED "❌ TIMEOUT/CONNECTION_FAILED"
                echo "$url|TIMEOUT/CONNECTION_FAILED" >> "$broken_urls_temp"
                ((broken_links++))
                ;;
            *)
                print_status $RED "❌ BROKEN ($status_code)"
                echo "$url|$status_code" >> "$broken_urls_temp"
                ((broken_links++))
                ;;
        esac
        
        ((current++))
        sleep 0.5  # Be nice to servers
    done < "$links_temp"
    
    echo ""
    print_status $BLUE "========================================="
    print_status $BLUE "VERIFICATION SUMMARY"
    print_status $BLUE "========================================="
    print_status $GREEN "✅ Working links: $working_links"
    print_status $RED "❌ Broken links: $broken_links"
    print_status $YELLOW "⏭️  Skipped links: $skipped_links"
    print_status $BLUE "📊 Total checked: $((working_links + broken_links))"
    
    # Log summary
    {
        echo ""
        echo "========================================="
        echo "SUMMARY"
        echo "========================================="
        echo "Working links: $working_links"
        echo "Broken links: $broken_links"
        echo "Skipped links: $skipped_links"
        echo "Total checked: $((working_links + broken_links))"
        echo ""
    } >> "$LOG_FILE"
    
    # Report broken links with remediation suggestions
    if [[ $broken_links -gt 0 ]]; then
        echo ""
        print_status $RED "🚨 BROKEN LINKS REPORT"
        print_status $RED "================================"
        
        {
            echo "BROKEN LINKS REQUIRING REMEDIATION:"
            echo "===================================="
            echo ""
        } >> "$LOG_FILE"
        
        while IFS='|' read -r url status; do
            print_status $RED "❌ $url"
            print_status $RED "   Status: $status"
            suggest_alternatives "$url"
            echo ""
            
            # Log to file (remove color codes)
            {
                echo "❌ $url"
                echo "   Status: $status"
                suggest_alternatives "$url" 2>/dev/null | sed 's/\x1b\[[0-9;]*m//g'
                echo ""
            } >> "$LOG_FILE"
        done < "$broken_urls_temp"
        
        print_status $YELLOW "💡 REMEDIATION STEPS:"
        echo "1. Review broken links above and their suggested alternatives"
        echo "2. Test suggested alternatives manually"
        echo "3. Update the article with working URLs"
        echo "4. Re-run this script to verify fixes"
        echo ""
        print_status $BLUE "📋 Detailed report saved to: $LOG_FILE"
        
    else
        print_status $GREEN "🎉 All links are working!"
        echo "All links verified successfully" >> "$LOG_FILE"
    fi
    
    # Cleanup temporary files
    rm -f "$links_temp" "$broken_urls_temp"
    
    return $broken_links
}

# Function to show usage
show_usage() {
    echo "Usage: $0 [article-path]"
    echo ""
    echo "Examples:"
    echo "  $0 docs/devto/articles/2025-09-13-ai-driven-development-day-insights.md"
    echo "  $0 docs/_posts/2025-09-13-ai-driven-development-day-insights.md"
    echo ""
    echo "Options:"
    echo "  -h, --help    Show this help message"
    echo ""
    echo "The script will:"
    echo "  - Extract all links from the markdown file"
    echo "  - Test each link for accessibility"
    echo "  - Report broken links with suggested alternatives"
    echo "  - Generate a detailed log file"
}

# Main script logic
main() {
    # Check for help flag
    if [[ "$1" == "-h" || "$1" == "--help" ]]; then
        show_usage
        exit 0
    fi
    
    # Check if curl is available
    if ! command -v curl &> /dev/null; then
        print_status $RED "Error: curl is required but not installed"
        exit 1
    fi
    
    # Get article file path
    local article_file="$1"
    
    # If no argument provided, try to find article files
    if [[ -z "$article_file" ]]; then
        print_status $YELLOW "No article specified. Available articles:"
        echo ""
        
        # Look for common article locations
        local found_count=0
        
        if [[ -d "docs/devto/articles" ]]; then
            find docs/devto/articles -name "*.md" 2>/dev/null | while read -r file; do
                echo "$((++found_count)). $file"
            done
        fi
        
        if [[ -d "docs/_posts" ]]; then
            find docs/_posts -name "*.md" 2>/dev/null | while read -r file; do
                echo "$((++found_count)). $file"
            done
        fi
        
        echo ""
        show_usage
        exit 1
    fi
    
    # Verify the article and return appropriate exit code
    verify_article_links "$article_file"
    exit $?
}

# Run main function with all arguments
main "$@"