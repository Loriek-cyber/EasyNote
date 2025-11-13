using System;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace EasyNote.Services
{
    public static class HighlightingService
    {
        public static void RegisterCustomHighlighting()
        {
            var syntaxDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Syntax");

            if (!Directory.Exists(syntaxDir))
            {
                // Or handle this case as an error
                return;
            }

            var files = Directory.GetFiles(syntaxDir, "*.xshd");

            foreach (var file in files)
            {
                try
                {
                    var highlightingName = Path.GetFileNameWithoutExtension(file);
                    
                    // Skip if already registered
                    if (HighlightingManager.Instance.GetDefinition(highlightingName) != null)
                    {
                        continue;
                    }

                    using (var reader = new XmlTextReader(file))
                    {
                        var highlighting = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        HighlightingManager.Instance.RegisterHighlighting(highlightingName, [".txt"], highlighting);
                    }
                }
                catch (Exception)
                {
                    // Log or handle the error appropriately
                }
            }
        }
    }
}
