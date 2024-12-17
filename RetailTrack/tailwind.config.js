/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Pages/**/*.cshtml",   // Razor Pages
    "./Views/**/*.cshtml",   // MVC Views
    "./**/*.razor",          // Blazor Components
    "./wwwroot/**/*.html"    // HTML estático
  ],
  theme: {
    extend: {},
  },
  plugins: [],
};
