import { createEffect, sample } from 'effector';
import { createForm } from '@effector-reform/core';
import { zodAdapter } from '@effector-reform/zod';
import { z } from 'zod';
import { api } from '@shared/api/api';
import { login } from '@shared/user';

type SignInValues = {
  email: string;
  password: string;
};

export const loginFx = createEffect<SignInValues, { success: true }>(
  async ({ email, password }) => {
    console.log('Email:', email);
    console.log('Password:', password);

    const response = await api.post('/api/account/login', { email, password });
    console.log('Response:', response);
    
    if (response.data && response.data.id) {
      login({
        id: response.data.id,
        email: response.data.email || email,
        name: response.data.name || '',
        role: response.data.role || '',
      });
    }
    
    return { success: true };
  }
);

export const loginForm = createForm<SignInValues>({
  schema: {
    email: '',
    password: '',
  },
  validation: zodAdapter(
    z.object({
      email: z.string().email(),
      password: z.string()
    })
  )
});

sample({
  clock: loginForm.validatedAndSubmitted,
  target: loginFx,
});
