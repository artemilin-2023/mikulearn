import { api } from "@shared/api/api";
import { AxiosResponse } from "axios";


export default class TestService {
    
    static async createTest(
        file: File,
        name: string,
        description: string,
    ): Promise<AxiosResponse<null>> {
        const formData = new FormData();
        formData.append('file', file);
        formData.append('name', name);
        formData.append('description', description);
        
        return api.post<null>("/test/generate", formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    }
}
