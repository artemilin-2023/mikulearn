import { createEffect, sample } from 'effector';
import { createForm } from '@effector-reform/core';
import { zodAdapter } from '@effector-reform/zod';
import { z } from 'zod';

type SignInValues = {
  email: string;
  password: string;
};

export const loginFx = createEffect<SignInValues, { success: true }>(
  async ({ email, password }) => {
    await new Promise((r) => setTimeout(r, 1000));
    console.log('Email:', email);
    console.log('Password:', password);
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
      password: z.string().length(8)
    })
  )
});

sample({
  clock: loginForm.validatedAndSubmitted,
  target: loginFx,
});
