import {
  Box,
  Paper,
  Title,
  Text,
  Stack,
  Container,
} from '@mantine/core';
import { ReactNode } from 'react';

import classes from './auth-form.module.css';

export interface AuthFormProps {
  title: string;
  description?: string;
  onSubmit: (e: React.FormEvent) => void;
  children: ReactNode;
  footer?: ReactNode;
}

export const AuthForm = ({
  title,
  description,
  onSubmit,
  children,
  footer,
}: AuthFormProps) => {
  return (
    <Container size={420} my={40}>
      <Title ta="center" className={classes.title}>
        {title}
      </Title>
      {description && (
        <Text c="dimmed" size="sm" ta="center" mt={5}>
          {description}
        </Text>
      )}

      <Paper withBorder shadow="md" p={30} mt={30} radius="md">
        <form onSubmit={onSubmit}>
          <Stack>
            {children}
          </Stack>
        </form>
        {footer && <Box mt="md">{footer}</Box>}
      </Paper>
    </Container>
  );
}; 
