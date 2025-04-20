import { AxiosResponse } from 'axios';

import { api } from '@shared/api/api';


export default class TestService {
    
  static async createTest(
    file: File,
    name: string,
    description: string,
  ): Promise<AxiosResponse<string>> {
    const formData = new FormData();
    formData.append('file', file);
    formData.append('name', name);
    formData.append('description', description);
        
    return api.post<string>('/test/generate', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    });
  }
}
