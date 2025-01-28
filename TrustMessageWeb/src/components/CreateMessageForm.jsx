import { useState } from 'react';
import { createMessage } from '../services/messageService';
import MarkdownEditor from './MarkdownEditor';

export default function CreateMessageForm({ onMessageCreated }) {
  const [content, setContent] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (!content.trim() || !password.trim()) {
      setError('Both message content and password are required');
      return;
    }

    try {
      await createMessage({ content, password });
      setContent('');
      setPassword('');
      if (onMessageCreated) {
        onMessageCreated();
      }
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="create-message-form">
      <h2>Create New Message</h2>
      {error && <div className="error">{error}</div>}
      <form onSubmit={handleSubmit}>
        <MarkdownEditor value={content} onChange={setContent} />
        <div>
          <label>Password:</label>
          <input
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            required
          />
        </div>
        <button type="submit">Post Message</button>
      </form>
    </div>
  );
} 