/*
Allora come logica di buiness visto che non capisco come cazz implementare in locale 
faro con un sistema in cookie ecc e memory 
per ora questo codice lo leggeremo io e forse il signore
*/

//questo è un copia in colla pelese, quindi daje

const { createRxDatabase } = require('rxdb/plugins/core');
const { getRxStorageLocalstorage } = require('rxdb/plugins/storage-localstorage');
const { Document } = require('./doc.cjs');


const db = await createRxDatabase({
  name: 'mydatabase',
  storage: wrappedValidateAjvStorage({
    storage: getRxStorageLocalstorage()
  })
});


let doc = new Document(null,null,null,null); // Placeholder, will be initialized in initDatabase

// Inizializza il database e la collezione

async function initDatabase() {
  doc = await db.collection({
    name: 'documents',
    schema: doc.schema
  });
}

