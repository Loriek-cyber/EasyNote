using System;

namespace EasyNote.Models
{
    public class Document
    {
        public string Id { get; set; }           // LiteDB usa string/Guid
        public string Path {get; set;}          //il path per la gestione delle cartelle all'interno dal file explorer
        public string Title { get; set; }
        public string Content { get; set; }      // Markdown o LaTeX// "Markdown" | "LaTeX"
        public DateTime LastModified { get; set; }
        public string toString()
        {
            return string.Format("Title: {0}, Content: {1}", Title, Path);
        }

        public string Markdown()
        {
            return $@"_{Path}  : modificato: {LastModified.ToString()}_"+"\n"+
                   $@"#{Title}"+"\n"+
                   $@"{Content}"+"\n";
        }
    }
    
}