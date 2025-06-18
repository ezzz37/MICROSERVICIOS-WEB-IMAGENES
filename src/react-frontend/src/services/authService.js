import apiClient from './apiClient';

const login = async ({ email, password }) => {
  const { data } = await apiClient.post('/auth/login', { email, password });
  // data debería tener { token, user }
  return data;
};

const refreshToken = async () => {
  const { data } = await apiClient.post('/auth/refresh');
  return data; // { token }
};

export default { login, refreshToken };
