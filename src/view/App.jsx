import { useState, useEffect, useCallback } from 'react'
import './App.css'
import Sidebar from './components/Sidebar'
import Editor from './components/Editor'

function App() {
  const [documents, setDocuments] = useState([]);
  const [currentDocument, setCurrentDocument] = useState(null);
  const [loading, setLoading] = useState(true);
  const [saveTimeout, setSaveTimeout] = useState(null);

  // Carica tutti i documenti all'avvio
  useEffect(() => {
    loadDocuments();
  }, []);

  const loadDocuments = async () => {
    try {
      setLoading(true);
      const docs = await window.database.getAllDocuments();
      setDocuments(docs || []);
    } catch (error) {
      console.error('Error loading documents:', error);
    } finally {
      setLoading(false);
    }
  };

  const generateId = () => {
    return Date.now().toString(36) + Math.random().toString(36).substr(2);
  };

  const handleNewDocument = async () => {
    const newDoc = {
      id: generateId(),
      title: 'Untitled Document',
      content: '',
      path: 'Untitled Document',
      parent_path: null
    };

    try {
      const result = await window.database.insertDocument(newDoc);
      if (result.success) {
        await loadDocuments();
        setCurrentDocument(newDoc);
      }
    } catch (error) {
      console.error('Error creating document:', error);
    }
  };

  const handleSelectDocument = (doc) => {
    setCurrentDocument(doc);
  };

  const handleUpdateDocument = useCallback(async (updatedDoc) => {
    // Debounce dei salvataggi
    if (saveTimeout) {
      clearTimeout(saveTimeout);
    }

    // Aggiorna lo stato locale immediatamente
    setCurrentDocument(updatedDoc);
    setDocuments(prev => 
      prev.map(doc => doc.id === updatedDoc.id ? updatedDoc : doc)
    );

    // Salva nel database dopo un breve delay
    const timeout = setTimeout(async () => {
      try {
        await window.database.updateDocument(updatedDoc.id, {
          title: updatedDoc.title,
          content: updatedDoc.content,
          path: updatedDoc.title // Aggiorna il path con il nuovo titolo
        });
      } catch (error) {
        console.error('Error saving document:', error);
      }
    }, 500);

    setSaveTimeout(timeout);
  }, [saveTimeout]);

  const handleDocumentLinkClick = async (linkText) => {
    // Cerca un documento collegato
    // Il path sarà parentPath/linkText
    let targetPath = linkText;
    
    if (currentDocument && currentDocument.path) {
      targetPath = currentDocument.path + '/' + linkText;
    }

    try {
      // Prima prova a cercare per path esatto
      let linkedDoc = await window.database.getDocumentByPath(targetPath);
      
      // Se non esiste, crea un nuovo documento
      if (!linkedDoc) {
        const newDoc = {
          id: generateId(),
          title: linkText,
          content: '',
          path: targetPath,
          parent_path: currentDocument ? currentDocument.path : null
        };
        
        const result = await window.database.insertDocument(newDoc);
        if (result.success) {
          await loadDocuments();
          setCurrentDocument(newDoc);
        }
      } else {
        setCurrentDocument(linkedDoc);
      }
    } catch (error) {
      console.error('Error handling document link:', error);
    }
  };

  
  if (loading) {
    return (
      <div className="app-container">
        <div className="loading">Loading...</div>
      </div>
    );
  }

  return (
    <div className="app-container">
      <Sidebar
        documents={documents}
        currentDocId={currentDocument?.id}
        onSelectDocument={handleSelectDocument}
        onNewDocument={handleNewDocument}
      />
      <Editor
        document={currentDocument}
        onUpdate={handleUpdateDocument}
        onDocumentLinkClick={handleDocumentLinkClick}
      />
    </div>
  );
}

export default App
