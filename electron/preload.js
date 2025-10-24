const { contextBridge } = require('electron');

// Safe bridge for communication
contextBridge.exposeInMainWorld('api', {
  ping: () => 'pong 🏓 from Electron!',
});
