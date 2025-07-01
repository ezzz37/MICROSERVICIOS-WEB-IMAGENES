import React, { useEffect, useState } from 'react';
import ReactDOM from 'react-dom';
import './CompareModal.css';

import imagenService         from '../../services/imagenService';
import processedImageService from '../../services/processedImageService';
import comparacionesService  from '../../services/comparacionesService';

export default function CompareModal(props) {
  // Acepta tanto `isOpen` como `show` para controlar la visibilidad
  const open = typeof props.isOpen === 'boolean' ? props.isOpen : props.show;

  const [originalImages,   setOriginalImages]    = useState([]);
  const [processedImages,  setProcessedImages]   = useState([]);
  const [selOrig,          setSelOrig]           = useState('');
  const [selProc,          setSelProc]           = useState('');
  const [comparisonData,   setComparisonData]    = useState(null);

  const [loadingOriginals, setLoadingOriginals]  = useState(false);
  const [loadingProcessed, setLoadingProcessed]  = useState(false);
  const [loadingCompare,   setLoadingCompare]    = useState(false);

  const [errorOriginals,   setErrorOriginals]    = useState('');
  const [errorProcessed,   setErrorProcessed]    = useState('');
  const [errorCompare,     setErrorCompare]      = useState('');

  useEffect(() => {
    if (!open) return;

    // Reset de estados
    setOriginalImages([]);
    setProcessedImages([]);
    setSelOrig('');
    setSelProc('');
    setComparisonData(null);
    setErrorOriginals('');
    setErrorProcessed('');
    setErrorCompare('');

    // Cargo imágenes originales
    setLoadingOriginals(true);
    imagenService.list()
      .then(data => setOriginalImages(data))
      .catch(() => setErrorOriginals('No se pudo cargar las imágenes originales.'))
      .finally(() => setLoadingOriginals(false));

    // Cargo imágenes procesadas
    setLoadingProcessed(true);
    processedImageService.list()
      .then(data => setProcessedImages(data))
      .catch(() => setErrorProcessed('No se pudo cargar las imágenes procesadas.'))
      .finally(() => setLoadingProcessed(false));
  }, [open]);

  const handleCompare = async () => {
    setErrorCompare('');
    setComparisonData(null);

    if (!selOrig || !selProc) {
      setErrorCompare('Debes seleccionar ambas imágenes.');
      return;
    }
    if (selOrig === selProc) {
      setErrorCompare('No puedes comparar la misma imagen.');
      return;
    }

    setLoadingCompare(true);
    try {
      // Paso dos argumentos separados según la firma del servicio
      const result = await comparacionesService.compare(
        Number(selOrig),
        Number(selProc)
      );
      setComparisonData(result);
    } catch {
      setErrorCompare('Error al cargar los datos de comparación.');
    } finally {
      setLoadingCompare(false);
    }
  };

  if (!open) return null;

  return ReactDOM.createPortal(
    <>
      <div className="compare-modal-overlay" onClick={props.onClose} />
      <div className="compare-modal-container" onClick={e => e.stopPropagation()}>
        <h2>Comparar Imágenes</h2>

        {/* Selección de Imagen Original */}
        <div className="compare-modal-field">
          <label>Imagen Original:</label>
          {loadingOriginals ? (
            <p>Cargando originales…</p>
          ) : errorOriginals ? (
            <p className="compare-modal-error">{errorOriginals}</p>
          ) : (
            <select value={selOrig} onChange={e => setSelOrig(e.target.value)}>
              <option value="">-- Selecciona --</option>
              {originalImages.map(img => (
                <option key={img.idImagen} value={img.idImagen}>
                  {img.nombreArchivo ?? img.nombre ?? `ID ${img.idImagen}`}
                </option>
              ))}
            </select>
          )}
        </div>

        {/* Selección de Imagen Procesada */}
        <div className="compare-modal-field" style={{ marginTop: '1rem' }}>
          <label>Imagen Procesada:</label>
          {loadingProcessed ? (
            <p>Cargando procesadas…</p>
          ) : errorProcessed ? (
            <p className="compare-modal-error">{errorProcessed}</p>
          ) : (
            <select value={selProc} onChange={e => setSelProc(e.target.value)}>
              <option value="">-- Selecciona --</option>
              {processedImages.map(img => (
                <option key={img.idImagenProcesada} value={img.idImagenProcesada}>
                  {img.nombreArchivo ?? img.nombre ?? `ID ${img.idImagenProcesada}`}
                </option>
              ))}
            </select>
          )}
        </div>

        {/* Botón de Comparar */}
        <div className="compare-modal-actions" style={{ marginTop: '1.25rem' }}>
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

        {/* Resultados de la Comparación */}
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
                  <td>{new Date(comparisonData.fechaComparacion).toLocaleString()}</td>
                </tr>
                <tr>
                  <td><strong>Imagen Diferencias</strong></td>
                  <td>
                    {comparisonData.imagenDiferenciasBase64 ? (
                      <img
                        src={`data:image/png;base64,${comparisonData.imagenDiferenciasBase64}`}
                        alt="Diferencias"
                        style={{ maxWidth: '100%', height: 'auto' }}
                      />
                    ) : '–'}
                  </td>
                </tr>
              </tbody>
            </table>
          </div>
        )}

        {/* Botón Cerrar */}
        <button
          className="compare-modal-close"
          onClick={props.onClose}
          style={{ marginTop: '1.5rem' }}
        >
          Cerrar
        </button>
      </div>
    </>,
    document.body
  );
}
