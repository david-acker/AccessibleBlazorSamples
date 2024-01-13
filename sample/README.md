# Blazor Accessibility Sample

## Tailwind

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
