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

export const createMessage = async (messageData) => {
  try {
    const response = await fetch('/api/messages', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(messageData),
      credentials: 'include'
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error('Failed to create message');
      error.response = { data: errorData };
      throw error;
    }

    return response.json();
  } catch (error) {
    if (error.response) {
      throw error;
    }
    const newError = new Error(error.message);
    newError.response = { data: { message: error.message } };
    throw newError;
  }
}; 