import apiClient from './apiClient';

/**
 * Realiza login enviando username y password como parámetros separados.
 * @param {string} username
 * @param {string} password
 */
const login = async (username, password) => {
  const { data } = await apiClient.post(
    '/auth/login',
    { username, password }
  );
  return data; // { accessToken }
};

const refreshToken = async () => {
  const { data } = await apiClient.post('/auth/refresh');
  return data;
};

export default {
  login,
  refreshToken
};
