using System;
using System.Text;
using Markdig;

namespace EasyNote.Services;

public class MarkdownService
{
    public static string RenderMarkdownLatex(string? input)
    {
        // 1. Configure Markdig
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions()
            .UseYamlFrontMatter()
            .Build();

        string markdownHtml = Markdown.ToHtml(input ?? "", pipeline);

        // 2. Define Online CDN URLs
        // Highlight.js (Theme: StackOverflow Dark)
        string cssCdn = "";
        // Highlight.js (Core Library)
        string jsHljsCdn = "";
        // MathJax
        string jsMathJaxCdn = "https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js";

        // 3. Construct the HTML
        // NOTE: All CSS/JS braces { } are escaped as {{ }} inside C# interpolated string ($)
        string fullHtml = $@"
        <!DOCTYPE html>
        <html lang='it'>
        <head>
            <meta charset='UTF-8'>
            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
            <title>EasyNote Render</title>
            
            <link rel='stylesheet' href='{cssCdn}'>

            <style>
                /* --- GLOBAL VARIABLES --- */
                :root {{
                    --bg-color: #0a192f;
                    --text-primary: #ccd6f6;
                    --text-secondary: #8892b0;
                    --accent-color: #64ffda;
                    --code-bg: #112240;
                    --border-color: #233554;
                    --font-main: 'Segoe UI', Roboto, Helvetica, Arial, sans-serif;
                    --font-mono: 'Fira Code', Consolas, monospace;
                }}

                /* --- RESET & BODY --- */
                * {{ box-sizing: border-box; }}
                
                body {{
                    background-color: var(--bg-color);
                    color: var(--text-primary);
                    font-family: var(--font-main);
                    line-height: 1.6;
                    padding: 40px;
                    margin: 0;
                    overflow-x: hidden;
                }}

                /* --- SCROLLBARS --- */
                ::-webkit-scrollbar {{ width: 10px; height: 10px; }}
                ::-webkit-scrollbar-track {{ background: var(--bg-color); }}
                ::-webkit-scrollbar-thumb {{ background: #495670; border-radius: 5px; border: 2px solid var(--bg-color); }}
                ::-webkit-scrollbar-thumb:hover {{ background: var(--accent-color); }}

                /* --- TYPOGRAPHY --- */
                h1, h2, h3, h4, h5, h6 {{
                    color: #e6f1ff;
                    margin-top: 24px;
                    margin-bottom: 16px;
                    font-weight: 700;
                    line-height: 1.25;
                }}

                h1 {{ 
                    font-size: 2.5em; 
                    border-bottom: 1px solid var(--border-color); 
                    padding-bottom: 0.3em;
                    background: linear-gradient(90deg, #e6f1ff, #64ffda);
                    -webkit-background-clip: text;
                    -webkit-text-fill-color: transparent;
                }}

                h2 {{ font-size: 2em; border-bottom: 1px solid var(--border-color); padding-bottom: 0.3em; }}
                
                p {{ margin-bottom: 16px; color: var(--text-secondary); }}

                a {{ color: var(--accent-color); text-decoration: none; transition: 0.2s; }}
                a:hover {{ text-decoration: underline; text-shadow: 0 0 5px var(--accent-color); }}

                /* --- CODE BLOCKS & INLINE CODE --- */
                pre {{
                    background-color: var(--code-bg);
                    padding: 16px;
                    border-radius: 8px;
                    overflow: auto;
                    box-shadow: 0 10px 30px -10px rgba(2,12,27,0.7);
                    border: 1px solid var(--border-color);
                }}

                code {{
                    font-family: var(--font-mono);
                    font-size: 0.9em;
                    color: #e6f1ff;
                }}
                
                /* Inline code styling (not inside pre) */
                :not(pre) > code {{
                    background-color: rgba(100, 255, 218, 0.1);
                    color: var(--accent-color);
                    padding: 0.2em 0.4em;
                    border-radius: 4px;
                }}

                /* --- BLOCKQUOTES --- */
                blockquote {{
                    border-left: 4px solid var(--accent-color);
                    background: rgba(17, 34, 64, 0.5);
                    margin: 0 0 16px 0;
                    padding: 16px;
                    font-style: italic;
                    color: var(--text-secondary);
                    border-radius: 0 8px 8px 0;
                }}

                /* --- TABLES --- */
                table {{
                    border-collapse: collapse;
                    width: 100%;
                    margin-bottom: 16px;
                    background-color: var(--code-bg);
                    border-radius: 8px;
                    overflow: hidden;
                    box-shadow: 0 5px 15px rgba(0,0,0,0.3);
                }}
                
                th, td {{
                    padding: 12px 15px;
                    text-align: left;
                    border-bottom: 1px solid var(--border-color);
                }}

                th {{
                    background-color: #233554;
                    color: var(--accent-color);
                    text-transform: uppercase;
                    font-size: 0.85em;
                    letter-spacing: 1px;
                }}

                tr:last-child td {{ border-bottom: none; }}
                tr:hover {{ background-color: #1d2d50; }}

                /* --- IMAGES --- */
                img {{
                    max-width: 100%;
                    border-radius: 8px;
                    box-shadow: 0 10px 30px -15px rgba(0,0,0,0.5);
                    display: block;
                    margin: 20px auto;
                    border: 2px solid var(--border-color);
                }}

                /* --- LISTS --- */
                ul, ol {{ padding-left: 2em; color: var(--text-secondary); }}
                li {{ margin-bottom: 0.5em; }}
                li::marker {{ color: var(--accent-color); }}

                /* --- PRINT BUTTON --- */
                .print-button-container {{
                    position: fixed;
                    top: 20px;
                    right: 20px;
                    z-index: 1000;
                }}

                .print-button {{
                    background-color: transparent;
                    color: var(--accent-color);
                    border: 1px solid var(--accent-color);
                    padding: 10px 20px;
                    border-radius: 4px;
                    cursor: pointer;
                    font-family: var(--font-mono);
                    font-weight: bold;
                    transition: 0.2s;
                }}

                .print-button:hover {{
                    background-color: rgba(100, 255, 218, 0.1);
                    transform: translateY(-2px);
                    box-shadow: 0 5px 15px rgba(0,0,0,0.3);
                }}

                /* --- PRINT MEDIA QUERY --- */
                @media print {{
                    body {{ background-color: white; color: black; }}
                    .print-button-container {{ display: none; }}
                    pre, code, blockquote {{ border: 1px solid #ccc; color: black; background: none; }}
                    h1, h2, a {{ color: black; -webkit-text-fill-color: black; }}
                }}
            </style>

            <script src='{jsHljsCdn}' async onload='hljs.highlightAll()'></script>

            <script>
                window.MathJax = {{
                    tex: {{
                        inlineMath: [['$', '$'], ['\\(', '\\)']],
                        displayMath: [['$$', '$$'], ['\\[', '\\]']]
                    }},
                    svg: {{ fontCache: 'global' }},
                    startup: {{
                        // Opzionale: Log per debug quando mathjax è pronto
                        pageReady: () => {{
                            return MathJax.startup.defaultPageReady().then(() => {{
                                console.log('MathJax loaded and rendered.');
                            }});
                        }}
                    }}
                }};
            </script>
            <script src='{jsMathJaxCdn}' async></script>
        </head>
        <body>
            <div class='print-button-container'>
                <button class='print-button' onclick='window.print()'>⭳</button>
            </div>
            
            <div class='markdown-body'>
                {markdownHtml}
            </div>
        </body>
        </html>";

        return fullHtml;
    }
}