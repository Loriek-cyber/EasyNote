using System;
using System.Data;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace EasyNote.Services
{
    public class DBService : IAsyncDisposable
    {
        // String per creare la tabella del database
        public const string CreateTableQuery = @"
            CREATE TABLE IF NOT EXISTS Document (
                Id TEXT PRIMARY KEY,
                Path TEXT NOT NULL UNIQUE,
                Title TEXT NOT NULL,
                Content TEXT,
                LastModified TEXT NOT NULL
            );";

        private readonly SQLiteConnection _connection;
        public string DbPath { get; }

        public DBService(string dbPath)
        {
            if (string.IsNullOrWhiteSpace(dbPath))
                throw new ArgumentException("Percorso del database non valido.", nameof(dbPath));

            DbPath = dbPath;

            string connectionString = $"Data Source={DbPath};Version=3;";
            _connection = new SQLiteConnection(connectionString);

            try
            {
                _connection.Open();
                Console.WriteLine("[SQLite] Connessione stabilita.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[SQLite] Errore connessione: {ex.Message}");
                throw; // Propaga l'errore
            }
        }

        /// <summary>
        /// Esegue una query di selezione e restituisce il risultato in un DataTable.
        /// </summary>
        public DataTable SelectQuery(string query, SQLiteParameter[]? parameters = null)
        {
            if (_connection.State != ConnectionState.Open)
                throw new NotConnectedException("Connessione non aperta.");

            var dt = new DataTable();

            try
            {
                using var cmd = new SQLiteCommand(query, _connection);

                if (parameters is not null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                using var adapter = new SQLiteDataAdapter(cmd);
                adapter.Fill(dt);
            }
            catch (SQLiteException ex)
            {
                Console.WriteLine($"[SQLite] Error in SelectQuery: {ex.Message}");
                throw;
            }

            return dt;
        }

        /// <summary>
        /// Esegue una query di tipo non-query (INSERT, UPDATE, DELETE) e restituisce il numero di righe interessate.
        /// </summary>
        public int ExecuteNonQuery(string query, SQLiteParameter[]? parameters = null)
        {
            if (_connection.State != ConnectionState.Open)
                throw new NotConnectedException("Connessione non aperta.");

            try
            {
                using var cmd = new SQLiteCommand(query, _connection);

                if (parameters is not null && parameters.Length > 0)
                {
                    cmd.Parameters.AddRange(parameters);
                }

                return cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine($"[SQLite] Error in ExecuteNonQuery: {e.Message}");
                throw;
            }
        }

        // Pattern Dispose corretto
        public void Dispose()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }

            _connection.Dispose();
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection.State == ConnectionState.Open)
            {
                await _connection.CloseAsync();
            }

            await _connection.DisposeAsync();
            GC.SuppressFinalize(this);
        }

        // Eccezione personalizzata
        public class NotConnectedException : Exception
        {
            public NotConnectedException(string message)
                : base("[DBService] " + message)
            {
            }
        }
    }
}
