const { app, BrowserWindow, ipcMain } = require('electron');


function createWindow() {
   const win = new BrowserWindow({
    width: 900,
    height: 700,
    webPreferences: {
        preload: path.join(__dirname, 'preload.cjs'),
        contextIsolation: true,
        nodeIntegration: false,
    }
  });
  win.removeMenu();
  win.loadURL('http://localhost:5173');
}

app.whenReady().then(async () => {
    createWindow();
    app.on('activate', () => {
        if (BrowserWindow.getAllWindows().length === 0) createWindow();
    });
});

app.on('window-all-closed', () => {
    if (process.platform !== 'darwin') {
        // Close database connection before quitting
        dbManager.close().finally(() => {
            app.quit();
        });
    }
});
