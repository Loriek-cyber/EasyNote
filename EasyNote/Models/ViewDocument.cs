using EasyNote.Services;

namespace EasyNote.Models;

public class ViewDocument
{
    public string OriginalText { get; set; }
    public string Html { get; private set; }

    public ViewDocument()
    {
        OriginalText = string.Empty;
        Html = string.Empty;
    }

    public void ToHtml()
    {
        // Validate input before processing
        if (string.IsNullOrWhiteSpace(OriginalText))
        {
            Html = MarkdownService.RenderMarkdownLatex("# Empty Document");
            return;
        }

        Html = MarkdownService.RenderMarkdownLatex(OriginalText);
    }
}