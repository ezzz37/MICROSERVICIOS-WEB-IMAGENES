import apiClient from './apiClient';

const list = () =>
  apiClient.get('/api/ImagenesProcesadas').then(res => res.data);

const get = id =>
  apiClient.get(`/api/ImagenesProcesadas/${id}`).then(res => res.data);

const update = (id, payload) =>
  apiClient.put(`/api/ImagenesProcesadas/${id}`, payload).then(res => res.data);

const remove = id =>
  apiClient.delete(`/api/ImagenesProcesadas/${id}`).then(res => res.data);

// dispara el procesamiento en background
const process = idImagen =>
  apiClient.post(`/api/ImagenesProcesadas/procesar/${idImagen}`).then(res => res.data);

export default { list, get, update, remove, process };
