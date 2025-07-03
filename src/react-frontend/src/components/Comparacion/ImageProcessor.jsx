import React, { useState } from 'react';
import imagenService         from '../../services/imagenService';
import processedImageService from '../../services/processedImageService';
import CompareModal          from '../Comparacion/CompareModal';
import './ImageProcessor.css';

export default function ImageProcessor() {
  const [file, setFile] = useState(null);
  const [originalPreview, setOriginalPreview] = useState(null);
  const [uploadedImage, setUploadedImage] = useState(null);
  const [processedImage, setProcessedImage] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');
  const [isCompareOpen, setIsCompareOpen] = useState(false);

  // Cuando el usuario selecciona un archivo
  const handleFileChange = (e) => {
    const f = e.target.files[0];
    if (!f) return;
    setFile(f);
    setError('');
    setUploadedImage(null);
    setProcessedImage(null);

    // Vista previa local
    const reader = new FileReader();
    reader.onloadend = () => setOriginalPreview(reader.result);
    reader.readAsDataURL(f);
  };

  // Subir a POST /api/Imagenes/upload
  const handleUpload = async () => {
    if (!file) {
      setError('Selecciona un archivo primero.');
      return;
    }
    setLoading(true);
    setError('');
    try {
      const formData = new FormData();
      formData.append('Archivo', file);
      formData.append('Nombre', file.name);

      const result = await imagenService.upload(formData);
      // result: { idImagen, nombreArchivo, datosImagenBase64 }
      setUploadedImage(result);
      // si quieres usar la imagen del servidor en lugar de la local:
      setOriginalPreview(`data:image/png;base64,${result.datosImagenBase64}`);
    } catch (e) {
      console.error(e);
      setError('Error al subir la imagen: ' + e.message);
    } finally {
      setLoading(false);
    }
  };

  // Procesar en POST /api/ImagenesProcesadas/procesar/{idImagen}
  const handleProcess = async () => {
    if (!uploadedImage) {
      setError('Primero sube una imagen.');
      return;
    }
    setLoading(true);
    setError('');
    try {
      const proc = await processedImageService.process(uploadedImage.idImagen);
      // proc: { idImagenProcesada, idImagenOriginal, datosProcesadosBase64, algoritmo, ... }
      setProcessedImage(proc);
    } catch (e) {
      console.error(e);
      setError('Error al procesar la imagen: ' + e.message);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="image-processor-container">
      <h2>Digitalizador de Imágenes</h2>

      {/* Input para seleccionar archivo */}
      <div className="image-drop-area">
        <input
          type="file"
          accept="image/*"
          onChange={handleFileChange}
          style={{
            position: 'absolute',
            width: '100%',
            height: '100%',
            opacity: 0,
            cursor: 'pointer',
            zIndex: 2,
          }}
        />
        {!originalPreview && (
          <span>Arrastra o haz clic para seleccionar una imagen</span>
        )}
        {originalPreview && (
          <img
            src={originalPreview}
            alt="Original preview"
            style={{ maxWidth: '100%', maxHeight: '100%', objectFit: 'contain' }}
          />
        )}
      </div>

      {/* Botones de acción */}
      <div className="buttons-row" style={{ marginTop: '1rem' }}>
        <button
          className="btn-procesar-imagen"
          onClick={handleUpload}
          disabled={loading || !file}
        >
          {loading && !uploadedImage ? 'Subiendo…' : 'Subir Imagen'}
        </button>

        <button
          className="btn-procesar-imagen"
          onClick={handleProcess}
          disabled={loading || !uploadedImage}
        >
          {loading && uploadedImage && !processedImage
            ? 'Procesando…'
            : 'Procesar Imagen'}
        </button>

        <button
          className="btn-comparar-imagen"
          onClick={() => setIsCompareOpen(true)}
          disabled={!processedImage}
        >
          Comparar Imágenes
        </button>
      </div>

      {/* Vista previa de la imagen procesada */}
      {processedImage && (
        <div className="processed-preview" style={{ marginTop: '1.5rem' }}>
          <h3>Imagen Digitalizada</h3>
          <img
            src={`data:image/${
              processedImage.algoritmo.toLowerCase() === 'png' ? 'png' : 'jpeg'
            };base64,${processedImage.datosProcesadosBase64}`}
            alt="Processed preview"
            style={{ maxWidth: '100%', height: 'auto' }}
          />
        </div>
      )}

      {/* Mensaje de error */}
      {error && (
        <p className="compare-modal-error" style={{ marginTop: '1rem' }}>
          {error}
        </p>
      )}

      {/* Modal de comparación */}
      <CompareModal
        isOpen={isCompareOpen}
        onClose={() => setIsCompareOpen(false)}
      />
    </div>
  );
}
