using System.Xml.Linq;
using Markdig;

namespace EasyNote.Services;

public class MarkdownService
{
    
    public static string RenderMarkdownLatex(string input)
    {
        
        // Converte Markdown in HTML con funzionalità estese
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions() // Tabelle, liste di task, ecc.
            // NOTE: Per un highlighting funzionante, è necessario usare un'estensione Markdig
            // come Markdig.SyntaxHighlighting nella pipeline, che genera classi CSS come 'keyword', 'string', ecc.
            .Build();
        
        string markdownHtml = Markdown.ToHtml(input, pipeline);

        // Aggiunge MathJax per il rendering LaTeX e il nuovo tema scuro con highlighting
        string fullHtml = $@"
<!DOCTYPE html>
<html lang='it'>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>EasyNote Render</title>
    
    <!-- Avviso: MathJax è un script esterno e richiede una connessione Internet per il rendering LaTeX/Math. -->
    
    <style>
        /* ================================================= */
        /* --- 1. CONFIGURAZIONE GLOBALE E TEMA SCURO --- */
        /* ================================================= */
        body {{
            background-color: #0a192f; /* Blu scuro (navy) - Colore principale */
            color: #e0e0e0; /* Grigio chiaro per testo */
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.7;
            padding: 25px;
            max-width: 900px;
            margin: 0 auto;
        }}

        /* Stile per Evidenziazione Testo (più facile da evidenziare) */
        ::selection {{
            background-color: #007bff; /* Blu brillante per evidenziazione */
            color: #ffffff;
        }}
        ::-moz-selection {{
            background-color: #007bff;
            color: #ffffff;
        }}

        /* ================================================= */
        /* --- 2. PULSANTE DI STAMPA --- */
        /* ================================================= */
        .print-button-container {{
            text-align: right;
            margin: -10px 0 20px 0;
            padding: 0;
        }}
        .print-button {{
            background-color: #007bff;
            color: white;
            border: none;
            padding: 10px 18px;
            border-radius: 6px;
            font-weight: 600;
            font-size: 15px;
            cursor: pointer;
            transition: background-color 0.2s ease, transform 0.1s ease;
        }}
        .print-button:hover {{
            background-color: #0056b3;
        }}
        .print-button:active {{
            transform: translateY(1px);
        }}

        /* ================================================= */
        /* --- 3. STILI DEL CONTENUTO MARKDOWN E CODE HIGHLIGHTING --- */
        /* ================================================= */

        /* Link */
        a {{
            color: #61dafb; /* Ciano brillante */
            text-decoration: none;
        }}
        a:hover {{
            color: #21a1c4;
            text-decoration: underline;
        }}

        /* Titoli (sottolinea il contrasto) */
        h1, h2, h3, h4, h5, h6 {{
            color: #ffffff;
            border-bottom: 1px solid #304a6e; /* Linea di separazione sottile */
            padding-bottom: 8px;
            margin-top: 1.5em;
        }}
        h1 {{ border-bottom-width: 2px; }}

        /* Codice Inline */
        code {{
            background-color: #1e3a5f; /* Blu più scuro */
            color: #f1f1f1;
            padding: 3px 7px;
            border-radius: 5px;
            font-family: 'Consolas', 'Monaco', 'SFMono-Regular', monospace;
            font-size: 0.9em;
        }}

        /* Blocchi di Codice (Pre) - Sfondo scuro per highlighting */
        pre {{
            background-color: #1e3a5f;
            padding: 16px;
            border-radius: 8px;
            overflow-x: auto;
            border: 1px solid #304a6e;
            box-shadow: 0 4px 10px rgba(0, 0, 0, 0.3); /* Ombra per profondità */
            position: relative; /* Per posizionamento elementi interni */
        }}
        pre code {{
            background-color: transparent;
            padding: 0;
            border: none;
            font-size: 0.9em;
            line-height: 1.5;
        }}

        /* Stili per le classi di Syntax Highlighting (Palette Monokai-like) */
        /* Questi stili funzioneranno solo se il parser Markdig inietta classi come 'keyword' ecc. */
        .markdown-body pre code span.keyword,
        .markdown-body pre code span.k {{
            color: #f92672; /* Rosa/Rosso per parole chiave */
            font-weight: bold;
        }}
        .markdown-body pre code span.comment,
        .markdown-body pre code span.c {{
            color: #75715e; /* Grigio scuro per commenti */
            font-style: italic;
        }}
        .markdown-body pre code span.string,
        .markdown-body pre code span.s {{
            color: #e6db74; /* Giallo per stringhe */
        }}
        .markdown-body pre code span.type,
        .markdown-body pre code span.t {{
            color: #66d9ef; /* Ciano per tipi/classi */
        }}
        .markdown-body pre code span.number,
        .markdown-body pre code span.n {{
            color: #ae81ff; /* Viola per numeri */
        }}
        .markdown-body pre code span.name,
        .markdown-body pre code span.m {{
             color: #a6e22e; /* Verde per funzioni/variabili */
        }}


        /* Citazioni (Blockquote) */
        blockquote {{
            border-left: 5px solid #007bff; /* Blu brillante */
            padding-left: 20px;
            margin-left: 0;
            color: #b0c4de;
            font-style: italic;
        }}

        /* Tabelle */
        table {{
            border-collapse: collapse;
            width: 100%;
            margin: 20px 0;
        }}
        th, td {{
            border: 1px solid #304a6e;
            padding: 10px 14px;
            text-align: left;
        }}
        th {{
            background-color: #1e3a5f;
            color: #ffffff;
        }}
        tr:nth-child(even) {{
            background-color: #0c203b; /* Strisce righe */
        }}

        /* Liste */
        ul, ol {{
            padding-left: 25px;
        }}
        li {{
            margin-bottom: 8px;
        }}

        /* Correzioni MathJax per Tema Scuro */
        .MathJax {{
            color: #e0e0e0 !important;
        }}
        .MathJax_Display {{
            overflow-x: auto;
            overflow-y: hidden;
            padding: 0.5em 0;
        }}
        .MathJax_SVG_LineBreak {{
            display: block !important;
        }}
        
        /* ================================================= */
        /* --- 4. STILI DI STAMPA (MEDIA QUERY) --- */
        /* ================================================= */
        @media print {{
            /* Nascondi il pulsante di stampa */
            .print-button-container {{
                display: none;
            }}
            
            /* Inverti i colori per la stampa su sfondo bianco */
            body {{
                background-color: #ffffff;
                color: #000000;
                padding: 1in; 
                margin: 0;
                max-width: 100%;
            }}

            h1, h2, h3, h4, h5, h6 {{
                color: #000000;
                border-bottom-color: #cccccc;
            }}
            a {{
                color: #0000EE;
                text-decoration: underline;
            }}
            /* Stili di stampa per i blocchi di codice */
            code, pre {{
                background-color: #f4f4f4;
                color: #333;
                border: 1px solid #ddd;
            }}
            /* Resetta l'highlighting a colori scuri su sfondo chiaro */
            .markdown-body pre code span {{ color: #333 !important; font-weight: normal; font-style: normal; }}
            .markdown-body pre code span.keyword, .markdown-body pre code span.k {{ color: #0000ff !important; font-weight: bold; }}
            .markdown-body pre code span.comment, .markdown-body pre code span.c {{ color: #008000 !important; font-style: italic; }}
            .markdown-body pre code span.string, .markdown-body pre code span.s {{ color: #a31515 !important; }}
            
            blockquote {{
                color: #555;
                border-left-color: #ccc;
                background-color: #f9f9f9;
            }}
            table, th, td {{
                border: 1px solid #ccc;
            }}
            th {{
                background-color: #f4f4f4;
                color: #000;
            }}
            tr:nth-child(even) {{
                background-color: #f9f9f9;
            }}
            .MathJax {{
                color: #000000 !important;
            }}
        }}
    </style>

    <!-- Configurazione e Script MathJax -->
    <script>
        window.MathJax = {{
            tex: {{
                inlineMath: [['$', '$'], ['\\(', '\\)']],
                displayMath: [['$$', '$$'], ['\\[', '\\]']]
            }},
            svg: {{
                fontCache: 'global'
            }}
        }};
    </script>
    <script src='https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js' async></script>
</head>
<body>
    <div class='print-button-container'>
        <button class='print-button' onclick='window.print()'><svg xmlns=""http://www.w3.org/2000/svg"" width=""20px"" height=""20px"" viewBox=""0 0 24 24"" fill=""none"">
<path d=""M7 18H6.2C5.0799 18 4.51984 18 4.09202 17.782C3.71569 17.5903 3.40973 17.2843 3.21799 16.908C3 16.4802 3 15.9201 3 14.8V10.2C3 9.0799 3 8.51984 3.21799 8.09202C3.40973 7.71569 3.71569 7.40973 4.09202 7.21799C4.51984 7 5.0799 7 6.2 7H7M17 18H17.8C18.9201 18 19.4802 18 19.908 17.782C20.2843 17.5903 20.5903 17.2843 20.782 16.908C21 16.4802 21 15.9201 21 14.8V10.2C21 9.07989 21 8.51984 20.782 8.09202C20.5903 7.71569 20.2843 7.40973 19.908 7.21799C19.4802 7 18.9201 7 17.8 7H17M7 11H7.01M17 7V5.4V4.6C17 4.03995 17 3.75992 16.891 3.54601C16.7951 3.35785 16.6422 3.20487 16.454 3.10899C16.2401 3 15.9601 3 15.4 3H8.6C8.03995 3 7.75992 3 7.54601 3.10899C7.35785 3.20487 7.20487 3.35785 7.10899 3.54601C7 3.75992 7 4.03995 7 4.6V5.4V7M17 7H7M8.6 21H15.4C15.9601 21 16.2401 21 16.454 20.891C16.6422 20.7951 16.7951 20.6422 16.891 20.454C17 20.2401 17 19.9601 17 19.4V16.6C17 16.0399 17 15.7599 16.891 15.546C16.7951 15.3578 16.6422 15.2049 16.454 15.109C16.2401 15 15.9601 15 15.4 15H8.6C8.03995 15 7.75992 15 7.54601 15.109C7.35785 15.2049 7.20487 15.3578 7.10899 15.546C7 15.7599 7 16.0399 7 16.6V19.4C7 19.9601 7 20.2401 7.10899 20.454C7.20487 20.6422 7.35785 20.7951 7.54601 20.891C7.75992 21 8.03995 21 8.6 21Z"" stroke=""#000000"" stroke-width=""2"" stroke-linecap=""round"" stroke-linejoin=""round""/>
</svg></button>
    </div>

    <div class='markdown-body'>
        {markdownHtml}
    </div>
</body>
</html>";

        return fullHtml;
    }
}