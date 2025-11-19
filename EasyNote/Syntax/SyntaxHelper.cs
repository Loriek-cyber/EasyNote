using System;
using System.IO;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;

namespace EasyNote.Syntax;

public static class SyntaxHelper
{
    // 1. Define Lazy properties. This ensures the file is read only once, on demand.
    private static readonly Lazy<IHighlightingDefinition> _cpp = new(() => Load("Cpp"));
    private static readonly Lazy<IHighlightingDefinition> _java = new(() => Load("Java"));
    private static readonly Lazy<IHighlightingDefinition> _python = new(() => Load("Python"));
    private static readonly Lazy<IHighlightingDefinition> _json = new(() => Load("JSON"));
    private static readonly Lazy<IHighlightingDefinition> _php = new(() => Load("PHP"));
    private static readonly Lazy<IHighlightingDefinition> _latex = new(() => Load("Latex"));
    private static readonly Lazy<IHighlightingDefinition> _powerShell = new(() => Load("PowerShell"));
    private static readonly Lazy<IHighlightingDefinition> _javaScript = new(() => Load("JavaScript"));
    private static readonly Lazy<IHighlightingDefinition> _sql = new(() => Load("SQL"));
    private static readonly Lazy<IHighlightingDefinition> _typeScript = new(() => Load("TypeScript"));
    private static readonly Lazy<IHighlightingDefinition> _markdown = new(() => Load("Markdown"));

    // 2. Expose them as public IHighlightingDefinition properties
    public static IHighlightingDefinition Cpp => _cpp.Value;
    public static IHighlightingDefinition Java => _java.Value;
    public static IHighlightingDefinition Python => _python.Value;
    public static IHighlightingDefinition Json => _json.Value;
    public static IHighlightingDefinition Php => _php.Value;
    public static IHighlightingDefinition Latex => _latex.Value;
    public static IHighlightingDefinition PowerShell => _powerShell.Value;
    public static IHighlightingDefinition JavaScript => _javaScript.Value;
    public static IHighlightingDefinition Sql => _sql.Value;
    public static IHighlightingDefinition TypeScript => _typeScript.Value;
    public static IHighlightingDefinition Markdown => _markdown.Value;

    // Adjust this path if your folder is named "WebScript/Syntax" or just "Syntax"
    private static readonly string SyntaxDirectory = Path.Combine(AppContext.BaseDirectory, "Syntax");

    /// <summary>
    /// Helper method to load the .xshd file and convert it to an IHighlightingDefinition
    /// </summary>
    private static IHighlightingDefinition Load(string filename)
    {
        var filePath = Path.Combine(SyntaxDirectory, $"{filename}.xshd");

        if (!File.Exists(filePath))
        {
            // You might want to return null here if you don't want to crash, 
            // but throwing is safer for debugging missing files.
            throw new FileNotFoundException($"Unable to locate syntax definition '{filename}'.", filePath);
        }

        try
        {
            using (var stream = File.OpenRead(filePath))
            using (var reader = XmlReader.Create(stream))
            {
                // This is the core AvalonEdit method that parses the .xshd
                return HighlightingLoader.Load(reader, HighlightingManager.Instance);
            }
        }
        catch (Exception ex)
        {
            // Wrap the exception to make it clear which file failed
            throw new InvalidOperationException($"Failed to parse syntax definition for {filename}", ex);
        }
    }
}