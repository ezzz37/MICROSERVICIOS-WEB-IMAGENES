import React from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';

import LoginPage     from './pages/Login/LoginPage';
import DashboardPage from './pages/Dashboard/DashboardPage';
import { useAuth }   from './utils/authHelpers';

function App() {
  const { isAuthenticated } = useAuth();

  return (
    <Routes>
      {/* 1) Siempre redirijo "/" a "/login" */}
      <Route path="/" element={<Navigate to="/login" replace />} />

      {/* 2) Login */}
      <Route path="/login" element={<LoginPage />} />

      {/* 3) Dashboard protegido */}
      <Route
        path="/dashboard"
        element={
          isAuthenticated
            ? <DashboardPage />
            : <Navigate to="/login" replace />
        }
      />

      {/* 4) Cualquier otra ruta → /login */}
      <Route path="*" element={<Navigate to="/login" replace />} />
    </Routes>
  );
}

export default App;
