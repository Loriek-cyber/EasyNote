using System.IO;
using System.Xml;

namespace EasyNote.Syntax;

public static class SyntaxHelper
{
    public static readonly SyntaxDefinition Cpp = new("Cpp");
    public static readonly SyntaxDefinition Java = new("Java");
    public static readonly SyntaxDefinition Python = new("Python");
    public static readonly SyntaxDefinition Json = new("JSON");
    public static readonly SyntaxDefinition Php = new("PHP");
    public static readonly SyntaxDefinition Latex = new("Latex");
    public static readonly SyntaxDefinition PowerShell = new("PowerShell");
    public static readonly SyntaxDefinition JavaScript = new("JavaScript");
    public static readonly SyntaxDefinition Sql = new("SQL");
    public static readonly SyntaxDefinition TypeScript = new("TypeScript");
    public static readonly SyntaxDefinition Markdown = new("MarkDown");

    private static readonly string SyntaxDirectory = Path.Combine(AppContext.BaseDirectory, "Syntax");

    public sealed class SyntaxDefinition
    {
        private readonly string _name;

        internal SyntaxDefinition(string name)
        {
            _name = name;
        }

        public XmlReader CreateReader()
        {
            var filePath = Path.Combine(SyntaxDirectory, $"{_name}.xshd");

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Unable to locate syntax definition '{_name}'.", filePath);
            }

            return XmlReader.Create(filePath);
        }
    }
}
