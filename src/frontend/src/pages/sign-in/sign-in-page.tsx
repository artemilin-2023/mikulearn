import { useUnit } from 'effector-react';
import { TextInput, Button } from '@mantine/core';

import {
	$email,
	$password,
	emailChanged,
	passwordChanged,
	formSubmitted,
	loginFx,
} from '@features/auth-service/auth.service';

export const SignInPage = () => {
	const [email, password, onEmailChange, onPasswordChange, onSubmit, pending] =
		useUnit([
			$email,
			$password,
			emailChanged,
			passwordChanged,
			formSubmitted,
			loginFx.pending,
		]);

	return (
		<div>
			<TextInput
				label="Email"
				placeholder="your@email.com"
				value={email}
				onChange={(e) => onEmailChange(e.currentTarget.value)}
			/>
			<TextInput
				label="Password"
				placeholder="password"
				type="password"
				value={password}
				onChange={(e) => onPasswordChange(e.currentTarget.value)}
			/>

			<Button color="blue" mt="md" onClick={onSubmit} loading={pending}>
				Войти
			</Button>
		</div>
	);
};
