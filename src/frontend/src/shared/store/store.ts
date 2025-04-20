import { makeAutoObservable } from 'mobx';
import { createContext, useContext } from 'react';

import { api } from '@shared/api/api';
import { User } from '@shared/services/AuthService/AuthService';


class Store {
  user: User | null = null;
  isAuth: boolean = false;
  token: string | null = null;
  initialized: boolean = false;

  constructor() {
    makeAutoObservable(this);
    this.init();
  }

  async init() {
    this.loadTokenFromStorage();
    if (this.token) {
      try {
        const response = await api.get('/account/me');
        if (response.data) {
          this.setUser(response.data);
          this.setIsAuth(true);
        }
      } catch (error) {
        console.error('Failed to fetch user data on init', error);
        // Token might be invalid or expired
        this.clearAuth();
      }
    }
    this.initialized = true;
  }

  setUser(user: User) {
    this.user = user;
  }

  setIsAuth(isAuth: boolean) {
    this.isAuth = isAuth;
  }

  setToken(token: string) {
    this.token = token;
    this.saveTokenToStorage();
  }

  loadTokenFromStorage() {
    const token = localStorage.getItem('token');
    if (token) {
      this.setToken(token);
    }
  }

  saveTokenToStorage() {
    if (this.token) {
      localStorage.setItem('token', this.token);
    }
  }

  clearAuth() {
    this.user = null;
    this.isAuth = false;
    this.token = null;
    localStorage.removeItem('token');
  }
}

export const store = new Store();

export const StoreContext = createContext(store);

export const useStore = () => useContext(StoreContext);

// usage:
// import { useStore } from "shared/store/store"
// const store = useStore()
