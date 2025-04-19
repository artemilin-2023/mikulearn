import { makeAutoObservable } from "mobx";
import { createContext, useContext } from "react";

type User = {
    id: string;
    email: string;
    name: string;
    role: string;
};

class Store {
    user: User | null = null;
	isAuth: boolean = false;
    token: string | null = null;
    
    constructor() {
		makeAutoObservable(this)
    }

    setUser(user: User) {
        this.user = user;
    }

	setIsAuth(isAuth: boolean) {
		this.isAuth = isAuth;
	}

	setToken(token: string) {
		this.token = token;
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
}

export const store = new Store();

export const StoreContext = createContext(store);

export const useStore = () => useContext(StoreContext);

// usage:
// import { useStore } from "shared/store/store"
// const store = useStore()