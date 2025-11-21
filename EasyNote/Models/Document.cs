using System;
using System.Text;

namespace EasyNote.Models
{
    public class Document
    {
        public string Id { get; set; }           // LiteDB usa string/Guid
        public string Path {get; set;}          //il path per la gestione delle cartelle all'interno dal file explorer
        public string Title { get; set; }
        public string Content { get; set; }      // Markdown o LaTeX// "Markdown" | "LaTeX"
        public DateTime LastModified { get; set; }
        

        public string Markdown(bool includeMetadata = true, string dateFormat = "dd/MM/yyyy HH:mm")
        {
            var sb = new StringBuilder();

            if (includeMetadata)
            {
                sb.AppendLine($"*{Path} — Modificato: {LastModified.ToString(dateFormat)}*");
                sb.AppendLine();
            }

            sb.AppendLine($"# {Title}");
            sb.AppendLine();
            sb.Append(Content);

            return sb.ToString();
        }
    }
    
}