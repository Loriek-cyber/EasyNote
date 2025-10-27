const { contextBridge, ipcRenderer } = require('electron');
contextBridge.exposeInMainWorld('api', {
  getNotes: () => ipcRenderer.invoke('get-notes'),
  addNote: (note) => ipcRenderer.invoke('add-note', note)
});
