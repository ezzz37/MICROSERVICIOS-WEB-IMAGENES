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
    // Temporalmente deshabilitado para evitar errores CORS
    console.log('Refresh disabled to avoid CORS issues');
    return Promise.reject('Refresh not implemented');
  };

  // Cierra sesión: borra token y marca como no autenticado
  const logout = () => {
    authService.logout();
    localStorage.removeItem('token');
    setIsAuthenticated(false);
  };

  // Al montar, siempre empieza sin autenticar
  // El usuario debe hacer login explícitamente
  useEffect(() => {
    // Limpiar cualquier token anterior para forzar login
    localStorage.removeItem('token');
    setIsAuthenticated(false);
  }, []);

  // DEBUG: Log para verificar el baseURL
  console.log('API Client configured for:', window.location.origin);

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
