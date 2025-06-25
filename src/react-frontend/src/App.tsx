// src/App.tsx

import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';

import LoginPage     from './pages/Login/LoginPage';
import DashboardPage from './pages/Dashboard/DashboardPage';
import { useAuth }   from './utils/authHelpers';

function App() {
  const { isAuthenticated, logout } = useAuth();

  return (
    <Routes>
      <Route path="/" element={<Navigate to="/login" replace />} />
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/dashboard"
        element={
          isAuthenticated
            ? <DashboardPage onLogout={logout} />
            : <Navigate to="/login" replace />
        }
      />
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default App;
