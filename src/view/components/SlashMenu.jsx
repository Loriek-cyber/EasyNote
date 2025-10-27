import React from 'react';
import { motion } from 'framer-motion';

export default function SlashMenu({ x = 0, y = 0, onSelect }) {
  const items = [
    { id: 'paragraph', label: 'Text' },
    { id: 'heading', label: 'Heading' },
    { id: 'bullet', label: 'Bulleted list' },
    { id: 'checkbox', label: 'Checkbox' },
    { id: 'code', label: 'Code block' },
    { id: 'quote', label: 'Quote' }
  ];

  return (
    <motion.ul
      initial={{ opacity: 0, scale: 0.95 }}
      animate={{ opacity: 1, scale: 1 }}
      exit={{ opacity: 0, scale: 0.95 }}
      className="absolute z-50 w-56 bg-white dark:bg-gray-800 rounded shadow-lg p-2 border border-gray-200 dark:border-gray-700"
      style={{ left: x, top: y }}
    >
      {items.map(i => (
        <li key={i.id} className="p-2 rounded hover:bg-gray-100 dark:hover:bg-gray-700 cursor-pointer" onMouseDown={(e) => { e.preventDefault(); onSelect(i.id); }}>
          <div className="font-medium">{i.label}</div>
        </li>
      ))}
    </motion.ul>
  );
}
