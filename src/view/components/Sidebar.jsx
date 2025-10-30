import React, { useEffect, useState } from 'react';
import useStore from '../store/useStore';

export default function Sidebar() {
  const notes = useStore((state) => state.notes);
  const loadNotes = useStore((state) => state.loadNotes);
  const createNote = useStore((state) => state.createNote);
  const selectedId = useStore((s) => s.selectedId);

  const [q, setQ] = useState('');
  const [selectedTag, setSelectedTag] = useState(null);

  useEffect(() => {
    loadNotes();
    // intentionally run once on mount
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // extract tags from notes content (#tag syntax)
  const allTags = Array.from(new Set(notes.flatMap(n => {
    const s = `${n.title || ''} ${n.content || ''}`;
    const m = Array.from(s.matchAll(/#([a-zA-Z0-9_-]+)/g)).map(r => r[1]);
    return m;
  })));

  const filtered = notes.filter(n => {
    if (selectedTag) {
      const s = `${n.title || ''} ${n.content || ''}`;
      if (!s.includes(`#${selectedTag}`)) return false;
    }
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

      <div className="mb-2">
        {allTags.slice(0, 12).map(tag => (
          <button key={tag} onClick={() => setSelectedTag(selectedTag === tag ? null : tag)} className={`mr-2 mb-2 px-2 py-1 rounded text-sm ${selectedTag === tag ? 'bg-blue-600 text-white' : 'bg-gray-100 dark:bg-gray-800 text-gray-700 dark:text-gray-200'}`}>
            #{tag}
          </button>
        ))}
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
            onClick={() => useStore.getState().setSelectedId(note.id)}
            className={`p-2 rounded cursor-pointer ${note.id === selectedId ? 'bg-blue-100 dark:bg-blue-900' : 'hover:bg-gray-100 dark:hover:bg-gray-800'}`}
          >
            <div className="flex items-center justify-between">
              <div className="font-medium">{note.title || 'Untitled'}</div>
              <div className="text-xs text-gray-500">{new Date(note.updated_at || 0).toLocaleDateString()}</div>
            </div>
            <div className="text-sm text-gray-600 dark:text-gray-400 overflow-hidden truncate">
              {(note.content || '')
                .replace(/[`*_>#\-\[\]\(\)!]/g, '')
                .replace(/\s+/g, ' ')
                .trim()
                .slice(0, 80)}
            </div>
            <div className="mt-2 flex gap-2">
              {Array.from(new Set(Array.from((`${note.title || ''} ${note.content || ''}`).matchAll(/#([a-zA-Z0-9_-]+)/g)).map(r=>r[1]))).slice(0,3).map(t => (
                <span key={t} className="text-xs px-2 py-1 bg-gray-100 dark:bg-gray-800 rounded">#{t}</span>
              ))}
            </div>
          </li>
        ))}
      </ul>
    </aside>
  );
}
