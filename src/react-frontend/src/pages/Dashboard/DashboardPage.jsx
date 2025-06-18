import React, { useEffect, useState } from 'react';
import { useAuth } from '../utils/authHelpers';
import { Navigate } from 'react-router-dom';

import imagenService from '../services/imagenService';
import processedImageService from '../services/processedImageService';
import comparacionesService from '../services/comparacionesService';

const DashboardPage = () => {
  const { isAuthenticated, logout } = useAuth();
  const [rawImages, setRawImages] = useState([]);
  const [processedImages, setProcessedImages] = useState([]);
  const [toCompare, setToCompare] = useState([]);        // [id1, id2]
  const [comparisonResult, setComparisonResult] = useState(null);

  useEffect(() => {
    if (!isAuthenticated) return;
    imagenService.list().then(setRawImages);
    processedImageService.list().then(setProcessedImages);
  }, [isAuthenticated]);

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  const handleProcess = async idImagen => {
    await processedImageService.process(idImagen);
    const list = await processedImageService.list();
    setProcessedImages(list);
  };

  const toggleCompare = id => {
    setToCompare(cur => {
      if (cur.includes(id)) return cur.filter(x => x !== id);
      if (cur.length < 2) return [...cur, id];
      return cur; // ya hay dos seleccionados
    });
  };

  const doCompare = async () => {
    if (toCompare.length !== 2) return;
    const [id1, id2] = toCompare;
    const result = await comparacionesService.compare(id1, id2);
    setComparisonResult(result);
  };

  return (
    <div className="dashboard-page">
      <header>
        <h2>Dashboard</h2>
        <button onClick={logout}>Cerrar sesión</button>
      </header>

      <section>
        <h3>Imágenes crudas</h3>
        <ul>
          {rawImages.map(img => (
            <li key={img.id}>
              <img src={img.url} alt="" width={100} />
              <button onClick={() => handleProcess(img.id)}>
                Procesar
              </button>
            </li>
          ))}
        </ul>
      </section>

      <section>
        <h3>Imágenes procesadas</h3>
        <ul>
          {processedImages.map(img => (
            <li key={img.id}>
              <img src={img.urlProcesada} alt="" width={100} />
              <label>
                <input
                  type="checkbox"
                  checked={toCompare.includes(img.id)}
                  onChange={() => toggleCompare(img.id)}
                />
                Seleccionar
              </label>
            </li>
          ))}
        </ul>
      </section>

      <section>
        <h3>Comparar</h3>
        <button
          disabled={toCompare.length !== 2}
          onClick={doCompare}
        >
          Ejecutar comparación
        </button>
        {comparisonResult && (
          <pre>{JSON.stringify(comparisonResult, null, 2)}</pre>
        )}
      </section>
    </div>
  );
};

export default DashboardPage;
