import React, { useEffect, useMemo, useRef, useState } from 'react';
import ReactMarkdown from 'react-markdown';
import remarkMath from 'remark-math';
import rehypeKatex from 'rehype-katex';
import useStore from '../store/useStore';
import 'katex/dist/katex.min.css';

export default function EditorPane() {
  const notes = useStore((s) => s.notes);
  const selectedId = useStore((s) => s.selectedId);
  const updateNote = useStore((s) => s.updateNote);

  const currentNote = useMemo(
    () => notes.find((note) => note.id === selectedId) || null,
    [notes, selectedId]
  );

  const [draftTitle, setDraftTitle] = useState('');
  const [draftContent, setDraftContent] = useState('');
  const [isSaving, setIsSaving] = useState(false);

  const lastPersistedRef = useRef({ id: null, title: '', content: '' });
  const saveTimerRef = useRef(null);

  useEffect(() => {
    if (!currentNote) {
      setDraftTitle('');
      setDraftContent('');
      lastPersistedRef.current = { id: null, title: '', content: '' };
      return;
    }

    const initialTitle = currentNote.title || '';
    const initialContent = currentNote.content || '';

    setDraftTitle(initialTitle);
    setDraftContent(initialContent);
    lastPersistedRef.current = {
      id: currentNote.id,
      title: initialTitle,
      content: initialContent
    };
  }, [currentNote?.id]);

  useEffect(() => {
    if (!currentNote) return undefined;

    const hasChanges =
      draftTitle !== lastPersistedRef.current.title ||
      draftContent !== lastPersistedRef.current.content;

    if (!hasChanges) return undefined;

    if (saveTimerRef.current) clearTimeout(saveTimerRef.current);

    saveTimerRef.current = setTimeout(() => {
      setIsSaving(true);
      const payload = {
        id: currentNote.id,
        title: draftTitle.trim() === '' ? 'Untitled' : draftTitle,
        content: draftContent
      };
      updateNote(payload)
        .then(() => {
          lastPersistedRef.current = {
            id: currentNote.id,
            title: payload.title,
            content: payload.content
          };
        })
        .finally(() => setIsSaving(false));
    }, 240);

    return () => {
      if (saveTimerRef.current) clearTimeout(saveTimerRef.current);
    };
  }, [draftTitle, draftContent, currentNote?.id, updateNote]);

  useEffect(() => () => {
    if (saveTimerRef.current) clearTimeout(saveTimerRef.current);
  }, []);

  const applyLocalUpdate = (changes) => {
    if (!currentNote) return;
    useStore.setState((state) => ({
      notes: state.notes.map((note) =>
        note.id === currentNote.id
          ? { ...note, ...changes, updated_at: Date.now() }
          : note
      )
    }));
  };

  const handleTitleChange = (value) => {
    setDraftTitle(value);
    applyLocalUpdate({ title: value });
  };

  const handleContentChange = (value) => {
    setDraftContent(value);
    applyLocalUpdate({ content: value });
  };

  const renderedMarkdown = useMemo(
    () => (
      <ReactMarkdown
        className="prose prose-invert max-w-none"
        remarkPlugins={[remarkMath]}
        rehypePlugins={[rehypeKatex]}
      >
        {draftContent || '*Inizia a scrivere per vedere l\'anteprima live.*'}
      </ReactMarkdown>
    ),
    [draftContent]
  );

  if (!currentNote) {
    return (
      <div className="flex-1 flex items-center justify-center text-gray-400 text-lg">
        Seleziona o crea una nota per iniziare.
      </div>
    );
  }

  return (
    <div className="flex-1 flex flex-col gap-6 p-6 overflow-hidden">
      <div>
        <input
          className="w-full text-3xl font-bold bg-transparent outline-none text-gray-100 placeholder-gray-500"
          placeholder="Titolo senza limiti"
          value={draftTitle}
          onChange={(event) => handleTitleChange(event.target.value)}
        />
        <div className="mt-2 text-sm text-gray-400 flex items-center gap-2">
          {isSaving ? (
            <span className="text-emerald-300">Salvataggio…</span>
          ) : (
            <span className="text-gray-500">Tutte le modifiche sono salvate</span>
          )}
          <span className="h-1 w-1 rounded-full bg-gray-600" />
          <span>{new Date(currentNote.updated_at || Date.now()).toLocaleString()}</span>
        </div>
      </div>

      <div className="flex-1 grid grid-cols-1 gap-4 lg:grid-cols-2 min-h-0">
        <section className="flex flex-col rounded-xl border border-white/10 bg-white/5 backdrop-blur">
          <header className="flex items-center justify-between border-b border-white/10 px-4 py-2 text-sm uppercase tracking-[0.2em] text-gray-400">
            Editor Markdown
            <span className="text-xs lowercase text-gray-500">Realtime</span>
          </header>
          <textarea
            className="flex-1 w-full resize-none bg-transparent p-4 font-mono text-base leading-relaxed text-gray-100 focus:outline-none"
            placeholder="Scrivi qui usando la sintassi Markdown, supportiamo formule LaTeX, liste, heading e molto altro."
            value={draftContent}
            onChange={(event) => handleContentChange(event.target.value)}
          />
        </section>

        <section className="flex flex-col overflow-hidden rounded-xl border border-white/10 bg-white/5 backdrop-blur">
          <header className="border-b border-white/10 px-4 py-2 text-sm uppercase tracking-[0.2em] text-gray-400">
            Anteprima
          </header>
          <div className="flex-1 overflow-auto px-6 py-4 text-gray-100">
            {renderedMarkdown}
          </div>
        </section>
      </div>
    </div>
  );
}
