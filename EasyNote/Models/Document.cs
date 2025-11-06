namespace EasyNote.Models;

public class Document {
    private string _title;
    private string _text;
    public Document(string title, string text)
    {
        _title = title;
        _text = text;
    }
    
    //il return è il get? ma che diavolo? va be
    public string Text
    {
        get => _text;
    }

    public string Title
    {
        get => _title;
    }
}