import { useEffect, useState } from 'react';
import { EditorContent, useEditor } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';

function App() {
  const [notes, setNotes] = useState([]);
  const editor = useEditor({ extensions: [StarterKit], content: '' });

  useEffect(() => {
    window.api.getNotes().then(setNotes);
  }, []);

  const saveNote = () => {
    const content = editor.getHTML();
    window.api.addNote({ title: 'Nuova nota', content }).then(() => window.api.getNotes().then(setNotes));
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
