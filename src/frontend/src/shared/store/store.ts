import { makeAutoObservable } from "mobx";
import { createContext, useContext } from "react";

class Store {
    something: string = 'something';

	constructor() {
		makeAutoObservable(this);
    }

	setSomething(something: string) {
		this.something = something;
	}
	
}

export const store = new Store();

export const StoreContext = createContext(store);

export const useStore = () => useContext(StoreContext);

// usage:
// import { useStore } from "shared/store/store"
// const store = useStore()