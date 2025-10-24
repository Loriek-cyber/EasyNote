import { useState, useEffect, useCallback } from 'react'
import './App.css'

function App() {
  return (
    <div className="app-container p-6">
      <h1 className="text-2xl font-bold mb-4">My Slate Editor</h1>
      <SlateEditor />
    </div>
  );
}

export default App
