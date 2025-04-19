import { useUnit } from 'effector-react';
import { TextInput, Button, PasswordInput, Group, Anchor, Text } from '@mantine/core';
import { loginForm, loginFx } from './model';
import { useForm } from '@effector-reform/react';
import { AuthForm } from '../../shared/ui/auth-form';
import { Link } from 'react-router-dom';

export const SignInPage = () => {
  const { fields, onSubmit, errors } = useForm(loginForm);
  const { pending } = useUnit({ pending: loginFx.pending });

  return (
    <AuthForm
      title="Вход в аккаунт"
      description="Введите данные для входа"
      onSubmit={(e) => onSubmit(e as React.FormEvent<HTMLFormElement>)}
      footer={
        <Group justify="space-between" mt="md">
          <Text size="sm">
            Нет аккаунта?{' '}
            <Anchor fw={500}>
              <Link to="/sign-up">Зарегистрироваться</Link>
            </Anchor>
          </Text>
        </Group>
      }
    >
      <TextInput
        label="Email"
        placeholder="your@email.com"
        required
        type="email"
        value={fields.email.value}
        onChange={(e) => fields.email.onChange(e.currentTarget.value)}
        error={errors.email}
      />

      <PasswordInput
        label="Пароль"
        placeholder="Ваш пароль"
        required
        value={fields.password.value}
        onChange={(e) => fields.password.onChange(e.currentTarget.value)}
        error={errors.password}
      />

      <Button 
        fullWidth 
        color=' var(--gradient-primary-secondary-light)' 
        mt="xs" 
        type="submit" 
        loading={pending}
        styles={{ 
          label: { 
            color: 'black' 
          } 
        }}
      >
        Войти
      </Button>
    </AuthForm>
  );
};
