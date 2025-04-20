import { api } from "@shared/api/api";
import { AxiosResponse } from "axios";

import { store } from "@shared/store/store";

export type User = {
    id: string;
    email: string;
    name: string;
    roles: Array<string>;
}

export default class AuthService {
    
    static async login(
        email: string,
        password: string
    ): Promise<AxiosResponse<null>> {
        return api.post<null>("/account/login", { email, password });
    }

    static async register(
        email: string,
        password: string
    ): Promise<AxiosResponse<unknown>> {
        return api.post<unknown>("/account/register", { email, password });
    }

    static async logout(): Promise<AxiosResponse<unknown>> {
        store.clearAuth();
        localStorage.removeItem("token");

        return api.get<unknown>("/account/logout");
    }

    static async getMe(): Promise<AxiosResponse<User>> {
        return api.get<User>("/account/me");
    }
}