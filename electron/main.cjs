const { app, BrowserWindow } = require('electron');
const path = require('path');

function createWindow() {
  const win = new BrowserWindow({
    width: 900,
    height: 700,
    webPreferences: {
        // preload script in this project is named preload.cjs
        preload: path.join(__dirname, 'preload.cjs'),
        // security: isolate renderer and disable node integration
        contextIsolation: true,
        nodeIntegration: false,
    },
  });
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
