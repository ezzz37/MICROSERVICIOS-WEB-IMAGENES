import React, { useState, useEffect } from 'react';
import { FaUser } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../../utils/authHelpers';
import './Login.css';

const LoginPage = () => {
  const { login, isAuthenticated } = useAuth();
  const navigate                   = useNavigate();

  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError]       = useState('');

  // Si ya está autenticado, lo mando al dashboard
  useEffect(() => {
    if (isAuthenticated) {
      navigate('/dashboard', { replace: true });
    }
  }, [isAuthenticated, navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    // -- DEBUG: confirmar que llegan ambos valores --
    console.log('Login payload:', { username, password });

    try {
      // <-- Aquí pasamos los dos parámetros separados -->
      await login(username, password);
      // El hook updateará isAuthenticated y el useEffect redirigirá
    } catch (err) {
      // Capturo el mensaje que venga del backend (o uno genérico)
      const serverMsg =
        err.response?.data?.message ||
        err.response?.data?.detail ||
        err.response?.data?.title;
      setError(serverMsg || err.message || 'Usuario o contraseña inválidos');
    }
  };

  return (
    <div className="login-page">
      <div className="login-card">
        <div className="login-header">
          <div className="icon-circle">
            <FaUser size={32} color="#FF7B00" />
          </div>
        </div>

        <div className="login-body">
          <h2 className="login-title">Iniciar Sesión</h2>
          {error && <p className="login-error">{error}</p>}

          <form className="login-form" onSubmit={handleSubmit}>
            <label htmlFor="username">Nombre de Usuario</label>
            <input
              id="username"
              type="text"
              placeholder="Ingresa tu nombre"
              value={username}
              onChange={e => setUsername(e.target.value)}
              required
            />

            <label htmlFor="password">Contraseña</label>
            <input
              id="password"
              type="password"
              placeholder="Ingresa tu contraseña"
              value={password}
              onChange={e => setPassword(e.target.value)}
              required
            />

            <button type="submit" className="btn-login">
              Ingresar
            </button>
          </form>

          <a href="#" className="forgot-link">
            ¿Olvidaste tu contraseña?
          </a>
        </div>
      </div>
    </div>
  );
};

export default LoginPage;
