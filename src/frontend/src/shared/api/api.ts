import axios from 'axios';
import { store } from '@shared/store/store';

export const api = axios.create({
  baseURL: window.location.origin,
});


api.interceptors.request.use((config) => {
  if (store.token) {
    config.headers.Authorization = `Bearer ${store.token}`;
  }
  return config;
});

api.interceptors.response.use((response) => {
  const authHeader = response.headers?.authorization || response.headers?.Authorization;
  
  if (authHeader && typeof authHeader === 'string' && authHeader.startsWith('Bearer ')) {
    const token = authHeader.substring(7);
    
    store.setToken(token);
  }
  
  return response;
});