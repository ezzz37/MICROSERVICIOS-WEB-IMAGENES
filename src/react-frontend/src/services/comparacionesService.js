import apiClient from './apiClient';

const list = () =>
  apiClient.get('/api/Comparaciones').then(res => res.data);

const get = id =>
  apiClient.get(`/api/Comparaciones/${id}`).then(res => res.data);

const byImages = (id1, id2) =>
  apiClient
    .get('/api/Comparaciones/porImagenes', { params: { id1, id2 } })
    .then(res => res.data);

const compare = (id1, id2) =>
  apiClient
    .post('/api/Comparaciones/comparar', {
      IdImagenOriginal:  id1,
      IdImagenProcesada: id2
    })
    .then(res => res.data);

export default { list, get, byImages, compare };
