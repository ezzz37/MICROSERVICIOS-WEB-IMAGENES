import axios from 'axios';

// Usar rutas relativas para que nginx haga el proxy
// Build timestamp: 2025-07-14T07:56:00Z - LOGIN REQUIRED ALWAYS
const BASE_URL = ''; // Rutas relativas: /auth/login, /api/imagenes, etc.

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
    // DEBUG: Log de todas las requests
    console.log('API Request:', config.method?.toUpperCase(), config.url, 'BaseURL:', config.baseURL);
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
