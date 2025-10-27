import React, { useEffect, useState } from 'react';
import useStore from '../store/useStore';

export default function Sidebar() {
  const { notes, loadNotes, createNote } = useStore((state) => ({
    notes: state.notes,
    loadNotes: state.loadNotes,
    createNote: state.createNote
  }));

  const selectedId = useStore((s) => s.selectedId);

  const [q, setQ] = useState('');

  useEffect(() => {
    loadNotes();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const filtered = notes.filter(n => {
    if (!q) return true;
    const t = q.toLowerCase();
    return ((n.title || '').toLowerCase().includes(t) || (n.content || '').toLowerCase().includes(t));
  });

  return (
    <aside className="w-72 bg-white dark:bg-gray-900 p-4 border-r border-gray-200 dark:border-gray-700">
      <div className="flex items-center justify-between mb-4">
        <h2 className="text-lg font-bold">Notes</h2>
        <button
          className="text-sm px-2 py-1 bg-blue-500 text-white rounded"
          onClick={() => createNote().catch(console.error)}
        >
          New
        </button>
      </div>

      <input
        placeholder="Search..."
        value={q}
        onChange={(e) => setQ(e.target.value)}
        className="w-full p-2 rounded mb-3 bg-gray-50 dark:bg-gray-800 border"
      />

      <ul className="space-y-2 overflow-auto" style={{ maxHeight: '60vh' }}>
        {filtered.map(note => (
          <li
            key={note.id}
            onClick={() => useStore.setState({ selectedId: note.id })}
            className={`p-2 rounded cursor-pointer ${note.id === selectedId ? 'bg-blue-100 dark:bg-blue-900' : 'hover:bg-gray-100 dark:hover:bg-gray-800'}`}
          >
            <div className="font-medium">{note.title || 'Untitled'}</div>
            <div className="text-sm text-gray-600 dark:text-gray-400 overflow-hidden truncate">{(note.content || '').replace(/<[^>]+>/g, '').slice(0, 80)}</div>
          </li>
        ))}
      </ul>
    </aside>
  );
}
