import create from 'zustand';

const useStore = create((set, get) => ({
  notes: [],
  selectedId: null,
  settings: { theme: 'light', autosave: true, fontSize: 16 },
  loadNotes: async () => {
    try {
      const notes = await window.api.getNotes();
      set({ notes: notes || [] });
    } catch (e) {
      console.error('loadNotes error', e);
    }
  },
  createNote: async (overrides = {}) => {
    const id = await window.api.addNote({ title: overrides.title || 'Untitled', content: overrides.content || '' });
    await get().loadNotes();
    set({ selectedId: id });
    return id;
  },
  updateNote: async (note) => {
    try {
      await window.api.updateNote(note);
      await get().loadNotes();
    } catch (e) {
      console.error('updateNote error', e);
    }
  },
  deleteNote: async (id) => {
    try {
      await window.api.deleteNote(id);
      await get().loadNotes();
      set({ selectedId: null });
    } catch (e) {
      console.error('deleteNote error', e);
    }
  },
  setSettings: async (settings) => {
    set({ settings });
    try {
      await window.api.setSetting('app.settings', settings);
    } catch (e) {
      console.error('setSettings error', e);
    }
  },
  loadSettings: async () => {
    try {
      const s = await window.api.getSetting('app.settings');
      if (s) set({ settings: s });
    } catch (e) {
      console.error('loadSettings error', e);
    }
  }
}));

export default useStore;
