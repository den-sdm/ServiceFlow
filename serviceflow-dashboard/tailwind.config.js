/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'critical': '#DC2626',
        'high': '#EA580C',
        'medium': '#F59E0B',
        'low': '#10B981',
      },
    },
  },
  plugins: [],
}