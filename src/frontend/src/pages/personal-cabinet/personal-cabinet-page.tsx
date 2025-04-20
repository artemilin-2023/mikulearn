import { zodResolver } from '@hookform/resolvers/zod';
import {
  Loader,
  Paper,
  Title,
  Container,
  Stack,
  Button,
  TextInput,
  Text,
} from '@mantine/core';
import { observer } from 'mobx-react-lite';
import { useState } from 'react';
import { useForm } from 'react-hook-form';
import { z } from 'zod';

import { fileInputStore } from '@features/file-input';
import { FileInput } from '@features/file-input/ui/file-input';
import TestService from '@shared/services/TestService/TestService';
import { useStore } from '@shared/store/store';
import { Sidebar } from '@widgets/Sidebar/Sidebar';

import styles from './personal-cabinet.module.css';

const testFormSchema = z.object({
  name: z.string().min(3, 'Название должно содержать минимум 3 символа'),
  description: z.string().min(10, 'Описание должно содержать минимум 10 символов'),
});

type TestFormData = z.infer<typeof testFormSchema>;

export const PersonalCabinetPage = observer(() => {
  const store = useStore();
  const [isSubmitting, setIsSubmitting] = useState(false);

  const {
    register,
    handleSubmit,
    formState: { errors },
    reset,
  } = useForm<TestFormData>({
    resolver: zodResolver(testFormSchema),
    defaultValues: {
      name: '',
      description: '',
    },
  });

  if (!store.initialized) {
    return (
      <div className={styles.loaderContainer}>
        <Loader size="xl" color="primary" />
      </div>
    );
  }

  const onSubmit = async (data: TestFormData) => {
    if (!fileInputStore.file) {
      return;
    }

    try {
      setIsSubmitting(true);
      const response = await TestService.createTest(fileInputStore.file, data.name, data.description);
      store.setTestGuid(response.data);
      reset();
    } catch (error) {
      console.error('Failed to create test:', error);
    } finally {
      setIsSubmitting(false);
    }
  };

  return (
    <div className={styles.container}>
      <Sidebar />
      <Container size="lg" className={styles.content}>
        {store.user?.roles.includes('Teacher') && (
          <Paper shadow="none" p="xl" radius="md" withBorder className={styles.contentCard}>
            <Stack>
              <Title order={2} mb="md">Создать тест </Title>
              <Title order={3} mb="sm">Загрузите файл с конспектами</Title>
              <form onSubmit={handleSubmit(onSubmit)}>
                <FileInput />
                {fileInputStore.file && (
                  <>
                    <TextInput
                      mt="md"
                      label="Название теста"
                      placeholder="Введите название теста"
                      {...register('name')}
                      error={errors.name?.message}
                    />
                    <TextInput
                      mt="md"
                      label="Описание теста"
                      placeholder="Введите описание теста"
                      {...register('description')}
                      error={errors.description?.message}
                    />
                    <Button
                      color="var(--gradient-primary-secondary-light)"
                      type="submit"
                      className={styles.createTestButton}
                      mt="md"
                      style={{ color: 'black', borderRadius: '4px' }}
                      loading={isSubmitting}
                      disabled={!fileInputStore.file || isSubmitting}
                    >
                      {isSubmitting ? 'Создание...' : 'Создать тест'}
                    </Button>
                  </>
                )}
                {!fileInputStore.file && (
                  <Text color="dimmed" mt="md">Загрузите файл, чтобы продолжить создание теста</Text>
                )}
              </form>
            </Stack>
          </Paper>
        )}

        {store.user?.roles.includes('Student') && (
          <Paper shadow="none" p="xl" radius="md" withBorder className={styles.contentCard}>
            <Stack>
              <Title order={2} mb="md">Мои тесты</Title>
            </Stack>
          </Paper>
        )}
      </Container>
    </div>
  );
});
