import axios from 'axios';
import { $token } from '@shared/user';

export const api = axios.create({
  baseURL: 'https://localhost',
});

let currentToken: string | null = null;

$token.watch((token) => {
  currentToken = token;
});

api.interceptors.request.use((config) => {
  if (currentToken) {
    config.headers.Authorization = `Bearer ${currentToken}`;
  }
  return config;
});

api.interceptors.response.use((response) => {
  const authHeader = response.headers?.authorization || response.headers?.Authorization;
  
  if (authHeader && typeof authHeader === 'string' && authHeader.startsWith('Bearer ')) {
    const token = authHeader.substring(7);
    
    import('@shared/user').then(({ setToken }) => {
      setToken(token);
    });
  }
  
  return response;
});