# EasyNote - Applicazione NoteTaking con Markdown e LaTeX

Un'applicazione desktop per prendere note con supporto completo per Markdown e LaTeX, con un sistema di collegamenti tra documenti simile a Notion.

## Caratteristiche

### 🎨 Interfaccia
- **Tema Nero e Viola**: Design moderno con palette scura e accenti viola
- **Document Explorer**: Sidebar laterale per navigare tra i documenti in modo gerarchico
- **Editor a Blocchi**: Sistema di editing con blocchi simile a Notion

### ✏️ Editor
- **Slash Commands**: Digita `/` per aprire un menu con opzioni di formattazione:
  - 📝 Markdown
  - H1, H2, H3 (Headings)
  - 🔢 LaTeX Inline (`$formula$`)
  - 📐 LaTeX Block (`$$formula$$`)
  - 💻 Code Block
  - 💬 Quote
  - • Bullet List
  - 1. Numbered List

### 🔗 Collegamenti tra Documenti
- Usa la sintassi `<col>NomeDocumento<col>` per creare collegamenti
- Esempio: in documento "A", scrivi `<col>B<col>` per creare un link al documento "B"
- Il path del documento B sarà automaticamente `A/B`
- Cliccando sul collegamento, se il documento non esiste viene creato automaticamente

### 📐 Supporto LaTeX
- Formule inline: `$E = mc^2$`
- Formule block:
  ```
  $$
  \int_{a}^{b} f(x) dx
  $$
  ```

### 📝 Markdown
Supporto completo per:
- Headers (`# ## ###`)
- Liste puntate e numerate
- Code blocks
- Blockquotes
- Grassetto, corsivo, codice inline
- E altro ancora...

## Avvio

### Development Mode
```bash
npm run dev
```

Questo comando avvia contemporaneamente:
- Vite dev server (http://localhost:5173)
- Electron in modalità sviluppo

### Build
```bash
npm run build
```

## Struttura Progetto

```
easymeet/
├── db/
│   ├── db.cjs          # Database layer (SQLite)
│   └── doc.js          # Modello documento
├── electron/
│   ├── main.cjs        # Main process Electron
│   └── preload.cjs     # Preload script per API sicure
├── src/
│   └── view/
│       ├── components/
│       │   ├── Sidebar.jsx           # Navigazione documenti
│       │   ├── Editor.jsx            # Editor principale
│       │   ├── ContentBlock.jsx      # Blocco di contenuto singolo
│       │   ├── SlashMenu.jsx         # Menu slash commands
│       │   └── MarkdownRenderer.jsx  # Renderer Markdown/LaTeX
│       ├── App.jsx         # Componente principale
│       ├── App.css         # Stili componenti
│       ├── index.css       # Stili globali
│       └── main.jsx        # Entry point React
└── index.html
```

## Database

L'applicazione usa SQLite (better-sqlite3) per memorizzare i documenti.
Il database è salvato in: `%APPDATA%/easynote/easymeet.db`

### Schema

```sql
CREATE TABLE documents (
  id TEXT PRIMARY KEY,
  title TEXT NOT NULL,
  content TEXT NOT NULL,
  path TEXT NOT NULL UNIQUE,
  parent_path TEXT,
  created_at INTEGER,
  updated_at INTEGER
)
```

## Tecnologie

- **Electron**: Framework desktop
- **React**: UI framework
- **Vite**: Build tool
- **SQLite**: Database locale
- **ReactMarkdown**: Parsing Markdown
- **KaTeX**: Rendering LaTeX
- **remark-math / rehype-katex**: Plugin per LaTeX in Markdown

## Shortcuts

- `Enter`: Crea nuovo blocco
- `Backspace` (su blocco vuoto): Elimina blocco
- `/`: Apri menu comandi
- `↑/↓`: Naviga menu slash
- `Enter` (in menu): Seleziona comando
- `Esc`: Chiudi menu slash

## Esempio di Utilizzo

1. Avvia l'applicazione con `npm run dev`
2. Clicca "+ New" per creare un nuovo documento
3. Scrivi il titolo del documento
4. Nell'editor, digita `/` per vedere le opzioni disponibili
5. Per creare un collegamento, scrivi: `Questo è un link a <col>AltroDocumento<col>`
6. Clicca sul pulsante "👁️ Preview" per vedere il rendering finale
7. Clicca sul collegamento per navigare o creare automaticamente il documento collegato

## Note

- I documenti vengono salvati automaticamente mentre scrivi (debounce di 500ms)
- La struttura gerarchica è basata sui path: un documento figlio avrà path `ParentPath/ChildName`
- Il rendering LaTeX richiede la sintassi corretta di KaTeX
