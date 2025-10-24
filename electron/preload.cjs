const { contextBridge } = require('electron');

// Safe bridge for communication
contextBridge.exposeInMainWorld('api', {
  log: (message) => console.log("Il modulo preload è stato caricato")
});
