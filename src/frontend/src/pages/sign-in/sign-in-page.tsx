import { useUnit } from 'effector-react';
import { TextInput, Button, PasswordInput } from '@mantine/core';
import { loginForm, loginFx } from './model';
import { useForm } from '@effector-reform/react';

export const SignInPage = () => {
  const { fields, onSubmit, errors } = useForm(loginForm);
  const { pending } = useUnit({ pending: loginFx.pending });

  return (
    <div>
      <form onSubmit={onSubmit}>
        <TextInput
          label="Email"
          placeholder="your@email.com"
          type="email"
          value={fields.email.value}
          onChange={(e) => fields.email.onChange(e.currentTarget.value)}
        />

        <PasswordInput
          label="Password"
          placeholder="password"
          type="password"
          value={fields.password.value}
          onChange={(e) => fields.password.onChange(e.currentTarget.value)}
        />

        {errors.email} 
        {errors.password} 
        
        <Button color="blue" mt="md" type="submit" loading={pending}>
          Войти
        </Button>
      </form>
    </div>
  );
};
