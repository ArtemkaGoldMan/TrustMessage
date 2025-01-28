import { useState } from 'react';
import { register } from '../services/authService';
import QRCode from 'qrcode';

export default function RegistrationForm({ onRegistrationSuccess }) {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [qrCodeDataUrl, setQrCodeDataUrl] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [registrationComplete, setRegistrationComplete] = useState(false);
  const [secretKey, setSecretKey] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setQrCodeDataUrl('');
    setSecretKey('');
    setIsLoading(true);

    try {
      const response = await register(username, email, password);
      const extractedSecretKey = response.qrCodeUri.split('secret=')[1]?.split('&')[0];
      setSecretKey(extractedSecretKey || '');
      
      // Try to generate QR code, but show secret key regardless
      QRCode.toDataURL(response.qrCodeUri, { errorCorrectionLevel: 'H' }, (err, url) => {
        setIsLoading(false);
        if (err) {
          console.warn('QR Code generation failed, showing secret key only');
        }
        setQrCodeDataUrl(url || '');
        setRegistrationComplete(true);
      });
    } catch (err) {
      setIsLoading(false);
      setError(err.message);
    }
  };

  if (registrationComplete) {
    return (
      <div className="registration-complete">
        {qrCodeDataUrl ? (
          <div className="qr-code-container">
            <p>Registration successful! Scan this QR code with your authenticator app:</p>
            <img src={qrCodeDataUrl} alt="2FA QR Code" />
          </div>
        ) : (
          <div className="qr-code-container">
            <p>Registration successful! Use this secret key in your authenticator app:</p>
          </div>
        )}
        <div className="secret-key-container">
          <p className="qr-instructions">
            Your secret key:
            <br />
            <code>{secretKey}</code>
          </p>
        </div>
        <button 
          onClick={onRegistrationSuccess}
          className="continue-button"
        >
          Continue to Login
        </button>
      </div>
    );
  }

  return (
    <form onSubmit={handleSubmit}>
      {error && <div className="error">{error}</div>}
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
        <label>Email:</label>
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
      </div>
      <div>
        <label>Password:</label>
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
      </div>
      <button type="submit" disabled={isLoading}>
        {isLoading ? 'Registering...' : 'Register'}
      </button>
    </form>
  );
} 