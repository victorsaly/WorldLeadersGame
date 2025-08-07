/**
 * UI Helper Functions for World Leaders Game
 * Context: Educational game UI helpers for 12-year-old players
 * Safety: Child-safe JavaScript utilities with XSS protection
 */

window.UIHelpers = {
    /**
     * Safely focus an element by ID
     * @param {string} elementId - The ID of the element to focus
     */
    focusElementById: function(elementId) {
        try {
            const element = document.getElementById(elementId);
            if (element) {
                element.focus();
            }
        } catch (error) {
            console.warn('Focus operation failed:', error);
        }
    },

    /**
     * Safely scroll to an element by ID
     * @param {string} elementId - The ID of the element to scroll to
     */
    scrollToElementById: function(elementId) {
        try {
            const element = document.getElementById(elementId);
            if (element) {
                element.scrollIntoView({ behavior: 'smooth' });
            }
        } catch (error) {
            console.warn('Scroll operation failed:', error);
        }
    },

    /**
     * Show/hide an element by ID
     * @param {string} elementId - The ID of the element
     * @param {boolean} show - Whether to show or hide the element
     */
    toggleElementVisibility: function(elementId, show) {
        try {
            const element = document.getElementById(elementId);
            if (element) {
                element.style.display = show ? '' : 'none';
            }
        } catch (error) {
            console.warn('Toggle visibility failed:', error);
        }
    }
};

// Legacy support for direct function calls
window.focusElementById = window.UIHelpers.focusElementById;
