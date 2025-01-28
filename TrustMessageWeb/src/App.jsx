import { useEffect, useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import LoginForm from './components/LoginForm'
import RegistrationForm from './components/RegistrationForm'
import { checkAuth, logout } from './services/authService'
import './App.css'

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [user, setUser] = useState(null)
  const [showRegistration, setShowRegistration] = useState(false)

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

  const handleRegistrationSuccess = () => {
    setShowRegistration(false)
    // Optionally automatically log in the user after registration
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
        <div>
          {showRegistration ? (
            <RegistrationForm onRegistrationSuccess={handleRegistrationSuccess} />
          ) : (
            <LoginForm onLoginSuccess={handleLoginSuccess} />
          )}
          <button onClick={() => setShowRegistration(!showRegistration)}>
            {showRegistration ? 'Back to Login' : 'Register'}
          </button>
        </div>
      )}
    </div>
  )
}

export default App
