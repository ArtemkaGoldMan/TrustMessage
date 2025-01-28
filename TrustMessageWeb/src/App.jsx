import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import Layout from './components/Layout';
import CreateMessagePage from './pages/CreateMessagePage';
import GeneralMessagesPage from './pages/GeneralMessagesPage';
import PersonalMessagesPage from './pages/PersonalMessagesPage';
import ChangePasswordPage from './pages/ChangePasswordPage';
import LoginForm from './components/LoginForm';
import RegistrationForm from './components/RegistrationForm';
import { checkAuth } from './services/authService';
import { useState, useEffect } from 'react';
import './App.css';

function App() {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [showRegistration, setShowRegistration] = useState(false);

  useEffect(() => {
    let mounted = true;
    let timeoutId = null;

    const verifyAuth = async () => {
      try {
        const result = await checkAuth();
        if (mounted) {
          setIsAuthenticated(true);
          setIsLoading(false);
        }
      } catch (err) {
        if (mounted) {
          setIsAuthenticated(false);
          setIsLoading(false);
        }
      }
    };

    verifyAuth();

    return () => {
      mounted = false;
      if (timeoutId) clearTimeout(timeoutId);
    };
  }, []);

  const handleLoginSuccess = () => {
    setIsAuthenticated(true);
  };

  if (isLoading) {
    return <div>Loading...</div>;
  }

  return (
    <BrowserRouter>
      <Routes>
        {isAuthenticated ? (
          <Route element={<Layout />}>
            <Route path="/" element={<Navigate to="/create" replace />} />
            <Route path="/create" element={<CreateMessagePage />} />
            <Route path="/general" element={<GeneralMessagesPage />} />
            <Route path="/personal" element={<PersonalMessagesPage />} />
            <Route path="/change-password" element={<ChangePasswordPage />} />
            <Route path="*" element={<Navigate to="/create" replace />} />
          </Route>
        ) : (
          <Route path="*" element={
            <div className="auth-container">
              {showRegistration ? (
                <>
                  <RegistrationForm onRegistrationSuccess={() => setShowRegistration(false)} />
                  <div className="auth-switch-button">
                    <button onClick={() => setShowRegistration(false)}>
                      Already have an account? Login
                    </button>
                  </div>
                </>
              ) : (
                <>
                  <LoginForm onLoginSuccess={handleLoginSuccess} />
                  <div className="auth-switch-button">
                    <button onClick={() => setShowRegistration(true)}>
                      Don't have an account? Register
                    </button>
                  </div>
                </>
              )}
            </div>
          } />
        )}
      </Routes>
    </BrowserRouter>
  );
}

export default App;