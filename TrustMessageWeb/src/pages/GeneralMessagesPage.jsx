import { useState, useEffect } from 'react';
import { getAllMessages } from '../services/messageService';
import MessageList from '../components/MessageList';

export default function GeneralMessagesPage() {
  const [messages, setMessages] = useState([]);
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const fetchMessages = async () => {
      setIsLoading(true);
      try {
        const data = await getAllMessages();
        setMessages(data);
      } catch (err) {
        console.error('Error fetching messages:', err);
      } finally {
        setIsLoading(false);
      }
    };

    fetchMessages();
  }, []);

  return (
    <div className="page">
      <h1>General Messages</h1>
      {isLoading ? (
        <p>Loading messages...</p>
      ) : (
        <MessageList messages={messages} />
      )}
    </div>
  );
} 