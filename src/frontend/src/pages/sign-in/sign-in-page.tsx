import { TextInput, Button, PasswordInput, Group, Anchor, Text } from '@mantine/core';
import { AuthForm } from '@shared/ui/auth-form';
import { Link } from 'react-router-dom';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';
import { api } from '@shared/api/api';
import { store } from '@shared/store/store';
import { useState } from 'react';

const signInSchema = z.object({
  email: z.string().email({ message: 'Введите корректный email' }),
  password: z.string().min(1, { message: 'Введите пароль' })
});

type SignInFormData = z.infer<typeof signInSchema>;

export const SignInPage = () => {
  const [loading, setLoading] = useState(false);
  const { control, handleSubmit, formState: { errors } } = useForm<SignInFormData>({
    resolver: zodResolver(signInSchema),
    defaultValues: {
      email: '',
      password: ''
    }
  });

  const onSubmit = async (data: SignInFormData) => {
    try {
      setLoading(true);
      console.log('Email:', data.email);
      console.log('Password:', data.password);

      const response = await api.post('/api/account/login', { email: data.email, password: data.password });
      console.log('Response:', response);
      
      if (response.data && response.data.id) {
        store.setUser({
          id: response.data.id,
          email: response.data.email || data.email,
          name: response.data.name || '',
          role: response.data.role || '',
        });
        store.setIsAuth(true);
        store.setToken(response.data.token);
      }
    } catch (error) {
      console.error('Login error:', error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <AuthForm
      title="Вход в аккаунт"
      description="Введите данные для входа"
      onSubmit={handleSubmit(onSubmit)}
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
      <Controller
        name="email"
        control={control}
        render={({ field }) => (
          <TextInput
            label="Email"
            placeholder="your@email.com"
            required
            type="email"
            error={errors.email?.message}
            {...field}
          />
        )}
      />

      <Controller
        name="password"
        control={control}
        render={({ field }) => (
          <PasswordInput
            label="Пароль"
            placeholder="Ваш пароль"
            required
            error={errors.password?.message}
            {...field}
          />
        )}
      />

      <Button 
        fullWidth 
        color=' var(--gradient-primary-secondary-light)' 
        mt="xs" 
        type="submit" 
        loading={loading}
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
