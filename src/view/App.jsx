import { useEffect, useState } from 'react';
import { EditorContent, useEditor } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';

function App() {
  const [notes, setNotes] = useState([]);

  const editor = useEditor({ extensions: [StarterKit], content: '' });

  useEffect(() => {
    if (window?.api && typeof window.api.getNotes === 'function') {
      window.api.getNotes().then(setNotes).catch((error) => {
        console.error('Errore nel recupero delle note:', error);
      });
    } else {
      console.warn('window.api.getNotes is not available');
    }
  }, []);

  const saveNote = () => {
    const content = editor?.getHTML ? editor.getHTML() : '';

    if (window?.api && typeof window.api.addNote === 'function') {
      window.api
        .addNote({ title: 'Nuova nota', content })
        .then(() => {
          if (typeof window.api.getNotes === 'function') {
            window.api.getNotes().then(setNotes).catch((error) => console.error('Errore nel recupero delle note:', error));
          }
        })
        .catch((error) => console.error('Errore nel salvataggio della nota:', error));
    } else {
      console.warn('window.api.addNote is not available');
    }
  };

  return (
    <div className="p-4 flex gap-4">
      <div className="w-1/3 border-r">
        {notes.map(n => <div key={n.id}>{n.title}</div>)}
      </div>
      <div className="flex-1">
        <EditorContent editor={editor} />
        <button onClick={saveNote} className="bg-blue-500 text-white px-4 py-2 rounded mt-4">Salva</button>
      </div>
    </div>
  );
}

export default App;
