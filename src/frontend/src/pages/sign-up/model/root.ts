import { createEffect, sample } from 'effector';
import { zodAdapter } from '@effector-reform/zod';
import { createForm } from '@effector-reform/core';
import { z } from 'zod';

type SignUpValues = {
  email: string;
  fullName: string;
  password: string;
};

export const signUpFx = createEffect<SignUpValues, { success: true }>(
  async ({ email, password }) => {
    await new Promise((r) => setTimeout(r, 1000));
    console.log('Email:', email);
    console.log('Password:', password);
    return { success: true };
  }
);

export const signUpForm = createForm<SignUpValues>({
  schema: {
    email: '',
    fullName: '',
    password: '',
  },
  validation: zodAdapter(
    z.object({
      email: z.string().email(),
      fullName: z.string(),
      password: z.string(),
    })
  ),
});

sample({
  clock: signUpForm.validatedAndSubmitted,
  target: signUpFx,
});
