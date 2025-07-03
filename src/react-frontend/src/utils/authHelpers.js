import React, { createContext, useContext, useEffect, useState } from 'react';
import authService from '../services/authService';

const AuthContext = createContext({
  login: async (username, password) => {},
  refresh: async () => {},
  logout: () => {},
  isAuthenticated: false,
});

export function AuthProvider({ children }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  // Llama a authService.login con (username, password)
  const login = async (username, password) => {
    const { accessToken } = await authService.login(username, password);
    localStorage.setItem('token', accessToken);
    setIsAuthenticated(true);
  };

  // Refresca el token cuando haga falta (revalida en backend)
  const refresh = async () => {
    const { accessToken } = await authService.refreshToken();
    localStorage.setItem('token', accessToken);
    setIsAuthenticated(true);
  };

  // Cierra sesión: borra token y marca como no autenticado
  const logout = () => {
    authService.logout();
    localStorage.removeItem('token');
    setIsAuthenticated(false);
  };

  // Al montar, intento un refresh real en el backend.
  // Si falla, fuerza al login; si pasa, deja el dashboard accesible.
  useEffect(() => {
    (async () => {
      try {
        const { accessToken } = await authService.refreshToken();
        localStorage.setItem('token', accessToken);
        setIsAuthenticated(true);
      } catch {
        localStorage.removeItem('token');
        setIsAuthenticated(false);
      }
    })();
  }, []);

  return (
    <AuthContext.Provider value={{ login, refresh, logout, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error('useAuth debe usarse dentro de un AuthProvider');
  }
  return ctx;
}
