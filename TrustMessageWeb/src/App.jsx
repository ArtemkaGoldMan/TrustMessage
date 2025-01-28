import { useEffect, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import LoginForm from './components/LoginForm'
import { checkAuth, logout } from './services/authService'
import './App.css'

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [user, setUser] = useState(null)

  useEffect(() => {
    const verifyAuth = async () => {
      try {
        const userData = await checkAuth()
        setIsAuthenticated(true)
        setUser(userData)
      } catch (err) {
        setIsAuthenticated(false)
        setUser(null)
      }
    }

    verifyAuth()
  }, [])

  const handleLoginSuccess = () => {
    setIsAuthenticated(true)
    checkAuth().then(setUser)
  }

  const handleLogout = async () => {
    await logout()
    setIsAuthenticated(false)
    setUser(null)
  }

  return (
    <div className="App">
      {isAuthenticated ? (
        <div>
          <h1>Welcome, {user?.username}</h1>
          <button onClick={handleLogout}>Logout</button>
          {/* Your main application content here */}
        </div>
      ) : (
        <LoginForm onLoginSuccess={handleLoginSuccess} />
      )}
    </div>
  )
}

export default App
