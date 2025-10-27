const Database = require('better-sqlite3');
const fs = require('fs');
const path = require('path');

module.exports = function initDb(dbPath) {
  // ensure directory exists
  const dir = path.dirname(dbPath);
  if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });

  const db = new Database(dbPath);

  db.prepare(
    `CREATE TABLE IF NOT EXISTS notes (
      id INTEGER PRIMARY KEY AUTOINCREMENT,
      title TEXT,
      content TEXT,
      updated_at INTEGER
    )`
  ).run();

  db.prepare(
    `CREATE TABLE IF NOT EXISTS settings (
      key TEXT PRIMARY KEY,
      value TEXT
    )`
  ).run();

  return {
    getNotes: () => db.prepare('SELECT * FROM notes ORDER BY updated_at DESC, id DESC').all(),
    addNote: (note) => {
      const now = Date.now();
      const info = db
        .prepare('INSERT INTO notes (title, content, updated_at) VALUES (?, ?, ?)')
        .run(note.title || 'Untitled', note.content || '', now);
      return info.lastInsertRowid;
    },
    updateNote: (note) => {
      const now = Date.now();
      return db
        .prepare('UPDATE notes SET title = ?, content = ?, updated_at = ? WHERE id = ?')
        .run(note.title, note.content, now, note.id);
    },
    deleteNote: (id) => db.prepare('DELETE FROM notes WHERE id = ?').run(id),
    getSetting: (key) => {
      const row = db.prepare('SELECT value FROM settings WHERE key = ?').get(key);
      return row ? JSON.parse(row.value) : null;
    },
    setSetting: (key, value) => {
      const v = JSON.stringify(value);
      db.prepare('INSERT OR REPLACE INTO settings (key, value) VALUES (?, ?)').run(key, v);
    }
  };
};
