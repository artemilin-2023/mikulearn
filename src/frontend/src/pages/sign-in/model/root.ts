import { createEffect, sample } from 'effector';
import { createForm } from '@effector-reform/core';

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
});

sample({
  clock: loginForm.validatedAndSubmitted,
  target: loginFx,
});
