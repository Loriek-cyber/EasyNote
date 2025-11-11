// In Visualer.xaml.cs

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using EasyNote.Services;
namespace EasyNote.View;
public partial class Visualer : UserControl
{
    public Visualer()
    {
        InitializeComponent();
        Loaded += OnLoaded;
    }
    
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await VisualerService.InitAsync(WebViewControl); // 'content' è il nome del WebView2 nel XAML
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Init error: {ex.Message}");
        }
    }
}
