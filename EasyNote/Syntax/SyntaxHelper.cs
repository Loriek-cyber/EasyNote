using System.Collections;
using System.Xml;

namespace EasyNote.Syntax;

public class SyntaxHelper
{
    public static Syntax cpp = new Syntax("Cpp");
    public static Syntax java = new Syntax("Java");
    public static Syntax python = new Syntax("Python");
    public static Syntax json = new Syntax("JSON");
    public static Syntax php = new Syntax("PHP");
    public static Syntax latex = new Syntax("Latex");
    public static Syntax powershell = new Syntax("PowerShell");
    public static Syntax javascrip = new Syntax("JavaScript");
    public static Syntax sql = new Syntax("SQL");
    public static Syntax typescript = new Syntax("TypeScript");
    public static Syntax markdown = new Syntax("MarkDown");
    private const string Path = @"C:\Users\Arjel Buzi\RiderProjects\EasyNote\EasyNote\Syntax";

    public class Syntax
    {
        private string _name;
        public XmlReader Reader { get; }

        public Syntax(string Name)
        {
            _name = Name;
            try
            {
                string p = (Path + @"\")+_name + ".xshd";
                
                Console.WriteLine("[Temp] File: "+ p);
                Reader = XmlReader.Create(p);
            }
            catch (Exception e)
            {
                Console.WriteLine("[SyntaxHelper] Errore con la lettura del file XSHD");
            }
        }

    }
}