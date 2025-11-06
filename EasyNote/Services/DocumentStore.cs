using LiteDB;
using EasyNote.Models;

namespace EasyNote.Services
{
    public class DocumentStore : IDisposable
    {
        private readonly LiteDatabase _db;
        private readonly ILiteCollection<Document> _col;

        public DocumentStore(string dbPath = "easynote.db")
        {
            _db = new LiteDatabase(dbPath);
            _col = _db.GetCollection<Document>("documents");
            _col.EnsureIndex(d => d.Title);
            _col.EnsureIndex(d => d.LastModified);
        }

        public IEnumerable<Document> GetAll(string search = null)
        {
            if (string.IsNullOrWhiteSpace(search))
                return _col.Query().OrderByDescending(d => d.LastModified).ToList();

            return _col.Query()
                .Where(d => d.Title.ToLower().Contains(search.ToLower()))
                .OrderByDescending(d => d.LastModified)
                .ToList();
        }

        public Document GetById(string id) => _col.FindById(id);

        public Document Create(string title, string mode = "Markdown")
        {
            var doc = new Document
            {
                Id = ObjectId.NewObjectId().ToString(),
                Title = string.IsNullOrWhiteSpace(title) ? "Nuovo documento" : title.Trim(),
                Content = "",
                Mode = mode,
                LastModified = DateTime.Now
            };
            _col.Insert(doc);
            return doc;
        }

        public void Update(Document doc)
        {
            doc.LastModified = DateTime.Now;
            _col.Update(doc);
        }

        public void Delete(string id) => _col.Delete(id);

        public void Dispose() => _db?.Dispose();
    }
}