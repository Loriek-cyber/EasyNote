using Markdig;
using Microsoft.Web.WebView2.Wpf;
public class MarkdownService
{
    public void RenderMarkdownLatex(WebView2 webView, string input)
    {
        // Converte Markdown in HTML
        string markdownHtml = Markdown.ToHtml(input);

        // Aggiunge MathJax per il rendering LaTeX
        string fullHtml = $@"
            <html>
            <head>
              <meta charset='UTF-8'>
              <script src='https://cdn.jsdelivr.net/npm/mathjax@3/es5/tex-mml-chtml.js'></script>
            </head>
            <body>
                {markdownHtml}
            </body>
            </html>";

        webView.NavigateToString(fullHtml);
    }

}