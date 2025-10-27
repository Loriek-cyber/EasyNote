import React, { useEffect, useState } from 'react';
import { useEditor, EditorContent } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import useStore from '../store/useStore';

export default function EditorPane() {
  const notes = useStore((s) => s.notes);
  const selectedId = useStore((s) => s.selectedId);
  const updateNote = useStore((s) => s.updateNote);

  const [current, setCurrent] = useState(null);

  useEffect(() => {
    const n = notes.find(x => x.id === selectedId) || null;
    setCurrent(n);
  }, [notes, selectedId]);

  const editor = useEditor({
    extensions: [StarterKit],
    content: current ? current.content : ''
  });

  // when selected note changes, set editor content
  useEffect(() => {
    if (editor && current) {
      editor.commands.setContent(current.content || '');
    }
  }, [editor, current]);

  // autosave (debounced)
  useEffect(() => {
    if (!editor) return;
    let t;
    const handler = () => {
      if (!current) return;
      clearTimeout(t);
      t = setTimeout(() => {
        const content = editor.getHTML();
        updateNote({ id: current.id, title: (current.title || '').slice(0, 64), content }).catch(console.error);
      }, 800);
    };
    editor.on('update', handler);
    return () => editor.off('update', handler);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [editor, current]);

  if (!current) {
    return <div className="p-8 text-gray-500">Select or create a note</div>;
  }

  return (
    <div className="flex-1 p-6">
      <div className="mb-3">
        <input
          className="w-full text-2xl font-bold bg-transparent outline-none"
          value={current.title || ''}
          onChange={(e) => useStore.setState((state) => {
            const note = state.notes.find(n => n.id === current.id);
            if (note) note.title = e.target.value;
            return { notes: state.notes };
          })}
        />
      </div>
      <div className="prose max-w-none">
        <EditorContent editor={editor} />
      </div>
    </div>
  );
}
