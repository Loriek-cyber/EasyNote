using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;


namespace EasyNote.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private string _title = "";
    private string _body = "";

    public string Title
    {
        get => _title;
        set { _title = value; OnPropertyChanged(); RefreshCommands(); }
    }

    public string Body
    {
        get => _body;
        set { _body = value; OnPropertyChanged(); RefreshCommands(); }
    }

    public ICommand SaveCommand { get; }
    public ICommand ClearCommand { get; }

    public MainViewModel()
    {
      
    }

    private bool CanSave()  => !string.IsNullOrWhiteSpace(Title) || !string.IsNullOrWhiteSpace(Body);
    private bool CanClear() => !string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Body);

    private void Save()
    {
        // Per ora: finta persistenza → messaggio
        MessageBox.Show($"Salvato:\nTitolo: {Title}\n{Body}", "EasyNote");
        Clear();
    }

    private void Clear()
    {
        Title = "";
        Body  = "";
    }

    private void RefreshCommands()
    {
        // forza ricalcolo CanExecute dei bottoni
        System.Windows.Input.CommandManager.InvalidateRequerySuggested();
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}