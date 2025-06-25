// src/services/authService.js

import apiClient from './apiClient';

/**
 * Realiza login enviando username y password como parámetros separados.
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
 * Refresca el token de acceso usando la cookie/session actual.
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
