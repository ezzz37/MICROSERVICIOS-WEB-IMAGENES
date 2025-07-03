import apiClient from './apiClient';

/**
 * @param {string} username
 * @param {string} password
 * @returns {Promise<{ accessToken: string }>}
 */
const login = async (username, password) => {
  const { data } = await apiClient.post(
    '/auth/login',
    { username, password }
  );
  // data === { accessToken: '...' }
  return data;
};

/**
 * @returns {Promise<{ accessToken: string }>}
 */
const refreshToken = async () => {
  const { data } = await apiClient.post('/auth/refresh');
  return data;
};

/**
 * Cierra la sesión en cliente (borra token local)
 */
const logout = () => {
  localStorage.removeItem('token');
};

export default {
  login,
  refreshToken,
  logout
};
