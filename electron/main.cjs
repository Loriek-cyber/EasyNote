const { app, BrowserWindow, ipcMain } = require('electron');
const path = require('path');
const DatabaseManager = require('../src/model/db.cjs');
const { DocDAO } = require('../src/model/DAO/docdao.js');

const dbManager = new DatabaseManager();
let docDAO;

async function initializeDatabase() {
    const dbPath = path.join(app.getPath('userData'), 'easymeet.sqlite');
    await dbManager.connect(dbPath);
    docDAO = new DocDAO(dbManager); // Pass the manager, not the raw db
    await docDAO.createTable();
    console.log('Database initialized and table is ready.');
}

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

function setupDatabaseIPC() {
    // Document-specific handlers
    ipcMain.handle('doc:create', (_, doc) => docDAO.create(doc));
    ipcMain.handle('doc:getAll', () => docDAO.getAll());
    ipcMain.handle('doc:getById', (_, id) => docDAO.getById(id));
    ipcMain.handle('doc:update', (_, id, doc) => docDAO.update(id, doc));
    ipcMain.handle('doc:delete', (_, id) => docDAO.delete(id));
    ipcMain.handle('doc:findByPath', (_, path) => docDAO.findByPath(path));

    // General DB connection status
    ipcMain.handle('db:isConnected', () => dbManager.isConnected());
}

app.whenReady().then(async () => {
    await initializeDatabase();
    setupDatabaseIPC();
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
