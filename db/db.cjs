const sqlite3 = require('sqlite3').verbose();


// Gestore della connessione al database SQLite
class DatabaseManager {
  constructor() {
    this.db = null; // inizialmente nessuna connessione
  }

  // Connetti al database
  connect(dbPath = ':memory:') {
    return new Promise((resolve, reject) => {
      this.db = new sqlite3.Database(dbPath, (err) => {
        if (err) {
          console.error('Errore di connessione al database SQLite:', err.message);
          reject(err);
        } else {
          console.log('Connesso al database SQLite:', dbPath);
          resolve();
        }
      });
    });
  }

  // Controlla se c'è una connessione attiva
  isConnected() {
    return this.db !== null;
  }

  // Esegui una query (INSERT, UPDATE, DELETE), ma che cazzo sto scrivendo?
  run(sql, params = []) {
    return new Promise((resolve, reject) => {
      this.db.run(sql, params, function(err) {
        if (err) {
          reject(err);
        } else {
          resolve({ id: this.lastID, changes: this.changes });
        }
      });
    });
  }

  // Ottieni una singola riga
  get(sql, params = []) {
    return new Promise((resolve, reject) => {
      this.db.get(sql, params, (err, row) => {
        if (err) {
          reject(err);
        } else {
          resolve(row);
        }
      });
    });
  }

  // Ottieni tutte le righe
  all(sql, params = []) {
    return new Promise((resolve, reject) => {
      this.db.all(sql, params, (err, rows) => {
        if (err) {
          reject(err);
        } else {
          resolve(rows);
        }
      });
    });
  }

  // Esegui una transazione
  async transaction(queries = []) {
    if (!this.isConnected()) throw new Error('Database non connesso');

    try {
      await this.run('BEGIN TRANSACTION;');
      for (const { sql, params } of queries) {
        await this.run(sql, params);
      }
      await this.run('COMMIT;');
    } catch (err) {
      await this.run('ROLLBACK;');
      throw err;
    }
  }

  // Chiudi la connessione
  close() {
    return new Promise((resolve, reject) => {
      if (!this.db) return resolve();
      this.db.close((err) => {
        if (err) {
          reject(err);
        } else {
          console.log('Database chiuso');
          this.db = null;
          resolve();
        }
      });
    });
  }


  // Cambia database questo è per cambiare blocco di appunti o roba del genere
  async switchDatabase(dbPath) {
    if (this.db) await this.close();
    await this.connect(dbPath);
  }
}

module.exports = DatabaseManager;
