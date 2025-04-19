import { TextInput, Button, PasswordInput, Group, Anchor, Text } from '@mantine/core';
import { AuthForm } from '@shared/ui/auth-form';
import { Link, useNavigate } from 'react-router-dom';
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
  const navigate = useNavigate();
  const { control, handleSubmit, formState: { errors }, setError } = useForm<SignInFormData>({
    resolver: zodResolver(signInSchema),
    defaultValues: {
      email: '',
      password: ''
    }
  });

  const onSubmit = async (data: SignInFormData) => {
    try {
      setLoading(true);
      
      const response = await api.post('/account/login', { 
        email: data.email, 
        password: data.password 
      });
      
      if (response.status === 200) {
        // Save token to localStorage
        if (response.headers?.authorization) {
          const token = response.headers.authorization.substring(7);
          localStorage.setItem('token', token);
          store.setToken(token);
        }
        
        // Fetch user data
        try {
          const userResponse = await api.get('/account/me');
          if (userResponse.data) {
            store.setUser({
              id: userResponse.data.id,
              email: userResponse.data.email,
              name: userResponse.data.name || '',
              role: userResponse.data.role || '',
            });
            store.setIsAuth(true);
            
            // Redirect to personal cabinet
            navigate('/personal-cabinet');
          }
        } catch (userError) {
          console.error('Error fetching user data:', userError);
        }
      }
    } catch (error: unknown) {
      console.error('Login error:', error);
      
      // Handle login errors
      if (error && typeof error === 'object' && 'response' in error) {
        const errorObj = error as { response?: { status?: number } };
        if (errorObj.response?.status === 400 || errorObj.response?.status === 401) {
          setError('password', { 
            message: 'Неверный email или пароль' 
          });
        } else {
          setError('email', { 
            message: 'Произошла ошибка при входе' 
          });
        }
      }
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
