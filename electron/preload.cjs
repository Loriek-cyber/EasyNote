const { contextBridge, ipcRenderer } = require('electron');

// Create safe database API for renderer
contextBridge.exposeInMainWorld('database', {
    run: async (sql, params) => {
        try {
            return await ipcRenderer.invoke('db:run', sql, params);
        } catch (err) {
            console.error('Database run error:', err);
            throw err;
        }
    },
    get: async (sql, params) => {
        try {
            return await ipcRenderer.invoke('db:get', sql, params);
        } catch (err) {
            console.error('Database get error:', err);
            throw err;
        }
    },
    all: async (sql, params) => {
        try {
            return await ipcRenderer.invoke('db:all', sql, params);
        } catch (err) {
            console.error('Database all error:', err);
            throw err;
        }
    },
    switchDB: async (dbPath) => {
        try {
            return await ipcRenderer.invoke('db:switch', dbPath);
        } catch (err) {
            console.error('Database switch error:', err);
            throw err;
        }
    },
    isok: async () => {
        try {
            return await ipcRenderer.invoke('db:isConnected');
        } catch (err) {
            console.error('Database connection check error:', err);
            return false;
        }
    }
});


