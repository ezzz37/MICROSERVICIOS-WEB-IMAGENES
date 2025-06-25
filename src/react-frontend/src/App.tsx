import React from 'react'
import {
  BrowserRouter as Router,
  Routes,
  Route,
  Navigate
} from 'react-router-dom'

import LoginPage     from './pages/Login/LoginPage'
import DashboardPage from './pages/Dashboard/DashboardPage'
import { useAuth }   from './utils/authHelpers'

function App() {
  const { isAuthenticated, logout } = useAuth()

  return (
    <Router>
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

        {/* Si no matchea ninguna ruta, siempre redirige a login */}
        <Route path="*" element={<Navigate to="/login" replace />} />
      </Routes>
    </Router>
  )
}

export default App
