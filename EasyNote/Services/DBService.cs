// Source - https://stackoverflow.com/a
// Posted by DA., modified by community. See post 'Timeline' for change history
// Retrieved 2025-11-17, License - CC BY-SA 4.0

using System;
using System.IO;
using System.Data;
using System.Data.SQLite;

namespace EasyNote.Services;

public class DBService : IDisposable, IAsyncDisposable
{
    private readonly SQLiteConnection _connection;
    private const string DbFileName = "EasyNote.db";

    // Correctly determine the base directory for the application.
    // For a desktop app, this is more reliable than assuming the current directory.
    private static readonly string BaseDirectory = AppDomain.CurrentDomain.BaseDirectory;
    private static readonly string DbPath = Path.Combine(BaseDirectory, "Database");
    private static readonly string ConnectionString = $"Data Source={Path.Combine(DbPath, DbFileName)}";

    public DBService()
    {
        if (!Directory.Exists(DbPath))
        {
            Directory.CreateDirectory(DbPath);
        }
        _connection = new SQLiteConnection(ConnectionString);
        try
        {
            _connection.Open();
            Console.WriteLine("[SQLite] Connection opened successfully to: " + ConnectionString);
        }
        catch (Exception e)
        {
            Console.WriteLine($"[SQLite] Error opening connection: {e.Message}");
            throw;
        }
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
            // Depending on the application's needs, you might want to re-throw the exception
            // or handle it in a way that informs the user.
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
            throw; // Re-throwing the exception to let the caller handle it.
        }
        return result;
    }

    public void Dispose()
    {
        _connection.Close();
    }

    public async ValueTask DisposeAsync()
    {
        Dispose();
    }
}
