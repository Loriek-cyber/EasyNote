using Markdig;

namespace EasyNote.Services;

public class MarkdownService
{
    public static string RenderMarkdownLatex(string input)
    {
        // Convert Markdown to HTML with extended features
        var pipeline = new MarkdownPipelineBuilder()
            .UseAdvancedExtensions() // Tables, task lists, etc.
            .Build();
        
        string markdownHtml = Markdown.ToHtml(input, pipeline);

        // Add MathJax for LaTeX rendering with proper configuration
        string fullHtml = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            padding: 20px;
            max-width: 900px;
            margin: 0 auto;
            color: white;
        }}
        code {{
            background-color: #f4f4f4;
            padding: 2px 6px;
            border-radius: 3px;
            font-family: 'Consolas', 'Monaco', monospace;
        }}
        pre {{
            background-color: #f4f4f4;
            padding: 12px;
            border-radius: 5px;
            overflow-x: auto;
        }}
        pre code {{
            background-color: transparent;
            padding: 0;
        }}
        blockquote {{
            border-left: 4px solid #ddd;
            padding-left: 16px;
            margin-left: 0;
            color: white;
        }}
        table {{
            border-collapse: collapse;
            width: 100%;
            margin: 16px 0;
        }}
        th, td {{
            border: 1px solid #ddd;
            padding: 8px 12px;
            text-align: left;
        }}
        th {{
            background-color: #f4f4f4;
        }}
    </style>
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
    {markdownHtml}
</body>
</html>";

        return fullHtml;
    }
}