import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import LoginPage from './pages/Login/LoginPage';
import DashboardPage from './pages/Dashboard/DashboardPage';
import { useAuth } from './utils/authHelpers';

function App() {
  const { isAuthenticated } = useAuth();

  return (
    <Routes>
      {/* Ruta raíz: si estás logueado vas al dashboard, si no al login */}
      <Route
        path="/"
        element={
          isAuthenticated
            ? <Navigate to="/dashboard" replace />
            : <Navigate to="/login" replace />
        }
      />

      {/* Login */}
      <Route
        path="/login"
        element={
          isAuthenticated
            ? <Navigate to="/dashboard" replace />
            : <LoginPage />
        }
      />

      {/* Dashboard */}
      <Route
        path="/dashboard"
        element={
          isAuthenticated
            ? <DashboardPage />
            : <Navigate to="/login" replace />
        }
      />

      {/* Cualquier otra ruta redirige a raíz */}
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;
