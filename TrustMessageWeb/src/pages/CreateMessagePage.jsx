import { useNavigate } from 'react-router-dom';
import CreateMessageForm from '../components/CreateMessageForm';

export default function CreateMessagePage() {
  const navigate = useNavigate();

  const handleMessageCreated = () => {
    navigate('/general');
  };

  return (
    <div className="page">
      <h1>Create New Message</h1>
      <CreateMessageForm onMessageCreated={handleMessageCreated} />
    </div>
  );
} 