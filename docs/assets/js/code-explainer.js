/**
 * Simple Code Explanation Toggle
 * Context: Clean, minimal code explanations for adult developers
 * Purpose: Toggle visibility of explanations embedded in markdown
 */

// Simple initialization when DOM is ready
document.addEventListener('DOMContentLoaded', function() {
    console.log('� Simple code explanation system initialized');
    
    // All explanations are already in the HTML as <details> elements
    // No API calls needed - just CSS for styling
    
    // Add click tracking for analytics (optional)
    const explanations = document.querySelectorAll('.code-explanation');
    explanations.forEach((explanation, index) => {
        explanation.addEventListener('toggle', function() {
            if (this.open) {
                console.log(`📖 Code explanation ${index + 1} opened`);
            } else {
                console.log(`📖 Code explanation ${index + 1} closed`);
            }
        });
    });
    
    console.log(`✅ Found ${explanations.length} code explanations ready to use`);
});
