using System.Windows.Controls;
using System.Windows.Input;

namespace EasyNote.View;

public partial class TerminalView : UserControl
{
    public TerminalView()
    {
        InitializeComponent();
    }


    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && sender is TextBox textBox)
        {
            Console.WriteLine(textBox.Text);  // Do something with the text
            textBox.Clear();                  // Optional: clear the box
            e.Handled = true;                 // Stops the "ding" sound and event bubbling
        }
    }
}