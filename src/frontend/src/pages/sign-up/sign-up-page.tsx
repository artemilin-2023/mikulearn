import { zodResolver } from '@hookform/resolvers/zod';
import {
  TextInput,
  Button,
  PasswordInput,
  Group,
  Anchor,
  Text,
  SegmentedControl,
} from '@mantine/core';
import { useState } from 'react';
import { useForm, Controller } from 'react-hook-form';
import { Link, useNavigate } from 'react-router-dom';
import { z } from 'zod';

import { api } from '@shared/api/api';
import { store } from '@shared/store/store';
import { AuthForm } from '@shared/ui/auth-form';


const signUpSchema = z.object({
  email: z.string().email({ message: 'Введите корректный email' }),
  username: z.string().min(2, { message: 'Имя должно содержать минимум 2 символа' }),
  password: z.string().min(6, { message: 'Пароль должен содержать минимум 6 символов' }),
  role: z.number().min(1, { message: 'Выберите роль' }),
});

type SignUpFormData = z.infer<typeof signUpSchema>;

export const SignUpPage = () => {
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const {
    control, handleSubmit, formState: { errors }, setError, 
  } = useForm<SignUpFormData>({
    resolver: zodResolver(signUpSchema),
    defaultValues: {
      email: '',
      username: '',
      password: '',
      role: 2,
    },
  });

  const onSubmit = async (data: SignUpFormData) => {
    try {
      setLoading(true);
      
      const response = await api.post('/account/register', data);
      
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
              roles: userResponse.data.roles || [],
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
      console.error('Registration error:', error);
      
      // Handle registration errors
      if (error && typeof error === 'object' && 'response' in error) {
        const errorObj = error as { response?: { status?: number; data?: unknown } };
        
        if (errorObj.response?.status === 400) {
          setError('email', { 
            message: 'Пользователь с таким email уже существует', 
          });
        } else {
          setError('email', { 
            message: 'Произошла ошибка при регистрации', 
          });
        }
      }
    } finally {
      setLoading(false);
    }
  };  

  return (
    <AuthForm
      title="Регистрация"
      description="Создайте новый аккаунт"
      onSubmit={handleSubmit(onSubmit)}
      footer={
        <Group justify="space-between" mt="md">
          <Text size="sm">
            Уже есть аккаунт?{' '}
            <Anchor fw={500}>
              <Link to="/sign-in">Войти</Link>
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
        name="username"
        control={control}
        render={({ field }) => (
          <TextInput
            label="Полное имя"
            placeholder="Иванов Иван Иванович"
            required
            error={errors.username?.message}
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

      <Controller
        name="role"
        control={control}
        render={({ field }) => (
          <SegmentedControl
            data={[
              { value: '2', label: 'Я студент' },
              { value: '3', label: 'Я преподаватель' },
            ]}
            value={field.value.toString()}
            onChange={(value) => field.onChange(parseInt(value))}
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
            color: 'black', 
          }, 
        }}
      >
        Зарегистрироваться
      </Button>
    </AuthForm>
  );
};
