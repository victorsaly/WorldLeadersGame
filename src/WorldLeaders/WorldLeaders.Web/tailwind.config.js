/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Components/**/*.{razor,html,cshtml}",
    "./Pages/**/*.{razor,html,cshtml}",
    "./Views/**/*.{razor,html,cshtml}",
    "./wwwroot/**/*.{html,js}",
    "./**/*.razor"
  ],
  theme: {
    extend: {
      // Child-friendly design system for 12-year-old learners
      colors: {
        'game-primary': {
          50: '#eff6ff',
          100: '#dbeafe', 
          200: '#bfdbfe',
          300: '#93c5fd',
          400: '#60a5fa',
          500: '#3b82f6', // Primary blue for trust and learning
          600: '#2563eb',
          700: '#1d4ed8',
          800: '#1e40af',
          900: '#1e3a8a',
        },
        'game-secondary': {
          50: '#faf5ff',
          100: '#f3e8ff',
          200: '#e9d5ff', 
          300: '#d8b4fe',
          400: '#c084fc',
          500: '#a855f7', // Purple for creativity and imagination
          600: '#9333ea',
          700: '#7c3aed',
          800: '#6b21a8',
          900: '#581c87',
        },
        'game-success': {
          50: '#ecfdf5',
          100: '#d1fae5',
          200: '#a7f3d0',
          300: '#6ee7b7',
          400: '#34d399',
          500: '#10b981', // Green for achievement and growth
          600: '#059669',
          700: '#047857',
          800: '#065f46',
          900: '#064e3b',
        },
        // Retro 32-bit Color Palette - Child Designer Approved
        'retro-green': {
          'dark': '#1a5a1a',
          'main': '#2ea44f',
          'light': '#4ade80',
          'bright': '#86efac',
        },
        'retro-blue': '#3b82f6',
        'retro-purple': '#8b5cf6',
        'retro-yellow': '#eab308',
        'retro-red': '#ef4444',
        'retro-orange': '#f97316',
        'pixel': {
          'black': '#000000',
          'dark-gray': '#374151',
          'gray': '#6b7280',
          'light-gray': '#d1d5db',
          'white': '#ffffff',
        }
      },
      fontFamily: {
        'child-friendly': ['Comic Neue', 'Arial Rounded MT Bold', 'Helvetica Rounded', 'Arial', 'sans-serif'],
        'readable': ['Open Sans', 'Inter', 'Arial', 'sans-serif'],
        'retro': ['Press Start 2P', 'Courier New', 'monospace'],
        'retro-body': ['Orbitron', 'Courier New', 'monospace']
      },
      fontSize: {
        'child-xs': ['12px', '18px'],
        'child-sm': ['14px', '20px'], 
        'child-base': ['18px', '26px'], // Larger base for readability
        'child-lg': ['22px', '30px'],
        'child-xl': ['26px', '34px'],
        'child-2xl': ['32px', '40px'],
        'child-3xl': ['38px', '46px']
      },
      spacing: {
        'child-touch': '44px', // Minimum touch target size
        'child-comfortable': '56px', // Comfortable touch target
        'child-large': '72px' // Large touch target for young users
      },
      borderRadius: {
        'child': '12px', // Friendly rounded corners
        'child-lg': '16px',
        'child-xl': '20px'
      },
      boxShadow: {
        'child-friendly': '0 4px 6px -1px rgba(0, 0, 0, 0.1), 0 2px 4px -1px rgba(0, 0, 0, 0.06)',
        'child-hover': '0 10px 15px -3px rgba(0, 0, 0, 0.1), 0 4px 6px -2px rgba(0, 0, 0, 0.05)',
        'child-active': '0 1px 2px 0 rgba(0, 0, 0, 0.05)'
      },
      scale: {
        '102': '1.02',
      }
    },
  },
  plugins: [
    require('@tailwindcss/forms')({
      strategy: 'class', // Use class-based form styling for better control
    }),
    require('@tailwindcss/typography'),
  ],
  // Safelist commonly used classes that might not be detected in Razor files
  safelist: [
    'btn-child-friendly',
    'card-child',
    'progress-meter',
    'progress-fill',
    'dice-container',
    'territory-card',
    'game-layout',
    'educational-card',
    // Animation classes
    'animate-bounce',
    'animate-pulse',
    'animate-spin',
    // Color variations
    'text-game-primary-500',
    'text-game-secondary-500', 
    'text-game-success-500',
    'bg-game-primary-500',
    'bg-game-secondary-500',
    'bg-game-success-500',
    // Responsive classes
    'sm:text-child-lg',
    'md:text-child-xl',
    'lg:text-child-2xl'
  ]
}
