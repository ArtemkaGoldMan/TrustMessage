export const login = async (username, password, twoFactorCode) => {
  try {
    const response = await fetch('/api/auth/login', {
      method: 'POST',
      credentials: 'include', // Include cookies
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        username,
        password,
        twoFactorCode
      })
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error('Login failed');
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

export const checkAuth = async () => {
  const response = await fetch('/api/auth/check', {
    credentials: 'include'
  });

  if (!response.ok) {
    throw new Error('Not authenticated');
  }

  return response.json();
};

export const logout = async () => {
  const response = await fetch('/api/auth/logout', {
    method: 'POST',
    credentials: 'include'
  });

  if (!response.ok) {
    throw new Error('Logout failed');
  }

  return response.json();
};

export const register = async (username, email, password) => {
  try {
    const response = await fetch('/api/auth/register', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({ username, email, password }),
      credentials: 'include'
    });

    if (!response.ok) {
      const errorData = await response.json();
      // Create an error object with the response data
      const error = new Error('Registration failed');
      error.response = { data: errorData };
      throw error;
    }

    const data = await response.json();
    return data;
  } catch (error) {
    if (error.response) {
      throw error; // Rethrow if it's our custom error
    }
    // Create a new error with response structure for network errors
    const newError = new Error(error.message);
    newError.response = { data: { message: error.message } };
    throw newError;
  }
};

export const changePassword = async (data) => {
  try {
    const response = await fetch('/api/user/change-password', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify(data),
      credentials: 'include'
    });

    if (!response.ok) {
      const errorData = await response.json();
      const error = new Error('Password change failed');
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