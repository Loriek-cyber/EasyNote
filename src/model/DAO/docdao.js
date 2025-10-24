export class DocDAO {
    constructor(db) {
        this.db = db;
    }

    async create(doc) {
        const sql = "INSERT INTO documents (title, content, path) VALUES (?, ?, ?)";
        const params = [doc.title, doc.content, doc];
        return this.db.run(sql, params);
    }

    async getById(id) {
        const sql = "SELECT * FROM documents WHERE id = ?";
        const params = [id];
        return this.db.get(sql, params);
    }

    async getAll() {
        const sql = "SELECT * FROM documents";
        return this.db.all(sql);
    }

    async update(id, doc) {
        const sql = "UPDATE documents SET title = ?, content = ? WHERE id = ?";
        const params = [doc.title, doc.content, id];
        return this.db.run(sql, params);
    }

    async delete(id) {
        const sql = "DELETE FROM documents WHERE id = ?";
        const params = [id];
        return this.db.run(sql, params);
    }

    async findByPath(path) {
        const sql = "SELECT * FROM documents WHERE path = ?";
        const params = [path];
        return this.db.get(sql, params);
    }
}