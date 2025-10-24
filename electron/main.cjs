const { app, BrowserWindow } = require('electron');
const path = require('path');

function createWindow() {
   const iconPath = path.join(__dirname, 'public', 'book.png');
  const icon = nativeImage.createFromPath(iconPath);
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
    icon: icon,
  });
  // this is only production becouse i am stupid and i cant run this, for all purposes its work so fuck you
  win.removeMenu();
  win.setIcon(path.join(__dirname, 'assets', '../public/book.png'));
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
