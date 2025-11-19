using EasyNote.Services;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;

namespace EasyNote.Models;

public class DocumentDAO
{
    private const string CreateTableQuery = $@"
        CREATE TABLE IF NOT EXISTS Document (
            Id TEXT PRIMARY KEY,
            Path TEXT NOT NULL UNIQUE,
            Title TEXT NOT NULL,
            Content TEXT,
            LastModified TEXT NOT NULL
        );";

    public DocumentDAO()
    {
        using (var db = new DBService())
        {
            db.ExecuteNonQuery(CreateTableQuery);
        }
    }

    public void Insert(Document doc)
    {
        const string query = "INSERT OR IGNORE INTO Document (Id, Path, Title, Content, LastModified) VALUES (@Id, @Path, @Title, @Content, @LastModified)";
        using (var db = new DBService())
        {
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@Id", doc.Id),
                new SQLiteParameter("@Path", doc.Path),
                new SQLiteParameter("@Title", doc.Title),
                new SQLiteParameter("@Content", doc.Content),
                new SQLiteParameter("@LastModified", doc.LastModified.ToString("o")) // ISO 8601 format
            };
            db.ExecuteNonQuery(query, parameters);
        }
    }

    public void Update(Document doc)
    {
        const string query = "UPDATE Document SET Title = @Title, Content = @Content, LastModified = @LastModified WHERE Id = @Id";
        using (var db = new DBService())
        {
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@Title", doc.Title),
                new SQLiteParameter("@Content", doc.Content),
                new SQLiteParameter("@LastModified", doc.LastModified.ToString("o")), // ISO 8601 format
                new SQLiteParameter("@Id", doc.Id)
            };
            db.ExecuteNonQuery(query, parameters);
        }
    }

    public Document GetByPath(string path)
    {
        const string query = "SELECT * FROM Document WHERE Path = @Path";
        using (var db = new DBService())
        {
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@Path", path)
            };
            var dt = db.SelectQuery(query, parameters);
            if (dt.Rows.Count > 0)
            {
                var row = dt.Rows[0];
                return new Document
                {
                    Id = row["Id"].ToString(),
                    Path = row["Path"].ToString(),
                    Title = row["Title"].ToString(),
                    Content = row["Content"].ToString(),
                    LastModified = DateTime.Parse(row["LastModified"].ToString())
                };
            }
        }
        return null;
    }
    
    public List<Document> GetAllDocuments()
    {
        const string query = "SELECT * FROM Document ORDER BY Title";
        var documents = new List<Document>();
        using (var db = new DBService())
        {
            var dt = db.SelectQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                documents.Add(new Document
                {
                    Id = row["Id"].ToString(),
                    Path = row["Path"].ToString(),
                    Title = row["Title"].ToString(),
                    Content = row["Content"].ToString(),
                    LastModified = DateTime.Parse(row["LastModified"].ToString())
                });
            }
        }
        return documents;
    }

    public void Delete(string id)
    {
        const string query = "DELETE FROM Document WHERE Id = @Id";
        using (var db = new DBService())
        {
            var parameters = new SQLiteParameter[]
            {
                new SQLiteParameter("@Id", id)
            };
            db.ExecuteNonQuery(query, parameters);
        }
    }
}
