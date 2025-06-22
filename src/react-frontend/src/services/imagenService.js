import apiClient from './apiClient';

const ENDPOINT = '/api/Imagenes';

export default {
  // Trae todas las imágenes
  list: () =>
    apiClient
      .get(ENDPOINT)
      .then(res => res.data),

  // Sube la imagen. Recibe un FormData ya construido en el componente
  upload: formData =>
    apiClient
      .post(`${ENDPOINT}/upload`, formData)
      .then(res => res.data),

  create: payload =>
    apiClient
      .post(ENDPOINT, payload)
      .then(res => res.data),

  update: (id, payload) =>
    apiClient
      .put(`${ENDPOINT}/${id}`, payload)
      .then(res => res.data),

  remove: id =>
    apiClient
      .delete(`${ENDPOINT}/${id}`)
      .then(res => res.data),
};
