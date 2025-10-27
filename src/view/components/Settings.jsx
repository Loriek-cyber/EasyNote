import React, { useEffect, useState } from 'react';
import useStore from '../store/useStore';

export default function Settings() {
  const settings = useStore((s) => s.settings);
  const loadSettings = useStore((s) => s.loadSettings);
  const setSettings = useStore((s) => s.setSettings);

  useEffect(() => { loadSettings(); // run once on mount
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  const [local, setLocal] = useState(settings);

  useEffect(() => setLocal(settings), [settings]);

  const save = () => setSettings(local);

  return (
    <div className="p-4">
      <h3 className="text-lg font-bold mb-3">Settings</h3>
      <div className="space-y-3">
        <label className="block">
          Theme
          <select value={local.theme} onChange={(e) => setLocal({ ...local, theme: e.target.value })} className="ml-2">
            <option value="light">Light</option>
            <option value="dark">Dark</option>
            <option value="nord">Nord</option>
            <option value="monokai">Monokai</option>
            <option value="catppuccin">Catppuccin</option>
          </select>
        </label>

        <label className="block">
          Font size
          <input type="range" min="12" max="22" value={local.fontSize} onChange={(e) => setLocal({ ...local, fontSize: Number(e.target.value) })} />
        </label>

        <label className="flex items-center">
          <input type="checkbox" checked={local.autosave} onChange={(e) => setLocal({ ...local, autosave: e.target.checked })} />
          <span className="ml-2">Autosave</span>
        </label>

        <button className="px-3 py-1 bg-blue-600 text-white rounded" onClick={save}>Save</button>
      </div>
    </div>
  );
}
