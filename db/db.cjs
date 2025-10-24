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
        path TEXT NOT NULL UNIQUE,
        parent_path TEXT,
        created_at INTEGER DEFAULT (strftime('%s', 'now')),
        updated_at INTEGER DEFAULT (strftime('%s', 'now'))
      )
    `);
    
    console.log('Database initialized successfully');
    return true;
  } catch (error) {
    console.error('Failed to initialize database:', error);
    throw error;
  }
}

// Inserisce un documento unico
function insertDocument(document) {
  try {
    const stmt = db.prepare(`
      INSERT INTO documents (id, title, content, path, parent_path)
      VALUES (?, ?, ?, ?, ?)
    `);
    const result = stmt.run(
      document.id, 
      document.title, 
      document.content, 
      document.path,
      document.parent_path || null
    );
    return { success: true, changes: result.changes };
  } catch (error) {
    if (error.code === 'SQLITE_CONSTRAINT') {
      return { success: false, error: 'Document already exists' };
    }
    throw error;
  }
}

// Aggiorna un documento esistente
function updateDocument(id, updates) {
  try {
    const fields = [];
    const params = [];
    
    for (const [key, value] of Object.entries(updates)) {
      if (key !== 'id') {
        fields.push(`${key} = ?`);
        params.push(value);
      }
    }
    
    fields.push('updated_at = ?');
    params.push(Math.floor(Date.now() / 1000));
    params.push(id);
    
    const stmt = db.prepare(`
      UPDATE documents 
      SET ${fields.join(', ')}
      WHERE id = ?
    `);
    const result = stmt.run(...params);
    return { success: true, changes: result.changes };
  } catch (error) {
    console.error('Update error:', error);
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
    
    query += ' ORDER BY created_at DESC';
    
    const stmt = db.prepare(query);
    return stmt.all(...params);
  } catch (error) {
    console.error('Query error:', error);
    throw error;
  }
}

// Ottiene un singolo documento per ID
function getDocument(id) {
  try {
    const stmt = db.prepare('SELECT * FROM documents WHERE id = ?');
    return stmt.get(id);
  } catch (error) {
    console.error('Get document error:', error);
    throw error;
  }
}

// Ottiene documento per path
function getDocumentByPath(path) {
  try {
    const stmt = db.prepare('SELECT * FROM documents WHERE path = ?');
    return stmt.get(path);
  } catch (error) {
    console.error('Get document by path error:', error);
    throw error;
  }
}

// Ottiene documenti figli
function getChildDocuments(parentPath) {
  try {
    const stmt = db.prepare('SELECT * FROM documents WHERE parent_path = ? ORDER BY created_at DESC');
    return stmt.all(parentPath);
  } catch (error) {
    console.error('Get child documents error:', error);
    throw error;
  }
}

// Elimina un documento
function deleteDocument(id) {
  try {
    const stmt = db.prepare('DELETE FROM documents WHERE id = ?');
    const result = stmt.run(id);
    return { success: true, changes: result.changes };
  } catch (error) {
    console.error('Delete error:', error);
    throw error;
  }
}

module.exports = {
  initDatabase,
  insertDocument,
  updateDocument,
  queryDocuments,
  getDocument,
  getDocumentByPath,
  getChildDocuments,
  deleteDocument
};

