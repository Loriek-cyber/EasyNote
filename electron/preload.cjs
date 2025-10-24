const { contextBridge, ipcRenderer } = require('electron');

// Create safe database API for renderer
contextBridge.exposeInMainWorld('database', {
    
});


