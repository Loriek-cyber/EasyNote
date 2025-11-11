using System.Windows.Controls;
using System.Windows.Input;
using EasyNote.Services;
namespace EasyNote.View;

public partial class TerminalView : UserControl
{
    // Riferimento al Visualer da aggiornare

    public TerminalView()
    {
        InitializeComponent();
    }

    private async void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && sender is TextBox textBox)
        {
            string text = textBox.Text;

            if (!string.IsNullOrWhiteSpace(text))
            {
                try
                {
                    string currentText = VisualerService.ActiveView?.OriginalText ?? "";
                    await VisualerService.UpdateContentAsync(currentText + "\n" + text);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Update error: {ex.Message}");
                }
            }

            textBox.Clear();
            e.Handled = true;
        }
    }
}