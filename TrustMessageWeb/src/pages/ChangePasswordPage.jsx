import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { changePassword } from '../services/authService';

export default function ChangePasswordPage() {
  const [username, setUsername] = useState('');
  const [twoFactorCode, setTwoFactorCode] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState(false);
  const navigate = useNavigate();

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setSuccess(false);

    if (!username.trim() || !twoFactorCode.trim() || !newPassword.trim()) {
      setError('All fields are required');
      return;
    }

    try {
      await changePassword({ username, twoFactorCode, newPassword });
      setSuccess(true);
      setTimeout(() => navigate('/'), 2000);
    } catch (err) {
      console.log('Password change error:', err);
      if (err.response?.data?.errors) {
        const errorMessages = Object.entries(err.response.data.errors)
          .map(([field, messages]) => messages)
          .flat()
          .join('\n');
        setError(errorMessages);
      } else {
        setError(err.response?.data?.message || 'Failed to change password');
      }
    }
  };

  return (
    <div className="page">
      <h1>Change Password</h1>
      <form onSubmit={handleSubmit} className="change-password-form">
        <div>
          <label>Username:</label>
          <input
            type="text"
            value={username}
            onChange={(e) => setUsername(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Two-Factor Code:</label>
          <input
            type="text"
            value={twoFactorCode}
            onChange={(e) => setTwoFactorCode(e.target.value)}
            required
          />
        </div>
        <div>
          <label>New Password:</label>
          <input
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            required
          />
        </div>
        {error && (
          <div className="error">
            {error.split('\n').map((line, index) => (
              <div key={index}>{line}</div>
            ))}
          </div>
        )}
        {success && <div className="success">Password changed successfully! Redirecting...</div>}
        <button type="submit">Change Password</button>
      </form>
    </div>
  );
} 