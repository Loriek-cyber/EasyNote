import { useState } from 'react'
import './App.css'

function App() {
  if (win.database.isok()) {
    return (
      <div className="App">
        <h1>Database Connected</h1>
      </div>
    )
  } else {
    return (
      <div className="App">
        <h1>Database Not Connected</h1>
      </div>
    )
  }
}

export default App
