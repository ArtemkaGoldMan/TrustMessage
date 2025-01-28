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