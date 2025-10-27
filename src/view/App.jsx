import React, { useEffect, useState } from 'react';
import Sidebar from './components/Sidebar';
import EditorPane from './components/EditorPane';
import Settings from './components/Settings';
import Toast from './components/Toast';
import useStore from './store/useStore';

function App() {
  const [showSettings, setShowSettings] = useState(false);
  const [toast, setToast] = useState({ show: false, message: '' });
  const createNote = useStore((s) => s.createNote);

  useEffect(() => {
    const onKey = (e) => {
      const mod = (e.ctrlKey || e.metaKey);
      if (mod && e.key.toLowerCase() === 'n') {
        e.preventDefault();
        createNote().then(() => setToast({ show: true, message: 'Nota creata' }));
        setTimeout(() => setToast({ show: false, message: '' }), 1500);
      }
    };
    window.addEventListener('keydown', onKey);
    return () => window.removeEventListener('keydown', onKey);
  }, [createNote]);

  return (
    <div className="h-screen flex bg-gray-50 dark:bg-gray-900 text-gray-900 dark:text-gray-100">
      <Sidebar />

      <main className="flex-1 flex flex-col">
        <div className="flex items-center justify-between p-4 border-b border-gray-200 dark:border-gray-800">
          <div className="text-xl font-semibold">EasyNote</div>
          <div className="flex items-center gap-3">
            <button className="px-3 py-1 rounded" onClick={() => setShowSettings(s => !s)}>Settings</button>
          </div>
        </div>

        <div className="flex-1 flex overflow-hidden">
          <EditorPane />
          {showSettings && (
            <aside className="w-96 border-l bg-white dark:bg-gray-800">
              <Settings />
            </aside>
          )}
        </div>
      </main>

      <Toast message={toast.message} show={toast.show} />
    </div>
  );
}

export default App;
