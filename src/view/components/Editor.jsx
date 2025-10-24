import { useState, useEffect } from 'react';
import ContentBlock from './ContentBlock';
import MarkdownRenderer from './MarkdownRenderer';

function Editor({ document, onUpdate, onDocumentLinkClick }) {
  const [title, setTitle] = useState('');
  const [blocks, setBlocks] = useState(['']);
  const [isPreview, setIsPreview] = useState(false);

  useEffect(() => {
    if (document) {
      setTitle(document.title || '');
      // Dividi il contenuto in blocchi (per ora usiamo un singolo blocco)
      setBlocks([document.content || '']);
    }
  }, [document]);

  const handleTitleChange = (e) => {
    const newTitle = e.target.value;
    setTitle(newTitle);
    saveDocument(newTitle, blocks);
  };

  const handleBlockChange = (index, value) => {
    const newBlocks = [...blocks];
    newBlocks[index] = value;
    setBlocks(newBlocks);
    saveDocument(title, newBlocks);
  };

  const handleNewBlock = (index) => {
    const newBlocks = [...blocks];
    newBlocks.splice(index + 1, 0, '');
    setBlocks(newBlocks);
  };

  const handleDeleteBlock = (index) => {
    if (blocks.length > 1) {
      const newBlocks = blocks.filter((_, i) => i !== index);
      setBlocks(newBlocks);
      saveDocument(title, newBlocks);
    }
  };

  const saveDocument = (newTitle, newBlocks) => {
    if (document) {
      const content = newBlocks.join('\n\n');
      onUpdate({
        ...document,
        title: newTitle,
        content: content
      });
    }
  };

  if (!document) {
    return (
      <div className="main-content">
        <div className="empty-state">
          <div className="empty-state-icon">📝</div>
          <div className="empty-state-title">No document selected</div>
          <div className="empty-state-description">
            Select a document from the sidebar or create a new one to get started
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="main-content">
      <div className="editor-header">
        <input
          type="text"
          className="document-title-input"
          value={title}
          onChange={handleTitleChange}
          placeholder="Untitled Document"
        />
        <div className="document-path">{document.path}</div>
        <div style={{ marginTop: '12px' }}>
          <button
            onClick={() => setIsPreview(!isPreview)}
            style={{ marginRight: '8px' }}
          >
            {isPreview ? '✏️ Edit' : '👁️ Preview'}
          </button>
        </div>
      </div>
      <div className="editor-container">
        <div className="editor-content">
          {isPreview ? (
            <MarkdownRenderer
              content={blocks.join('\n\n')}
              onDocumentLinkClick={onDocumentLinkClick}
            />
          ) : (
            blocks.map((block, index) => (
              <ContentBlock
                key={index}
                value={block}
                onChange={(value) => handleBlockChange(index, value)}
                onNewBlock={() => handleNewBlock(index)}
                onDelete={() => handleDeleteBlock(index)}
                isLast={blocks.length === 1}
              />
            ))
          )}
        </div>
      </div>
    </div>
  );
}

export default Editor;
