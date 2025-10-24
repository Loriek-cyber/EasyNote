const Database = require('better-sqlite3');
const path = require('path');
const { app } = require('electron');

let db = null;

// Inizializza il database
async function initDatabase() {
  try {
    const dbPath = path.join(app.getPath('userData'), 'easymeet.db');
    console.log('Database path:', dbPath);
    
    db = new Database(dbPath);
    
    // Crea la tabella documents se non esiste
    db.exec(`
      CREATE TABLE IF NOT EXISTS documents (
        id TEXT PRIMARY KEY,
        title TEXT NOT NULL,
        content TEXT NOT NULL,
        path TEXT NOT NULL,
        UNIQUE(id, path)
      )
    `);
    
    console.log('Database initialized successfully');
    return true;
  } catch (error) {
    console.error('Failed to initialize database:', error);
    throw error;
  }
}

// Inserisce un documento unico (controlla duplicati)
function insertUnique(document) {
  try {
    const stmt = db.prepare(`
      INSERT INTO documents (id, title, content, path)
      VALUES (?, ?, ?, ?)
    `);
    const result = stmt.run(document.id, document.title, document.content, document.path);
    return { success: true, changes: result.changes };
  } catch (error) {
    if (error.code === 'SQLITE_CONSTRAINT') {
      return { success: false, error: 'Document with same id and path already exists' };
    }
    throw error;
  }
}

// Query per ottenere documenti
function queryDocuments(filters = {}) {
  try {
    let query = 'SELECT * FROM documents';
    const params = [];
    
    if (Object.keys(filters).length > 0) {
      const conditions = [];
      for (const [key, value] of Object.entries(filters)) {
        conditions.push(`${key} = ?`);
        params.push(value);
      }
      query += ' WHERE ' + conditions.join(' AND ');
    }
    
    const stmt = db.prepare(query);
    return stmt.all(...params);
  } catch (error) {
    console.error('Query error:', error);
    throw error;
  }
}

module.exports = {
  initDatabase,
  insertUnique,
  queryDocuments
};

