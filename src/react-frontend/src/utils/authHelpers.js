// src/utils/authHelpers.js

import React, { createContext, useContext, useEffect, useState } from 'react';
import authService from '../services/authService';

const AuthContext = createContext({
  login: async (username, password) => {},
  refresh: async () => {},
  isAuthenticated: false,
});

export function AuthProvider({ children }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false);

  // Llama a authService.login con (username, password)
  const login = async (username, password) => {
    // authService.login devuelve { accessToken }
    const { accessToken } = await authService.login(username, password);
    // Guarda el token y marca como autenticado
    localStorage.setItem('token', accessToken);
    setIsAuthenticated(true);
  };

  // Refresca el token cuando haga falta
  const refresh = async () => {
    const { accessToken } = await authService.refreshToken();
    localStorage.setItem('token', accessToken);
  };

  // Al montar, comprueba si hay token ya guardado
  useEffect(() => {
    if (localStorage.getItem('token')) {
      setIsAuthenticated(true);
    }
  }, []);

  return (
    <AuthContext.Provider value={{ login, refresh, isAuthenticated }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) {
    throw new Error('useAuth must be used inside an AuthProvider');
  }
  return ctx;
}
