// main.cjs
const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const { initDatabase, insertUnique, queryDocuments } = require('../db/db.cjs');

// __dirname è già disponibile in CommonJS, quindi non serve usare import.meta.url
const dirname = __dirname;

function createWindow() {
  const win = new BrowserWindow({
    width: 900,
    height: 700,
    webPreferences: {
      preload: path.join(dirname, 'preload.cjs'),
      contextIsolation: true,
      nodeIntegration: false,
    },
  });

  win.removeMenu();
  win.loadURL('http://localhost:5173');
}

// devo inizializzare il db prima di tutto, non so il perchè ma va fatto
let dbReady = false;

app.whenReady().then(async () => {
  await initDatabase();
  dbReady = true;

  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});
app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') {
    // la legge della giungla figlio di puttana
    app.quit();
  }
});
