import { useState } from 'react';
import { login } from '../services/authService';

export default function LoginForm({ onLoginSuccess }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [twoFactorCode, setTwoFactorCode] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (!username.trim() || !password.trim() || !twoFactorCode.trim()) {
      setError('All fields are required');
      return;
    }

    try {
      await login(username, password, twoFactorCode);
      onLoginSuccess();
    } catch (err) {
      console.log('Login error:', err);
      if (err.response?.data?.errors) {
        const errorMessages = Object.entries(err.response.data.errors)
          .map(([field, messages]) => messages)
          .flat()
          .join('\n');
        setError(errorMessages);
      } else {
        setError(err.response?.data?.message || 'Login failed');
      }
    }
  };

  return (
    <div className="auth-form">
      <h2>Login to TrustMessage</h2>
      {error && (
        <div className="error">
          {error.split('\n').map((line, index) => (
            <div key={index}>{line}</div>
          ))}
        </div>
      )}
      <form onSubmit={handleSubmit}>
        <div className="form-group">
          <label>Username:</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label>Password:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <div className="form-group">
          <label>Two-Factor Code:</label>
          <input
            type="text"
            value={twoFactorCode}
            onChange={(e) => setTwoFactorCode(e.target.value)}
            required
          />
        </div>
        <button type="submit" className="auth-button">Login</button>
      </form>
    </div>
  );
} 