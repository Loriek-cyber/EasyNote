const { app, BrowserWindow, ipcMain } = require('electron');
const Database = require('better-sqlite3');
const path = require('path');
const fs = require('fs');

let db;
app.on('ready', () => {
  
  // Sezione del Menu
  
  const win = new BrowserWindow({
    webPreferences: {
      // Ensure the preload script runs in an isolated context so contextBridge works
      preload: path.join(__dirname, 'preload.cjs'),
      contextIsolation: true,
      nodeIntegration: false
    }
  });
  win.loadURL('http://localhost:5173');
  win.removeMenu();


  // initialize DB using helper (store DB in app userData)
  const dbPath = path.join(app.getPath('userData'), 'notes.db');
  const initDb = require(path.join(__dirname, 'db.js'));
  db = initDb(dbPath);

  // IPC channels for renderer
  ipcMain.handle('get-notes', () => db.getNotes());
  ipcMain.handle('add-note', (_, note) => db.addNote(note));
  ipcMain.handle('update-note', (_, note) => db.updateNote(note));
  ipcMain.handle('delete-note', (_, id) => db.deleteNote(id));
  ipcMain.handle('get-setting', (_, key) => db.getSetting(key));
  ipcMain.handle('set-setting', (_, key, value) => db.setSetting(key, value));


});
