const sqlite3 = require('sqlite3').verbose();


// Gestore della connessione al database SQLite
class DatabaseManager {
    constructor() {
        this.db = null;
    }
}

module.exports = DatabaseManager;
