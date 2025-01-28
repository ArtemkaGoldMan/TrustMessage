export const getAllMessages = async () => {
  const response = await fetch('/api/messages/all', {
    credentials: 'include'
  });

  if (!response.ok) {
    throw new Error('Failed to fetch messages');
  }

  return response.json();
};

export const getUserMessages = async () => {
  const response = await fetch('/api/messages/my', {
    credentials: 'include'
  });

  if (!response.ok) {
    throw new Error('Failed to fetch user messages');
  }

  return response.json();
};

export const createMessage = async (message) => {
  const response = await fetch('/api/messages', {
    method: 'POST',
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      ...message,
      content: message.content // Server will handle sanitization
    })
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || 'Failed to create message');
  }

  return response.json();
}; 