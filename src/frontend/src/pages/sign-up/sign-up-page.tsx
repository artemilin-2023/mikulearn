import { Button, TextInput } from '@mantine/core';

export const SignUpPage = () => {
	return (
		<div>
			<TextInput label="Email" placeholder="your@email.com" />
			<Button color="blue" mt="md">
				Отправить
			</Button>
		</div>
	);
};
