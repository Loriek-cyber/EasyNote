// main.cjs
const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const { 
  initDatabase, 
  insertDocument, 
  updateDocument,
  queryDocuments, 
  getDocument,
  getDocumentByPath,
  getChildDocuments,
  deleteDocument
} = require('../db/db.cjs');

// __dirname è già disponibile in CommonJS, quindi non serve usare import.meta.url
const dirname = __dirname;

function createWindow() {
  const win = new BrowserWindow({
    width: 1400,
    height: 900,
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

  // IPC Handlers per database
  ipcMain.handle('db:getAllDocuments', async () => {
    return queryDocuments();
  });

  ipcMain.handle('db:getDocument', async (event, id) => {
    return getDocument(id);
  });

  ipcMain.handle('db:getDocumentByPath', async (event, path) => {
    return getDocumentByPath(path);
  });

  ipcMain.handle('db:getChildDocuments', async (event, parentPath) => {
    return getChildDocuments(parentPath);
  });

  ipcMain.handle('db:insertDocument', async (event, document) => {
    return insertDocument(document);
  });

  ipcMain.handle('db:updateDocument', async (event, id, updates) => {
    return updateDocument(id, updates);
  });

  ipcMain.handle('db:deleteDocument', async (event, id) => {
    return deleteDocument(id);
  });

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
