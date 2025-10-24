import { useState, useEffect } from 'react';

function Sidebar({ documents, currentDocId, onSelectDocument, onNewDocument }) {
  const [rootDocs, setRootDocs] = useState([]);
  const [childDocsMap, setChildDocsMap] = useState({});

  useEffect(() => {
    // Organizza documenti in gerarchia
    const roots = documents.filter(doc => !doc.parent_path);
    const childMap = {};
    
    documents.forEach(doc => {
      if (doc.parent_path) {
        if (!childMap[doc.parent_path]) {
          childMap[doc.parent_path] = [];
        }
        childMap[doc.parent_path].push(doc);
      }
    });

    setRootDocs(roots);
    setChildDocsMap(childMap);
  }, [documents]);

  const renderDocument = (doc, isChild = false) => {
    const children = childDocsMap[doc.path] || [];
    
    return (
      <div key={doc.id}>
        <div
          className={`document-item ${isChild ? 'child' : ''} ${doc.id === currentDocId ? 'active' : ''}`}
          onClick={() => onSelectDocument(doc)}
        >
          <span className="document-icon">📄</span>
          <span>{doc.title || 'Untitled'}</span>
        </div>
        {children.map(child => renderDocument(child, true))}
      </div>
    );
  };

  return (
    <div className="sidebar">
      <div className="sidebar-header">
        <div className="sidebar-title">📝 Documents</div>
        <button className="new-doc-btn" onClick={onNewDocument}>
          + New
        </button>
      </div>
      <div className="documents-list">
        {rootDocs.length === 0 ? (
          <div style={{ padding: '16px', color: 'var(--color-text-tertiary)', textAlign: 'center' }}>
            No documents yet
          </div>
        ) : (
          rootDocs.map(doc => renderDocument(doc))
        )}
      </div>
    </div>
  );
}

export default Sidebar;
