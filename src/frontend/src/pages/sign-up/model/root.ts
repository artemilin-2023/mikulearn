import { createEffect, sample } from 'effector';
import { zodAdapter } from '@effector-reform/zod';
import { createForm } from '@effector-reform/core';
import { z } from 'zod';
import { api } from '@shared/api/api';

type SignUpValues = {
  email: string;
  username: string;
  password: string;
  role: number; 
};

export const signUpFx = createEffect<SignUpValues, { success: true }>(
  async ({ email, username, password, role }) => {
    console.log('Email:', email);
    console.log('Username:', username);
    console.log('Password:', password);
    console.log('Role:', role);
    const response = await api.post('/api/account/register', { email, username, password, role });
    console.log('Response:', response);
    return { success: true };
  }
);

export const signUpForm = createForm<SignUpValues>({
  schema: {
    email: '',
    username: '',
    password: '',
    role: 2,
  },
  validation: zodAdapter(
    z.object({
      email: z.string().email(),
      username: z.string(),
      password: z.string(),
      role: z.number(),
    })
  ),
});

sample({
  clock: signUpForm.validatedAndSubmitted,
  target: signUpFx,
});
