namespace EasyNote.Models
{
    public class Document
    {
        public string Id { get; set; }           // LiteDB usa string/Guid
        public string Path {get; set;}          //il path per la gestione delle cartelle all'interno dal file explorer
        public string Title { get; set; }
        public string Content { get; set; }      // Markdown o LaTeX
        public string Mode { get; set; }         // "Markdown" | "LaTeX"
        public DateTime LastModified { get; set; }
    }
}