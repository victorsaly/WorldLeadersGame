// Mobile-First Accessibility and Performance Enhancements
// Educational game focus management for 12-year-old learners

window.RetroGameAccessibility = {
    // Focus management for mobile and keyboard users
    initializeFocusManagement: function() {
        // Trap focus in modals
        this.setupModalFocusTrap();
        
        // Handle mobile navigation focus
        this.setupMobileNavFocus();
        
        // Setup keyboard navigation for game components
        this.setupGameKeyboardNavigation();
        
        // Performance optimizations
        this.setupPerformanceOptimizations();
    },

    // Focus trap for modal dialogs
    setupModalFocusTrap: function() {
        document.addEventListener('keydown', function(e) {
            const modal = document.querySelector('.retro-modal-visible');
            if (!modal) return;

            const focusableElements = modal.querySelectorAll(
                'button, [href], input, select, textarea, [tabindex]:not([tabindex="-1"])'
            );
            
            if (focusableElements.length === 0) return;

            const firstFocusable = focusableElements[0];
            const lastFocusable = focusableElements[focusableElements.length - 1];

            if (e.key === 'Tab') {
                if (e.shiftKey) {
                    if (document.activeElement === firstFocusable) {
                        lastFocusable.focus();
                        e.preventDefault();
                    }
                } else {
                    if (document.activeElement === lastFocusable) {
                        firstFocusable.focus();
                        e.preventDefault();
                    }
                }
            }

            // Close modal on Escape
            if (e.key === 'Escape') {
                const closeButton = modal.querySelector('.retro-modal-close');
                if (closeButton) {
                    closeButton.click();
                }
            }
        });
    },

    // Mobile navigation accessibility
    setupMobileNavFocus: function() {
        const navItems = document.querySelectorAll('.retro-nav-item');
        
        navItems.forEach((item, index) => {
            item.addEventListener('keydown', function(e) {
                if (e.key === 'ArrowRight' || e.key === 'ArrowDown') {
                    e.preventDefault();
                    const nextIndex = (index + 1) % navItems.length;
                    navItems[nextIndex].focus();
                } else if (e.key === 'ArrowLeft' || e.key === 'ArrowUp') {
                    e.preventDefault();
                    const prevIndex = (index - 1 + navItems.length) % navItems.length;
                    navItems[prevIndex].focus();
                }
            });
        });
    },

    // Game component keyboard navigation
    setupGameKeyboardNavigation: function() {
        // Territory cards keyboard navigation
        const territoryCards = document.querySelectorAll('.retro-territory-card');
        
        territoryCards.forEach((card, index) => {
            card.addEventListener('keydown', function(e) {
                if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault();
                    card.click();
                } else if (e.key === 'ArrowRight') {
                    e.preventDefault();
                    const nextIndex = (index + 1) % territoryCards.length;
                    territoryCards[nextIndex].focus();
                } else if (e.key === 'ArrowLeft') {
                    e.preventDefault();
                    const prevIndex = (index - 1 + territoryCards.length) % territoryCards.length;
                    territoryCards[prevIndex].focus();
                }
            });
        });
    },

    // Performance optimizations for mobile
    setupPerformanceOptimizations: function() {
        // Lazy load images and components
        if ('IntersectionObserver' in window) {
            const imageObserver = new IntersectionObserver((entries, observer) => {
                entries.forEach(entry => {
                    if (entry.isIntersecting) {
                        const img = entry.target;
                        if (img.dataset.src) {
                            img.src = img.dataset.src;
                            img.removeAttribute('data-src');
                            observer.unobserve(img);
                        }
                    }
                });
            }, {
                rootMargin: '50px 0px',
                threshold: 0.01
            });

            document.querySelectorAll('img[data-src]').forEach(img => {
                imageObserver.observe(img);
            });
        }

        // Optimize touch interactions
        this.optimizeTouchInteractions();
        
        // Handle viewport changes for mobile
        this.handleViewportChanges();
    },

    // Touch interaction optimizations
    optimizeTouchInteractions: function() {
        // Add touch feedback for better user experience
        const touchElements = document.querySelectorAll('.retro-touch-button, .retro-touch-button-large, .retro-territory-card');
        
        touchElements.forEach(element => {
            element.addEventListener('touchstart', function() {
                this.classList.add('retro-touch-active');
            }, { passive: true });
            
            element.addEventListener('touchend', function() {
                setTimeout(() => {
                    this.classList.remove('retro-touch-active');
                }, 150);
            }, { passive: true });
            
            element.addEventListener('touchcancel', function() {
                this.classList.remove('retro-touch-active');
            }, { passive: true });
        });
    },

    // Handle mobile viewport changes
    handleViewportChanges: function() {
        // Update CSS custom properties for dynamic viewport
        const updateViewportHeight = () => {
            const vh = window.innerHeight * 0.01;
            document.documentElement.style.setProperty('--vh', `${vh}px`);
        };

        updateViewportHeight();
        window.addEventListener('resize', updateViewportHeight);
        window.addEventListener('orientationchange', () => {
            setTimeout(updateViewportHeight, 100);
        });
    },

    // Announce content changes for screen readers
    announceForScreenReader: function(message) {
        const announcement = document.createElement('div');
        announcement.setAttribute('aria-live', 'polite');
        announcement.setAttribute('aria-atomic', 'true');
        announcement.style.position = 'absolute';
        announcement.style.left = '-10000px';
        announcement.style.width = '1px';
        announcement.style.height = '1px';
        announcement.style.overflow = 'hidden';
        
        document.body.appendChild(announcement);
        announcement.textContent = message;
        
        setTimeout(() => {
            document.body.removeChild(announcement);
        }, 1000);
    },

    // Check if user prefers reduced motion
    prefersReducedMotion: function() {
        return window.matchMedia('(prefers-reduced-motion: reduce)').matches;
    },

    // Check if user prefers high contrast
    prefersHighContrast: function() {
        return window.matchMedia('(prefers-contrast: high)').matches;
    },

    // Initialize when DOM is ready
    init: function() {
        if (document.readyState === 'loading') {
            document.addEventListener('DOMContentLoaded', () => {
                this.initializeFocusManagement();
            });
        } else {
            this.initializeFocusManagement();
        }
    }
};

// Initialize accessibility features
window.RetroGameAccessibility.init();

// Export for Blazor interop
window.blazorInterop = window.blazorInterop || {};
window.blazorInterop.accessibility = window.RetroGameAccessibility;