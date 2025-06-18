import apiClient from './apiClient';

const list = () =>
  apiClient.get('/api/Imagenes').then(res => res.data);

const upload = file => {
  const form = new FormData();
  form.append('file', file);
  return apiClient
    .post('/api/Imagenes/upload', form, { headers: { 'Content-Type': 'multipart/form-data' } })
    .then(res => res.data);
};

const create = payload =>
  apiClient.post('/api/Imagenes', payload).then(res => res.data);

const update = (id, payload) =>
  apiClient.put(`/api/Imagenes/${id}`, payload).then(res => res.data);

const remove = id =>
  apiClient.delete(`/api/Imagenes/${id}`).then(res => res.data);

export default { list, upload, create, update, remove };
