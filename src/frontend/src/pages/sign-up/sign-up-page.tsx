import { useEffect } from 'react'
import { useUnit } from 'effector-react';
import { TextInput, Button, PasswordInput, Group, Anchor, Text, SegmentedControl } from '@mantine/core';
import { signUpForm, signUpFx } from './model';
import { useForm } from '@effector-reform/react';
import { AuthForm } from '../../shared/ui/auth-form';
import { Link } from '@argon-router/react';
import { routes } from '@shared/router';

export const SignUpPage = () => {
  const { fields, onSubmit, errors } = useForm(signUpForm);
  const { pending } = useUnit({ pending: signUpFx.pending });
  
  useEffect(() => { 
    fields.email.onChange("test@gmail.com")
    fields.password.onChange("123qwe")
    fields.username.onChange("test")
  },[])

  return (
    <AuthForm
      title="Создайте аккаунт"
      description="Заполните форму, чтобы зарегистрироваться"
      onSubmit={(e) => onSubmit(e as React.FormEvent<HTMLFormElement>)}
      footer={
        <Group justify="space-between" mt="md">
          <Text size="sm">
            Уже есть аккаунт?{' '}
            <Anchor fw={500}>
              <Link to={routes.signIn}>Войти</Link>
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
      
      <TextInput
        label="Полное имя"
        placeholder="Иванов Иван Иванович"
        required
        value={fields.username.value}
        onChange={(e) => fields.username.onChange(e.currentTarget.value)}
      />

      <PasswordInput
        label="Пароль"
        placeholder="Ваш пароль"
        required
        value={fields.password.value}
        onChange={(e) => fields.password.onChange(e.currentTarget.value)}
        error={errors.password}
      />

      <SegmentedControl
        data={[
          { value: '2', label: 'Я студент' },
          { value: '3', label: 'Я преподаватель' }
        ]}
        value={fields.role.value.toString()}
        onChange={(value) => fields.role.onChange(Number(value))}
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
        Зарегистрироваться
      </Button>
    </AuthForm>
  );
};
