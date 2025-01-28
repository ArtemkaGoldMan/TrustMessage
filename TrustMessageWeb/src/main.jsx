import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import './index.css'
import App from './App.jsx'

// Ensure the app is running on HTTPS
if (window.location.protocol === 'http:' && !window.location.hostname.includes('localhost')) {
  window.location.href = window.location.href.replace('http:', 'https:');
}

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <App />
  </StrictMode>,
)
