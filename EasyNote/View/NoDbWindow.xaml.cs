using System.Windows;
using EasyNote.Models;
using EasyNote.Services;
namespace EasyNote.View;

public partial class NoDbWindow : Window
{
    public NoDbWindow()
    {
        InitializeComponent();
    }
    
    private void Create_OnClick(object sender, RoutedEventArgs e)
    {
        DBService.NewDb();
        using var db = new DBService();
        db.ExecuteNonQuery(DocumentDAO.CreateTableQuery);
        Close();
        new MainWindow().Show();
    }

    private void Load_OnClick(object sender, RoutedEventArgs e)
    {
        DBService.OpenDb();
        using var db = new DBService();
        db.ExecuteNonQuery(DocumentDAO.CreateTableQuery);
        Close();
        new MainWindow().Show();
    }
}