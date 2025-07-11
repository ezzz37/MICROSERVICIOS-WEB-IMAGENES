import axios from 'axios';

const BASE_URL = ''; // puedes poner http://localhost:8081 si es necesario

const apiClient = axios.create({
  baseURL: BASE_URL,
  timeout: 10000,
  withCredentials: true, // si usas cookies de sesión
});

apiClient.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem('token');
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Interceptor de respuesta: captura 401 para redirigir al login
apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('token');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

export default apiClient;
