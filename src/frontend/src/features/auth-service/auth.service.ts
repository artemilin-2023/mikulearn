type LoginParams = {
	email: string;
	password: string;
};

import { createEvent, createStore, createEffect, sample } from 'effector';

export const emailChanged = createEvent<string>();
export const passwordChanged = createEvent<string>();

export const formSubmitted = createEvent();

export const $email = createStore('').on(emailChanged, (_, v) => v);
export const $password = createStore('').on(passwordChanged, (_, v) => v);

export const loginFx = createEffect<LoginParams, { success: true }>(
	async ({ email, password }) => {
		await new Promise((r) => setTimeout(r, 1000));
		console.log('Email:', email);
		console.log('Password:', password);
		return { success: true };
	}
);

sample({
	clock: formSubmitted,
	source: {
		email: $email,
		password: $password,
	},
	target: loginFx,
});
