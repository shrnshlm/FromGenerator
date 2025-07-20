import { useState } from 'react'
import './App.css'
import FormDisplaySystem from "./FormDisplaySystem.jsx";

function App() {
  const [count, setCount] = useState(0)

  return (
      <FormDisplaySystem></FormDisplaySystem>
  )
}

export default App
