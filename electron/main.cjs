const { app, BrowserWindow, ipcMain } = require('electron');
const Database = require('better-sqlite3');
const path = require('path');

let db;
app.on('ready', () => {
  const win = new BrowserWindow({
    webPreferences: {
      preload: path.join(__dirname, 'preload.js')
    }
  });
  win.loadURL('http://localhost:5173');
  win.removeMenu();
  
  db = new Database('notes.db');
  db.prepare(`CREATE TABLE IF NOT EXISTS notes (id INTEGER PRIMARY KEY, title TEXT, content TEXT)`).run();

  ipcMain.handle('get-notes', () => db.prepare('SELECT * FROM notes').all());
  ipcMain.handle('add-note', (_, note) => {
    db.prepare('INSERT INTO notes (title, content) VALUES (?, ?)').run(note.title, note.content);
  });
});
