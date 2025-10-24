const { contextBridge, ipcRenderer } = require('electron');

// Create safe database API for renderer
contextBridge.exposeInMainWorld('database', {
  getAllDocuments: () => ipcRenderer.invoke('db:getAllDocuments'),
  getDocument: (id) => ipcRenderer.invoke('db:getDocument', id),
  getDocumentByPath: (path) => ipcRenderer.invoke('db:getDocumentByPath', path),
  getChildDocuments: (parentPath) => ipcRenderer.invoke('db:getChildDocuments', parentPath),
  insertDocument: (document) => ipcRenderer.invoke('db:insertDocument', document),
  updateDocument: (id, updates) => ipcRenderer.invoke('db:updateDocument', id, updates),
  deleteDocument: (id) => ipcRenderer.invoke('db:deleteDocument', id)
});


