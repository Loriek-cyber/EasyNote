const { contextBridge, ipcRenderer } = require('electron');

// Simple log to help verify preload runs (appears in renderer DevTools console)
try {
  // eslint-disable-next-line no-console
  console.log('[preload] preload.cjs loaded, exposing api');
} catch (e) {}

contextBridge.exposeInMainWorld('api', {
  // notes
  getNotes: () => ipcRenderer.invoke('get-notes'),
  addNote: (note) => ipcRenderer.invoke('add-note', note),
  updateNote: (note) => ipcRenderer.invoke('update-note', note),
  deleteNote: (id) => ipcRenderer.invoke('delete-note', id),
  // settings
  getSetting: (key) => ipcRenderer.invoke('get-setting', key),
  setSetting: (key, value) => ipcRenderer.invoke('set-setting', key, value)
});
