using EasyNote.Services;
using System;
using System.Data;
using System.Data.SQLite;
using System.Collections.Generic;

namespace EasyNote.Models;

public class DocumentDAO : IDisposable
{
    private readonly DBService db;
    private bool disposed = false;

    public DocumentDAO() 
    {
        db = new DBService(App.dbs[^1]);
    }

    public void Insert(Document doc)
    {
        const string query = "INSERT OR IGNORE INTO Document (Id, Path, Title, Content, LastModified) VALUES (@Id, @Path, @Title, @Content, @LastModified)";
        var parameters = new SQLiteParameter[]
        {
            new("@Id", doc.Id),
            new("@Path", doc.Path),
            new("@Title", doc.Title),
            new("@Content", doc.Content),
            new("@LastModified", doc.LastModified.ToString("o"))
        };
        db.ExecuteNonQuery(query, parameters);
    }

    public void Update(Document doc)
    {
        const string query = "UPDATE Document SET Title = @Title, Content = @Content, LastModified = @LastModified WHERE Id = @Id";
        var parameters = new SQLiteParameter[]
        {
            new("@Title", doc.Title),
            new("@Content", doc.Content),
            new("@LastModified", doc.LastModified.ToString("o")),
            new("@Id", doc.Id)
        };
        db.ExecuteNonQuery(query, parameters);
    }

    public Document? GetByPath(string path)
    {
        const string query = "SELECT * FROM Document WHERE Path = @Path";
        var parameters = new SQLiteParameter[]
        {
            new("@Path", path)
        };
        var dt = db.SelectQuery(query, parameters);
        
        if (dt.Rows.Count > 0)
        {
            var row = dt.Rows[0];
            return new Document
            {
                Id = row["Id"].ToString()!,
                Path = row["Path"].ToString()!,
                Title = row["Title"].ToString()!,
                Content = row["Content"].ToString()!,
                LastModified = DateTime.Parse(row["LastModified"].ToString()!)
            };
        }
        return null;
    }
    
    public List<Document> GetAllDocuments()
    {
        const string query = "SELECT * FROM Document ORDER BY Title";
        var documents = new List<Document>();
        var dt = db.SelectQuery(query);
        
        foreach (DataRow row in dt.Rows)
        {
            documents.Add(new Document
            {
                Id = row["Id"].ToString()!,
                Path = row["Path"].ToString()!,
                Title = row["Title"].ToString()!,
                Content = row["Content"].ToString()!,
                LastModified = DateTime.Parse(row["LastModified"].ToString()!)
            });
        }
        return documents;
    }

    public void Delete(string id)
    {
        const string query = "DELETE FROM Document WHERE Id = @Id";
        var parameters = new SQLiteParameter[]
        {
            new("@Id", id)
        };
        db.ExecuteNonQuery(query, parameters);
    }

    public void InsertAll(List<Document> documents)
    {
        foreach (var doc in documents)
        {
            if (doc.Id != null) 
                Update(doc);
            else 
                Insert(doc);
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                db.Dispose();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}