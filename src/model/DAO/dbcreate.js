export class DBCreate {
    constructor(db) {
        this.db = db;
    }
    async createTables() {
        const createDocumentsTable = `
            CREATE TABLE IF NOT EXISTS documents (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                title TEXT NOT NULL,
                content TEXT NOT NULL,
                path TEXT UNIQUE NOT NULL
            );
        `;
        await this.db.run(createDocumentsTable);
    }
}