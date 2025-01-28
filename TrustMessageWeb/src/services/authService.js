export const login = async (username, password, twoFactorCode) => {
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
    throw new Error('Login failed');
  }

  return response.json();
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
  const response = await fetch('/api/auth/register', {
    method: 'POST',
    credentials: 'include',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      username,
      email,
      password
    })
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || 'Registration failed');
  }

  return response.json();
};

export const changePassword = async (data) => {
  const response = await fetch('/api/user/change-password', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify(data),
  });

  if (!response.ok) {
    const errorData = await response.json();
    throw new Error(errorData.message || 'Failed to change password');
  }

  return response.json();
}; 