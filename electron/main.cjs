const { app, BrowserWindow } = require('electron');
const path = require('path');
import DatabaseManager from '../src/model/db';

function createWindow() {
  const dbManager = new DatabaseManager(); 
  
  //la connessione al database viene fatta qui, non in App.jsx perchè altrimenti si crea una nuova connessione ad 
  // ogni render è io mi sparerei
  dbManager.connect('app_database.db');


   const win = new BrowserWindow({
    width: 900,
    height: 700,
    webPreferences: {
        // preload script in this project is named preload.cjs
        preload: path.join(__dirname, 'preload.cjs'),
        // security: isolate renderer and disable node integration
        contextIsolation: true,
        nodeIntegration: false,
    }
  });
  // this is only production becouse i am stupid and i cant run this, for all purposes its work so fuck you
  win.removeMenu();
  win.loadURL('http://localhost:5173'); // React dev server
}

app.whenReady().then(() => {
  createWindow();

  app.on('activate', () => {
    if (BrowserWindow.getAllWindows().length === 0) createWindow();
  });
});

app.on('window-all-closed', () => {
  if (process.platform !== 'darwin') app.quit();
});
