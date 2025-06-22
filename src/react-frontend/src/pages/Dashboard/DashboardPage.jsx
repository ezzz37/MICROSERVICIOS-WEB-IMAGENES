// src/pages/Dashboard/DashboardPage.jsx

import React, { useState } from 'react'
import ReactDOM from 'react-dom'
import ImageGallery          from '../../components/ImageGallery/ImageGallery'
import CompareModal          from '../../components/Comparacion/CompareModal'
import imagenService         from '../../services/imagenService'
import processedImageService from '../../services/processedImageService'
import './Dashboard.css'

export default function DashboardPage({ onLogout }) {
  const [resolution, setResolution]               = useState(500)
  const [colorDepthIndex, setColorDepthIndex]     = useState(1)
  const [colorDepth, setColorDepth]               = useState(8)
  const [compression, setCompression]             = useState(0.8)
  const [selectedFile, setSelectedFile]           = useState(null)
  const [originalPreview, setOriginalPreview]     = useState(null)
  const [uploadedImage, setUploadedImage]         = useState(null)
  const [processedImage, setProcessedImage]       = useState(null)
  const [processedPreview, setProcessedPreview]   = useState(null)
  const [loading, setLoading]                     = useState(false)
  const [error, setError]                         = useState(null)

  const [showGallery, setShowGallery]             = useState(false)
  const [isCompareOpen, setIsCompareOpen]         = useState(false)

  const DEPTHS = [1, 8, 24]

  // Seleccionar y previsualizar archivo en local
  const handleFileChange = e => {
    const file = e.target.files[0]
    if (!file) return
    setSelectedFile(file)
    setUploadedImage(null)
    setProcessedImage(null)
    setProcessedPreview(null)
    setError(null)

    const reader = new FileReader()
    reader.onloadend = () => setOriginalPreview(reader.result)
    reader.readAsDataURL(file)
  }

  // Subir imagen al backend
  const handleUpload = async () => {
    if (!selectedFile) {
      setError('Por favor, selecciona un archivo primero.')
      return
    }
    setLoading(true)
    setError(null)

    try {
      const formData = new FormData()
      formData.append('Archivo', selectedFile)
      formData.append('Nombre' , selectedFile.name)

      const result = await imagenService.upload(formData)
      setUploadedImage(result)
      setOriginalPreview(`data:image/png;base64,${result.datosImagenBase64}`)
    } catch (e) {
      setError('Error al subir la imagen: ' + e.message)
    } finally {
      setLoading(false)
    }
  }

  // Procesar imagen subida
  const handleProcess = async () => {
    if (!uploadedImage) {
      setError('Primero sube una imagen.')
      return
    }
    setLoading(true)
    setError(null)

    try {
      const payload = {
        AnchoResolucion:       resolution,
        AltoResolucion:        resolution,
        ProfundidadBits:       colorDepth,
        IdAlgoritmoCompresion: 1,
        Algoritmo:             'JPEG',
        NivelCompresion:       Math.round(compression * 100),
      }

      const proc = await processedImageService.process(
        uploadedImage.idImagen,
        payload
      )
      setProcessedImage(proc)
      const mime = proc.algoritmo.toLowerCase() === 'png' ? 'png' : 'jpeg'
      setProcessedPreview(`data:image/${mime};base64,${proc.datosProcesadosBase64}`)
    } catch (e) {
      setError('Error al procesar la imagen: ' + e.message)
      setProcessedImage(null)
      setProcessedPreview(null)
    } finally {
      setLoading(false)
    }
  }

  // Descargar imagen procesada
  const handleDownload = () => {
    if (!processedPreview) return
    const link = document.createElement('a')
    link.href = processedPreview
    link.download = 'imagen-digitalizada.' + (processedImage?.algoritmo.toLowerCase() === 'png' ? 'png' : 'jpg')
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
  }

  // Reiniciar todo el formulario
  const handleReset = () => {
    setResolution(500)
    setColorDepthIndex(1)
    setColorDepth(8)
    setCompression(0.8)
    setSelectedFile(null)
    setOriginalPreview(null)
    setUploadedImage(null)
    setProcessedImage(null)
    setProcessedPreview(null)
    setError(null)
  }

  // Galería
  const toggleGallery = () => setShowGallery(v => !v)
  const handleImageSelect = image => {
    setShowGallery(false)
    setUploadedImage(image)
    setSelectedFile(null)
    setProcessedImage(null)
    setProcessedPreview(null)
    setOriginalPreview(`data:image/png;base64,${image.datosImagenBase64}`)
    if (image.idImagenProcesada) {
      processedImageService.getById(image.idImagenProcesada)
        .then(proc => {
          setProcessedImage(proc)
          const mime = proc.algoritmo.toLowerCase() === 'png' ? 'png' : 'jpeg'
          setProcessedPreview(`data:image/${mime};base64,${proc.datosProcesadosBase64}`)
        })
        .catch(err => setError(err.message))
    }
  }

  // Sliders de profundidad
  const handleColorDepthChange = e => {
    const idx = Number(e.target.value)
    setColorDepthIndex(idx)
    setColorDepth(DEPTHS[idx])
  }
  const setDepthPreset = bits => {
    const idx = DEPTHS.indexOf(bits)
    if (idx > -1) {
      setColorDepthIndex(idx)
      setColorDepth(bits)
    }
  }

  // Comparar imágenes
  const openCompare = () => setIsCompareOpen(true)
  const closeCompare = () => setIsCompareOpen(false)

  return (
    <>
      <div className="App">
        <header className="app-header">
          <div>
            <h1>Digitalizador de Imágenes</h1>
            <p>Convierte imágenes analógicas a digital con tus parámetros</p>
          </div>
          <button className="btn-logout" onClick={onLogout}>
            Cerrar Sesión
          </button>
        </header>

        <section className="image-panels">
          <div className="panel">
            <h2>Imagen Original</h2>
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
                }}
              />
              {!selectedFile && !originalPreview && (
                <span>Arrastra o haz clic para seleccionar</span>
              )}
              {originalPreview && (
                <img
                  src={originalPreview}
                  alt="Original preview"
                  style={{
                    maxWidth: '100%',
                    maxHeight: '100%',
                    objectFit: 'contain',
                  }}
                />
              )}
            </div>
            <div className="buttons-row">
              <button
                className="btn-primary"
                onClick={handleUpload}
                disabled={loading || !selectedFile}
              >
                {loading && !uploadedImage ? 'Subiendo…' : 'Subir Imagen'}
              </button>
              <button className="btn-secondary" onClick={toggleGallery}>
                Ver Imágenes Cargadas
              </button>
            </div>
          </div>

          <div className="panel">
            <h2>Imagen Digitalizada</h2>
            <div className="image-drop-area digitalized">
              {loading && <p>Cargando…</p>}
              {!loading && processedPreview && (
                <img
                  src={processedPreview}
                  alt="Processed preview"
                  style={{
                    maxWidth: '100%',
                    maxHeight: '100%',
                    objectFit: 'contain',
                  }}
                />
              )}
              {!loading && !processedPreview && (
                <p>No hay imagen procesada</p>
              )}
            </div>
            <div className="buttons-row">
              <button
                onClick={handleDownload}
                disabled={!processedPreview || loading}
              >
                Descargar Imagen
              </button>
              <button onClick={handleReset}>Reiniciar</button>
            </div>
          </div>
        </section>

        <section className="params">
          <h2>Parámetros de Digitalización</h2>

          {/* Layout en dos columnas */}
          <div className="param-columns">
            <div className="param-group">
              <label>Muestreo (Resolución):</label>
              <span className="value">{resolution}×{resolution}</span>
              <input
                type="range"
                min="50"
                max="1000"
                value={resolution}
                onChange={e => setResolution(Number(e.target.value))}
              />
              <div className="presets">
                {[100, 500, 1000].map(r => (
                  <button key={r} onClick={() => setResolution(r)}>
                    {r}×{r}
                  </button>
                ))}
              </div>
            </div>

            <div className="divider" />

            <div className="param-group">
              <label>Profundidad de Color:</label>
              <span className="value">{colorDepth} bits</span>
              <input
                type="range"
                min="0"
                max="2"
                step="1"
                value={colorDepthIndex}
                onChange={handleColorDepthChange}
              />
              <div className="presets">
                <button onClick={() => setDepthPreset(1)}>1 bit</button>
                <button onClick={() => setDepthPreset(8)}>8 bits</button>
                <button onClick={() => setDepthPreset(24)}>24 bits</button>
              </div>
            </div>
          </div>

          <div className="param-group">
            <label>Compresión:</label>
            <span className="value">{compression}</span>
            <input
              type="range"
              min="0"
              max="1"
              step="0.01"
              value={compression}
              onChange={e => setCompression(Number(e.target.value))}
            />
            <div className="labels-range">
              <span>Alta</span>
              <span>Baja</span>
            </div>
          </div>

          <div className="buttons-row">
            <button
              className="btn-primary"
              onClick={handleProcess}
              disabled={loading || !uploadedImage}
            >
              {loading && uploadedImage && !processedImage
                ? 'Procesando…'
                : 'Procesar Imagen'}
            </button>
            <button
              className="btn-secondary"
              onClick={openCompare}
              disabled={!processedImage || loading}
            >
              Comparar Imágenes
            </button>
          </div>

          {error && <p className="error-text">{error}</p>}
        </section>
      </div>

      {showGallery &&
        ReactDOM.createPortal(
          <ImageGallery
            onClose={toggleGallery}
            onSelect={handleImageSelect}
          />,
          document.body
        )}

      <CompareModal
        isOpen={isCompareOpen}
        onClose={closeCompare}
      />
    </>
  )
}
