import React, { useEffect, useState } from 'react';
import ReactDOM from 'react-dom';
import './CompareModal.css';

import imagenService         from '../../services/imagenService';
import processedImageService from '../../services/processedImageService';
import comparacionesService  from '../../services/comparacionesService';

export default function CompareModal({ isOpen, onClose }) {
  //–– 1) lista de imágenes originales ––
  const [originalImages, setOriginalImages] = useState([]);
  const [loadingOriginals, setLoadingOriginals] = useState(false);
  const [errorOriginals, setErrorOriginals] = useState(null);

  //–– 2) lista de imágenes procesadas ––
  const [processedImages, setProcessedImages] = useState([]);
  const [loadingProcessed, setLoadingProcessed] = useState(false);
  const [errorProcessed, setErrorProcessed] = useState(null);

  //–– 3) selección y resultado de la comparación ––
  const [selectedOriginalId, setSelectedOriginalId] = useState('');
  const [selectedProcessedId, setSelectedProcessedId] = useState('');
  const [comparisonData, setComparisonData] = useState(null);
  const [loadingCompare, setLoadingCompare] = useState(false);
  const [errorCompare, setErrorCompare] = useState(null);

  // Cada vez que se abra el modal, recargamos todo
  useEffect(() => {
    if (!isOpen) return;

    setOriginalImages([]);
    setErrorOriginals(null);
    setLoadingOriginals(true);

    setProcessedImages([]);
    setErrorProcessed(null);
    setLoadingProcessed(true);

    setSelectedOriginalId('');
    setSelectedProcessedId('');
    setComparisonData(null);
    setErrorCompare(null);
    setLoadingCompare(false);

    imagenService.list()
      .then(data => setOriginalImages(data))
      .catch(() => setErrorOriginals('No se pudo cargar las imágenes originales.'))
      .finally(() => setLoadingOriginals(false));

    processedImageService.list()
      .then(data => setProcessedImages(data))
      .catch(() => setErrorProcessed('No se pudo cargar las imágenes procesadas.'))
      .finally(() => setLoadingProcessed(false));
  }, [isOpen]);

  const handleCompare = async () => {
    setErrorCompare(null);
    setComparisonData(null);

    if (!selectedOriginalId || !selectedProcessedId) {
      return setErrorCompare('Debes seleccionar ambas imágenes.');
    }
    if (selectedOriginalId === selectedProcessedId) {
      return setErrorCompare('No puedes comparar la misma imagen consigo misma.');
    }

    setLoadingCompare(true);
    try {
      const result = await comparacionesService.compare({
        IdImagenOriginal:   Number(selectedOriginalId),
        IdImagenProcesada:  Number(selectedProcessedId),
      });
      setComparisonData(result);
    } catch {
      setErrorCompare('Error al cargar los datos de comparación.');
    } finally {
      setLoadingCompare(false);
    }
  };

  if (!isOpen) return null;

  return ReactDOM.createPortal(
    <div
      className="compare-modal-overlay"
      onClick={onClose}
    >
      <div
        className="compare-modal-container"
        onClick={e => e.stopPropagation()}
      >
        <h2>Comparar Imágenes</h2>

        {/* === Selección Imagen Original === */}
        <div className="compare-modal-field">
          <label>Imagen Original:</label>
          {loadingOriginals
            ? <p>Cargando originales…</p>
            : errorOriginals
              ? <p className="compare-modal-error">{errorOriginals}</p>
              : (
                <select
                  value={selectedOriginalId}
                  onChange={e => setSelectedOriginalId(e.target.value)}
                >
                  <option value="">-- Selecciona --</option>
                  {originalImages.map(img => (
                    <option key={img.idImagen} value={img.idImagen}>
                      {img.nombreArchivo || `ID ${img.idImagen}`}
                    </option>
                  ))}
                </select>
              )
          }
        </div>

        {/* === Selección Imagen Procesada === */}
        <div className="compare-modal-field" style={{ marginTop: '1rem' }}>
          <label>Imagen Procesada:</label>
          {loadingProcessed
            ? <p>Cargando procesadas…</p>
            : errorProcessed
              ? <p className="compare-modal-error">{errorProcessed}</p>
              : (
                <select
                  value={selectedProcessedId}
                  onChange={e => setSelectedProcessedId(e.target.value)}
                >
                  <option value="">-- Selecciona --</option>
                  {processedImages.map(img => (
                    <option
                      key={img.idImagenProcesada}
                      value={img.idImagenProcesada}
                    >
                      {`ID ${img.idImagenProcesada} (orig: ${img.idImagenOriginal})`}
                    </option>
                  ))}
                </select>
              )
          }
        </div>

        {/* === Botón Comparar === */}
        <div style={{ marginTop: '1.25rem' }}>
          <button
            className="compare-modal-button"
            onClick={handleCompare}
            disabled={loadingCompare}
          >
            {loadingCompare ? 'Comparando…' : 'Mostrar Comparación'}
          </button>
          {errorCompare && (
            <p className="compare-modal-error" style={{ marginTop: '0.5rem' }}>
              {errorCompare}
            </p>
          )}
        </div>

        {/* === Resultados === */}
        {comparisonData && (
          <div className="compare-modal-results" style={{ marginTop: '1.5rem' }}>
            <h3>Resultado de la Comparación</h3>
            <table className="compare-modal-table">
              <tbody>
                <tr>
                  <td><strong>MSE</strong></td>
                  <td>{comparisonData.mse}</td>
                </tr>
                <tr>
                  <td><strong>PSNR</strong></td>
                  <td>{comparisonData.psnr}</td>
                </tr>
                <tr>
                  <td><strong>Fecha</strong></td>
                  <td>
                    {new Date(comparisonData.fechaComparacion)
                      .toLocaleString()}
                  </td>
                </tr>
                <tr>
                  <td><strong>Imagen Diferencias</strong></td>
                  <td>
                    {comparisonData.imagenDiferenciasBase64
                      ? <img
                          src={`data:image/png;base64,${comparisonData.imagenDiferenciasBase64}`}
                          alt="Diferencias"
                          style={{ maxWidth: '100%', height: 'auto' }}
                        />
                      : '–'
                    }
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        )}

        {/* === Cerrar Modal === */}
        <button
          className="compare-modal-close"
          onClick={onClose}
          style={{ marginTop: '1.5rem' }}
        >
          Cerrar
        </button>
      </div>
    </div>,
    document.body
  );
}
