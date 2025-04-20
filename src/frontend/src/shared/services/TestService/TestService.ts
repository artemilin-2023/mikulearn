import { api } from "@shared/api/api";
import { AxiosResponse } from "axios";


export default class TestService {
    
    static async createTest(
        file: File,
        name: string,
        description: string,
    ): Promise<AxiosResponse<null>> {
        return api.post<null>("/generate", { file, name, description });
    }
}
