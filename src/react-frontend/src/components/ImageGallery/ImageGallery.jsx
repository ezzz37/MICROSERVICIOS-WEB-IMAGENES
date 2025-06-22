import React, { useEffect, useState } from 'react';
import ReactDOM from 'react-dom';
import imagenService from '../../services/imagenService';
import './ImageGallery.css';

export default function ImageGallery({ onClose, onSelect }) {
  const [images, setImages]     = useState([]);
  const [loading, setLoading]   = useState(true);
  const [error, setError]       = useState(null);

  useEffect(() => {
    // Al abrir la galería, traemos la lista de imágenes
    imagenService.list()
      .then(data => setImages(data))
      .catch(() => setError('Error al cargar las imágenes.'))
      .finally(() => setLoading(false));
  }, []);

  return ReactDOM.createPortal(
    <div className="image-gallery-overlay" onClick={onClose}>
      <div className="image-gallery-container" onClick={e => e.stopPropagation()}>
        <div className="image-gallery-header">
          <h2>Imágenes Cargadas</h2>
          <button className="image-gallery-close" onClick={onClose}>×</button>
        </div>

        {loading ? (
          <p className="image-gallery-empty">Cargando imágenes…</p>
        ) : error ? (
          <p className="image-gallery-empty">{error}</p>
        ) : images.length === 0 ? (
          <p className="image-gallery-empty">No hay imágenes para mostrar.</p>
        ) : (
          <div className="image-gallery-grid">
            {images.map(img => (
              <div
                key={img.idImagen}
                className="image-gallery-item"
                onClick={() => {
                  onSelect(img);
                  onClose();
                }}
              >
                <img
                  src={`data:image/png;base64,${img.datosImagenBase64}`}
                  alt={img.nombreArchivo || `Imagen ${img.idImagen}`}
                />
              </div>
            ))}
          </div>
        )}
      </div>
    </div>,
    document.body
  );
}
