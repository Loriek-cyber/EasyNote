using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using Microsoft.Win32; // <--- IMPORTANTE per OpenFileDialog / SaveFileDialog

namespace EasyNote.Services;

public class DBService : IDisposable, IAsyncDisposable
{
    private readonly SQLiteConnection _connection;
    private const string DbFileName = "EasyNote.db";

    // Directory di default (come prima)
    private static string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private static string DbPath = Path.Combine(BaseDirectory, "Database");
    private static string DefaultDbFullPath = Path.Combine(DbPath, DbFileName);

    public DBService()
    {
        if (!Directory.Exists(DbPath))
        {
            Directory.CreateDirectory(DbPath);
        }

        // Creo la connessione ma NON la apro ancora
        _connection = new SQLiteConnection
        {
            ConnectionString = $"Data Source={DefaultDbFullPath}"
        };
    }

    // Proprietà per sapere dove stai puntando
    public string CurrentDatabasePath => _connection.ConnectionString;

    /// <summary>
    /// Crea un nuovo file DB scegliendo posizione e nome con il dialogo di Windows.
    /// Restituisce true se l'utente ha scelto un file e la connessione è stata aperta.
    /// </summary>
    public static void NewDb()
    {
        var saveDialog = new SaveFileDialog
        {
            Title = "Crea nuovo database SQLite",
            FileName = "EasyNote.db",
            DefaultExt = ".db",
            Filter = "Database SQLite (*.db)|*.db|Tutti i file (*.*)|*.*"
        };

        bool? result = saveDialog.ShowDialog();

        if (result != true)
            return; // Utente ha annullato
        string selectedPath = saveDialog.FileName;
        DbPath = selectedPath;
    }

    /// <summary>
    /// Apre un database esistente scegliendolo con il dialogo di Windows.
    /// </summary>
    public static void OpenDb()
    {
        var openDialog = new OpenFileDialog
        {
            Title = "Apri database SQLite",
            DefaultExt = ".db",
            Filter = "Database SQLite (*.db)|*.db|Tutti i file (*.*)|*.*"
        };
        bool? result = openDialog.ShowDialog();
        if (result != true)
            return; // Utente ha annullato
        string selectedPath = openDialog.FileName;
        DbPath = selectedPath;
    }

    public DataTable SelectQuery(string query, SQLiteParameter[] parameters = null)
    {
        var dt = new DataTable();
        try
        {
            using var cmd = new SQLiteCommand(query, _connection);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            using var adapter = new SQLiteDataAdapter(cmd);
            adapter.Fill(dt);
        }
        catch (SQLiteException ex)
        {
            Console.WriteLine($"[SQLite] Error in SelectQuery: {ex.Message}");
        }
        return dt;
    }

    public int ExecuteNonQuery(string query, SQLiteParameter[] parameters = null)
    {
        int result = -1;
        try
        {
            using var cmd = new SQLiteCommand(query, _connection);
            if (parameters != null)
            {
                cmd.Parameters.AddRange(parameters);
            }
            result = cmd.ExecuteNonQuery();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[SQLite] Error in ExecuteNonQuery: {e.Message}");
            throw;
        }
        return result;
    }

    public void Dispose()
    {
        if (_connection.State == ConnectionState.Open)
            _connection.Close();
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();
    }
}
