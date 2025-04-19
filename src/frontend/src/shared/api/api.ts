import axios from 'axios';
import { store } from '@shared/store/store';

export const api = axios.create({
  baseURL: `${window.location.origin}/api`,
});

// Initialize token from localStorage on startup
const token = localStorage.getItem('token');
if (token) {
  store.setToken(token);
}

api.interceptors.request.use((config) => {
  const token = store.token || localStorage.getItem('token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

api.interceptors.response.use(
  (response) => {
    const authHeader = response.headers?.authorization || response.headers?.Authorization;
    
    if (authHeader && typeof authHeader === 'string' && authHeader.startsWith('Bearer ')) {
      const token = authHeader.substring(7);
      
      store.setToken(token);
      localStorage.setItem('token', token);
    }
    
    return response;
  },
  (error) => {
    // Handle 401 Unauthorized errors (token expired)
    if (error.response && error.response.status === 401) {
      // Clear authentication
      store.clearAuth();
    }
    return Promise.reject(error);
  }
);