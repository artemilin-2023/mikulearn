import { TextInput, Button, PasswordInput, Group, Anchor, Text, SegmentedControl } from '@mantine/core';
import { AuthForm } from '@shared/ui/auth-form';
import { Link } from 'react-router-dom';
import { useForm, Controller } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { z } from 'zod';

const signUpSchema = z.object({
  email: z.string().email({ message: 'Введите корректный email' }),
  username: z.string().min(2, { message: 'Имя должно содержать минимум 2 символа' }),
  password: z.string().min(6, { message: 'Пароль должен содержать минимум 6 символов' }),
  role: z.number().min(1, { message: 'Выберите роль' })
});

type SignUpFormData = z.infer<typeof signUpSchema>;

export const SignUpPage = () => {
  const { control, handleSubmit, formState: { errors } } = useForm<SignUpFormData>({
    resolver: zodResolver(signUpSchema),
    defaultValues: {
      email: '',
      username: '',
      password: '',
      role: 2
    }
  });

  const onSubmit = (data: SignUpFormData) => {
    console.log(data);
  };

  return (
    <AuthForm
      title="Регистрация"
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
              { value: '3', label: 'Я преподаватель' }
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
        styles={{ 
          label: { 
            color: 'black' 
          } 
        }}
      >
        Зарегистрироваться
      </Button>
    </AuthForm>
  );
};
