# 🔷 EasyNote - Modern Note-Taking with LaTeX & AI

![Version](https://img.shields.io/badge/version-1.0.0-blue)
![C#](https://img.shields.io/badge/C%23-12.0-blueviolet)
![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![WPF](https://img.shields.io/badge/WPF-Windows-green)
![License](https://img.shields.io/badge/license-MIT-lightgrey)

**EasyNote** è un'applicazione desktop moderna progettata per la presa appunti avanzata, con supporto nativo per **Markdown**, rendering **LaTeX** in tempo reale e integrazione con l'**Intelligenza Artificiale** tramite modelli locali.
Sviluppata in C# con WPF, EasyNote offre un ambiente di scrittura pulito ed efficiente, ideale per studenti, ricercatori e sviluppatori che necessitano di strumenti matematici e assistenza AI senza compromettere la privacy.

> [!NOTE]
> **Stato del Progetto:** Questo è un progetto personale nato per approfondire lo sviluppo in C# e l'integrazione di strumenti di editing avanzati.

---

#### 📑 Indice
*   [Caratteristiche Principali](#-caratteristiche-principali)
*   [Integrazione AI (Ollama)](#-integrazione-ai-ollama)
*   [Tech Stack](#-tech-stack)
*   [Struttura del Progetto](#-struttura-del-progetto)
*   [Installazione e Configurazione](#-installazione-e-configurazione)
*   [Syntax Highlighting](#-syntax-highlighting)
*   [Autore](#-autore)

---

#### 🚀 Caratteristiche Principali
*   **Markdown & LaTeX:** Scrittura fluida in Markdown con supporto completo per formule matematiche tramite MathJax.
*   **Preview in Tempo Reale:** Rendering immediato tramite WebView2 per una visualizzazione fedele dei documenti.
*   **Editor Avanzato:** Basato su AvalonEdit, con supporto a temi e syntax highlighting personalizzato.
*   **AI Assistant:** Generazione di testo e assistenza alla scrittura tramite modelli LLM locali (Ollama).
*   **Storage Locale:** Gestione efficiente dei documenti tramite database SQLite e LiteDB.
*   **Visualizer & Terminal:** Strumenti integrati per la gestione dei file e l'esecuzione di comandi.

---

#### 🤖 Integrazione AI (Ollama)

EasyNote sfrutta la potenza di **Ollama** per offrire funzionalità AI completamente locali:
*   **Generazione Testo:** Chiedi all'AI di espandere i tuoi appunti o riassumere concetti.
*   **Selezione Modello:** Supporto dinamico per qualsiasi modello installato localmente (Llama3, Mistral, ecc.).
*   **Streaming:** Visualizzazione della risposta in tempo reale per un'esperienza interattiva.

*Nota: È necessario che Ollama sia installato e in esecuzione su `http://localhost:11434`.*

---

#### 💻 Tech Stack

##### Core & UI
*   **C# 12 / .NET 8**
*   **WPF (Windows Presentation Foundation)**
*   **AvalonEdit** (Editor di testo con syntax highlighting)
*   **Syncfusion WPF Controls** (UI components)
*   **WebView2** (Browser engine per la preview)

##### Librerie di Supporto
*   **Markdig** (Processore Markdown)
*   **MathJax 3** (Rendering formule LaTeX)
*   **Highlight.js** (Evidenziazione del codice nel rendering HTML)

##### Database & Storage
*   **SQLite** (Archiviazione documenti e metadati)
*   **LiteDB** (NoSQL per archiviazione documentale veloce)

---

#### 📂 Struttura del Progetto

Il progetto segue una struttura modulare suddivisa per responsabilità:

```text
EasyNote
├───Models          # Definizione dati e DAO (Document, DocumentDAO)
├───Services        # Logica di business (AiService, MarkdownService, DBService)
├───View            # Interfacce utente XAML (MainWindow, Settings, Visualer)
├───Syntax          # Definizioni per l'evidenziazione del codice (.xshd)
└───WebScript       # Asset per il rendering WebView2 (MathJax, Highlight.js)
```

---

#### 🛠️ Installazione e Configurazione

**Requisiti:**
*   Windows 10/11
*   .NET 8.0 SDK
*   [Ollama](https://ollama.com/) (opzionale, per le funzioni AI)

**Passaggi per lo sviluppo:**

1.  **Clonare il repository:**
    ```bash
    git clone https://github.com/ArjelBuzi/EasyNote
    ```

2.  **Ripristino Dipendenze:**
    ```bash
    dotnet restore
    ```

3.  **Compilazione ed Esecuzione:**
    ```bash
    dotnet run --project EasyNote/EasyNote.csproj
    ```

---

#### ✍️ Syntax Highlighting

L'editor supporta nativamente l'evidenziazione per numerosi linguaggi grazie alle definizioni XSHD:
*   **Web:** HTML, CSS, JavaScript, TypeScript, JSON
*   **Linguaggi:** C++, Java, Python, PHP, SQL
*   **Scripting:** PowerShell, LaTeX, Markdown

---

#### 👤 Autore

*   **Arjel Buzi** - *Sviluppatore principale* - [Profilo GitHub](https://github.com/ArjelBuzi)

---
*Progetto realizzato a scopo didattico per l'apprendimento delle tecnologie .NET e WPF.*
