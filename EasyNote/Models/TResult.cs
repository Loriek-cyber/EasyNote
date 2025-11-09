namespace EasyNote.Models;


/**
 * SO this is the parser for the terminal like expirience
 * @Return string
 */
public class TResult
{
    private string text;
    public string Text
    {
        get { return text; }
    }
    public TResult(string text)
    {
        this.text = text;
    }
}