import { Button, TextInput } from '@mantine/core';
import { useUnit } from 'effector-react';
import { $counter } from '../sign-in/model/store';

export const SignUpPage = () => {
	const counter = useUnit($counter);

	return (
		<div>
			<TextInput label="Email" placeholder="your@email.com" />
			{counter}
			<Button color="blue" mt="md">
				Отправить
			</Button>
		</div>
	);
};
