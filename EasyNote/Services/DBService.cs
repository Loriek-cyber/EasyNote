using System;
using System.IO;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;
using Microsoft.Win32; 

namespace EasyNote.Services;

public class DBService : IDisposable, IAsyncDisposable
{
    //connection ReadOnly
    private readonly SQLiteConnection _connection;
    // Percorso del DB globale
    public static string DbPath { get; set; }

    public DBService()
    {
        if (string.IsNullOrEmpty(DbPath)) 
            throw new NotConnectedException("Nessun database selezionato. Usa NewDb o OpenDb prima di istanziare il servizio.");
        // Configurazione stringa di connessione
        
        string connectionString = $"Data Source={DbPath};Version=3;";

        _connection = new SQLiteConnection(connectionString);

        try
        {
            // CORREZIONE 2: Bisogna aprire la connessione!
            _connection.Open();
        }
        catch (Exception ex)
        {
            throw new NotConnectedException($"Impossibile aprire il database: {ex.Message}");
        }
    }
    
    /*
     * Gestione Percorso DB (Statici)
     */

    /// <summary>
    /// Crea un nuovo file DB scegliendo posizione e nome con il dialogo di Windows.
    /// </summary>
    
    
    public static bool NewDb()
    {
        var saveDialog = new SaveFileDialog
        {
            Title = "Crea nuovo database SQLite",
            FileName = "EasyNote.db",
            DefaultExt = ".db",
            Filter = "Database SQLite (*.db)|*.db|Tutti i file (*.*)|*.*"
        };

        bool? result = saveDialog.ShowDialog();

        if (result == true && !string.IsNullOrWhiteSpace(saveDialog.FileName))
        {
            DbPath = saveDialog.FileName;
            Console.WriteLine("[DBService] Setting database path to " + DbPath);

            // Crea un database SQLite valido
            if (!File.Exists(DbPath))
            {
                SQLiteConnection.CreateFile(DbPath);
            }

            // Inizializza il DB (SQLite scrive l'header e lo rende "valido")
            using (var conn = new SQLiteConnection("Data Source=" + DbPath))
            {
                conn.Open();
            }
            
            return true;
        }

        return false;
    }


    /// <summary>
    /// Apre un database esistente scegliendolo con il dialogo di Windows.
    /// </summary>
    public static bool OpenDb()
    {
        var openDialog = new OpenFileDialog
        {
            Title = "Apri database SQLite",
            DefaultExt = ".db",
            Filter = "Database SQLite (*.db)|*.db|Tutti i file (*.*)|*.*"
        };

        bool? result = openDialog.ShowDialog();

        if (result == true && !string.IsNullOrWhiteSpace(openDialog.FileName))
        {
            DbPath = openDialog.FileName;
            return true;
        }

        return false;
    }

    
    /*
     * <summary>
     * Questa sezione riguarda l'esecuzione delle query.
     * </summary>
     */
    
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
            // In produzione potresti voler rilanciare l'eccezione o loggarla su file
            throw; 
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

    // Pattern Dispose corretto
    public void Dispose()
    {
        if (_connection != null)
        {
            if (_connection.State == ConnectionState.Open)
                _connection.Close();
            
            _connection.Dispose();
        }
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        if (_connection is not null)
        {
             await _connection.CloseAsync();
             await _connection.DisposeAsync();
        }
        GC.SuppressFinalize(this);
    }

    // Eccezione personalizzata
    public class NotConnectedException : Exception
    {
        public NotConnectedException(string message) : base("[DBService] " + message) { }
    }
}