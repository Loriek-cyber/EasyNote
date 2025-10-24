import { useState, useRef, useEffect } from 'react';
import SlashMenu from './SlashMenu';

function ContentBlock({ value, onChange, onNewBlock, onDelete, isLast }) {
  const [showSlashMenu, setShowSlashMenu] = useState(false);
  const [slashPosition, setSlashPosition] = useState(0);
  const [searchQuery, setSearchQuery] = useState('');
  const textareaRef = useRef(null);

  const handleChange = (e) => {
    const newValue = e.target.value;
    const cursorPos = e.target.selectionStart;
    
    // Verifica se l'utente ha digitato '/'
    if (newValue[cursorPos - 1] === '/' && (cursorPos === 1 || newValue[cursorPos - 2] === '\n' || newValue[cursorPos - 2] === ' ')) {
      setShowSlashMenu(true);
      setSlashPosition(cursorPos);
      setSearchQuery('');
    } else if (showSlashMenu) {
      // Aggiorna la ricerca se il menu è aperto
      const textAfterSlash = newValue.slice(slashPosition, cursorPos);
      if (textAfterSlash.includes(' ') || textAfterSlash.includes('\n')) {
        setShowSlashMenu(false);
      } else {
        setSearchQuery(textAfterSlash);
      }
    }

    onChange(newValue);
  };

  const handleKeyDown = (e) => {
    // Non gestire tasti se il menu slash è aperto (gestito da SlashMenu)
    if (showSlashMenu && ['ArrowDown', 'ArrowUp', 'Enter', 'Escape'].includes(e.key)) {
      return;
    }

    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      onNewBlock();
    } else if (e.key === 'Backspace' && value === '' && !isLast) {
      e.preventDefault();
      onDelete();
    }
  };

  const handleSlashSelect = (blockType) => {
    const beforeSlash = value.slice(0, slashPosition - 1);
    const afterSlash = value.slice(textareaRef.current.selectionStart);
    
    let newValue = beforeSlash;
    if (blockType.prefix) {
      newValue += blockType.prefix;
    }
    if (blockType.suffix) {
      newValue += blockType.suffix;
      // Posiziona il cursore tra prefix e suffix
      const cursorPosition = newValue.length - blockType.suffix.length;
      setTimeout(() => {
        textareaRef.current.focus();
        textareaRef.current.setSelectionRange(cursorPosition, cursorPosition);
      }, 0);
    } else {
      newValue += afterSlash;
      setTimeout(() => {
        textareaRef.current.focus();
        textareaRef.current.setSelectionRange(newValue.length - afterSlash.length, newValue.length - afterSlash.length);
      }, 0);
    }
    
    if (!blockType.suffix) {
      newValue += afterSlash;
    }
    
    onChange(newValue);
    setShowSlashMenu(false);
  };

  const adjustHeight = () => {
    if (textareaRef.current) {
      textareaRef.current.style.height = 'auto';
      textareaRef.current.style.height = textareaRef.current.scrollHeight + 'px';
    }
  };

  useEffect(() => {
    adjustHeight();
  }, [value]);

  return (
    <div className="content-block">
      <textarea
        ref={textareaRef}
        className="block-input"
        value={value}
        onChange={handleChange}
        onKeyDown={handleKeyDown}
        placeholder="Type '/' for commands..."
        rows={1}
      />
      {showSlashMenu && (
        <SlashMenu
          onSelect={handleSlashSelect}
          onClose={() => setShowSlashMenu(false)}
          searchQuery={searchQuery}
        />
      )}
    </div>
  );
}

export default ContentBlock;
