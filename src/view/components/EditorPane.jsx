import React, { useEffect, useState, useRef } from 'react';
import { useEditor, EditorContent } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import useStore from '../store/useStore';
import SlashMenu from './SlashMenu';

export default function EditorPane() {
  const notes = useStore((s) => s.notes);
  const selectedId = useStore((s) => s.selectedId);
  const updateNote = useStore((s) => s.updateNote);

  const [current, setCurrent] = useState(null);
  const [showSlash, setShowSlash] = useState(false);
  const [slashPos, setSlashPos] = useState({ x: 0, y: 0 });
  const slashRequestRef = useRef(null);

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

  // Slash command detection
  useEffect(() => {
    if (!editor) return;

    const checkSlash = () => {
      try {
        const pos = editor.state.selection.from;
        const start = Math.max(0, pos - 3);
        const before = editor.state.doc.textBetween(start, pos, '\n', '\n');
        if (before.endsWith('/**') || before.endsWith('/')) {
          // compute coordinates
          const coords = editor.view.coordsAtPos(pos);
          const rect = editor.view.dom.getBoundingClientRect();
          setSlashPos({ x: coords.left - rect.left, y: coords.bottom - rect.top });
          setShowSlash(true);
        } else {
          setShowSlash(false);
        }
      } catch (e) {
        // ignore
      }
    };

    const handler = () => {
      if (slashRequestRef.current) clearTimeout(slashRequestRef.current);
      slashRequestRef.current = setTimeout(checkSlash, 50);
    };

    editor.on('update', handler);
    editor.on('selectionUpdate', handler);
    return () => {
      editor.off('update', handler);
      editor.off('selectionUpdate', handler);
      if (slashRequestRef.current) clearTimeout(slashRequestRef.current);
    };
  }, [editor]);

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

  const applySlashCommand = async (cmd) => {
    if (!editor) return;
    const pos = editor.state.selection.from;
    const start = Math.max(0, pos - 3);
    // remove the trigger
    editor.chain().focus().command(({ tr, dispatch }) => {
      tr.delete(start, pos);
      if (dispatch) dispatch(tr);
      return true;
    }).run();

    // execute command
    switch (cmd) {
      case 'paragraph':
        editor.chain().focus().setParagraph().run();
        break;
      case 'heading':
        editor.chain().focus().toggleHeading({ level: 2 }).run();
        break;
      case 'bullet':
        editor.chain().focus().toggleBulletList().run();
        break;
      case 'checkbox':
        editor.chain().focus().insertContentAt(editor.state.selection.from, '<p>☐ </p>').run();
        break;
      case 'code':
        editor.chain().focus().setCodeBlock().run();
        break;
      case 'quote':
        editor.chain().focus().toggleBlockquote().run();
        break;
      default:
        break;
    }

    // close menu
    setShowSlash(false);
  };

  return (
    <div className="flex-1 p-6 relative">
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

      {showSlash && (
        <SlashMenu x={slashPos.x} y={slashPos.y} onSelect={applySlashCommand} />
      )}
    </div>
  );
}
