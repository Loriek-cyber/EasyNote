import { useState, useEffect } from 'react';

const BLOCK_TYPES = [
  {
    id: 'markdown',
    title: '📝 Markdown',
    description: 'Aggiungi testo Markdown formattato'
  },
  {
    id: 'heading1',
    title: 'H1 Heading',
    description: 'Titolo grande',
    prefix: '# '
  },
  {
    id: 'heading2',
    title: 'H2 Heading',
    description: 'Titolo medio',
    prefix: '## '
  },
  {
    id: 'heading3',
    title: 'H3 Heading',
    description: 'Titolo piccolo',
    prefix: '### '
  },
  {
    id: 'latex',
    title: '🔢 LaTeX Inline',
    description: 'Formula matematica inline',
    prefix: '$',
    suffix: '$'
  },
  {
    id: 'latex-block',
    title: '📐 LaTeX Block',
    description: 'Formula matematica su riga separata',
    prefix: '$$\n',
    suffix: '\n$$'
  },
  {
    id: 'code',
    title: '💻 Code Block',
    description: 'Blocco di codice',
    prefix: '```\n',
    suffix: '\n```'
  },
  {
    id: 'quote',
    title: '💬 Quote',
    description: 'Citazione',
    prefix: '> '
  },
  {
    id: 'list',
    title: '• Bullet List',
    description: 'Lista puntata',
    prefix: '- '
  },
  {
    id: 'numbered',
    title: '1. Numbered List',
    description: 'Lista numerata',
    prefix: '1. '
  }
];

function SlashMenu({ onSelect, onClose, searchQuery = '' }) {
  const [selectedIndex, setSelectedIndex] = useState(0);
  const [filteredItems, setFilteredItems] = useState(BLOCK_TYPES);

  useEffect(() => {
    const query = searchQuery.toLowerCase();
    const filtered = BLOCK_TYPES.filter(
      item => 
        item.title.toLowerCase().includes(query) || 
        item.description.toLowerCase().includes(query)
    );
    setFilteredItems(filtered);
    setSelectedIndex(0);
  }, [searchQuery]);

  useEffect(() => {
    const handleKeyDown = (e) => {
      if (e.key === 'ArrowDown') {
        e.preventDefault();
        setSelectedIndex(prev => (prev + 1) % filteredItems.length);
      } else if (e.key === 'ArrowUp') {
        e.preventDefault();
        setSelectedIndex(prev => (prev - 1 + filteredItems.length) % filteredItems.length);
      } else if (e.key === 'Enter') {
        e.preventDefault();
        if (filteredItems[selectedIndex]) {
          onSelect(filteredItems[selectedIndex]);
        }
      } else if (e.key === 'Escape') {
        e.preventDefault();
        onClose();
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [selectedIndex, filteredItems, onSelect, onClose]);

  if (filteredItems.length === 0) {
    return null;
  }

  return (
    <div className="slash-menu">
      {filteredItems.map((item, index) => (
        <div
          key={item.id}
          className={`slash-menu-item ${index === selectedIndex ? 'selected' : ''}`}
          onClick={() => onSelect(item)}
          onMouseEnter={() => setSelectedIndex(index)}
        >
          <div className="menu-item-title">{item.title}</div>
          <div className="menu-item-description">{item.description}</div>
        </div>
      ))}
    </div>
  );
}

export default SlashMenu;
