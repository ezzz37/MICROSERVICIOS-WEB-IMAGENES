import apiClient from './apiClient';

/**
 * DTO para procesar la imagen:
 * {
 *   anchoResolucion: number,
 *   altoResolucion: number,
 *   profundidadBits: number,      // 1, 8 o 24
 *   idAlgoritmoCompresion: number,
 *   nivelCompresion: number       // entre 0.0 y 1.0
 * }
 */

const list = () =>
  apiClient.get('/api/ImagenesProcesadas').then(res => res.data);

const get = id =>
  apiClient.get(`/api/ImagenesProcesadas/${id}`).then(res => res.data);

const update = (id, payload) =>
  apiClient.put(`/api/ImagenesProcesadas/${id}`, payload).then(res => res.data);

const remove = id =>
  apiClient.delete(`/api/ImagenesProcesadas/${id}`).then(res => res.data);

/**
 * Dispara el procesamiento en background enviando el DTO con todos los parámetros
 *
 * @param {number} idImagen   ID de la imagen original a procesar
 * @param {{ anchoResolucion:number, altoResolucion:number,
 *          profundidadBits:number, idAlgoritmoCompresion:number,
 *          nivelCompresion:number }} dto
 */
const process = (idImagen, dto) =>
  apiClient
    .post(`/api/ImagenesProcesadas/procesar/${idImagen}`, dto)
    .then(res => res.data);

export default {
  list,
  get,
  update,
  remove,
  process
};
