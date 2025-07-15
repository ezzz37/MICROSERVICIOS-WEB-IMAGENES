import apiClient from './apiClient';

/**
 * @param {string} username
 * @param {string} password
 * @returns {Promise<{ accessToken: string }>}
 */
const login = async (username, password) => {
  const { data } = await apiClient.post(
    '/auth/login',
    { Username: username, Password: password }
  );
  // data === { accessToken: '...' }
  return data;
};

/**
 * @returns {Promise<{ accessToken: string }>}
 */
const refreshToken = async () => {
  console.log('RefreshToken disabled to avoid CORS issues');
  return Promise.reject(new Error('Refresh token not implemented'));
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
