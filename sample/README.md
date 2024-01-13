# Blazor Accessibility Sample

## Tailwind

Download standalone CLI TailwindCSS here: [tailwindcss releases](https://github.com/tailwindlabs/tailwindcss/releases)

### Init

```bash
./tailwindcss init
```

Update tailwind.config.js:

```javascript
module.exports = {
  content: ["./Components/**/*.{razor,html,cshtml}"],
  theme: {
    extend: {},
  },
  plugins: [],
};
```

### Watch

```bash
./tailwindcss -i ./Styles/app.css -o ./wwwroot/app.css --watch
```
