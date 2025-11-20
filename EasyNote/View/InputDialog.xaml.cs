using System.Windows;

namespace EasyNote.View;

public partial class InputDialog : Window
{
    public string UserInput { get; set; } = "";
    public string Message { get; set; }

    public InputDialog(string message)
    {
        InitializeComponent();
        Message = message;
        DataContext = this;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    public static string? Show(string message)
    {
        var dialog = new InputDialog(message);
        bool? result = dialog.ShowDialog();
        return result == true ? dialog.UserInput : null;
    }
    
}
