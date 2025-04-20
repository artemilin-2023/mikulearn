import styles from './personal-cabinet.module.css';
import { Sidebar } from '@widgets/Sidebar/Sidebar';
import { useStore } from '@shared/store/store';
import { observer } from 'mobx-react-lite';
import { Loader, Paper, Title, Container, Stack, Button, TextInput } from '@mantine/core';
import { FileInput } from '@features/FileInput/FileInput';
import TestService from '@shared/services/TestService/TestService';

export const PersonalCabinetPage = observer(() => {


  const store = useStore();
  
  if (!store.initialized) {
    console.log(store.user)
    return (
      <div className={styles.loaderContainer}>
        <Loader size="xl" color="primary" />
      </div>
    );

  }

  const handleCreateTest = async () => {
    if (!store.file) {
      return;
    }

    await TestService.createTest(store.file, "test", "test");
  }
  
  return (
    <div className={styles.container}>
      <Sidebar />
      <Container size="lg" className={styles.content}>
        {store.user?.roles.includes("Teacher") && (
          <Paper shadow="none" p="xl" radius="md" withBorder className={styles.contentCard}>
            <Stack>
              <Title order={2} mb="md">Создать тест</Title>
                <Title order={3} mb="sm">Загрузите файл с конспектами</Title>
                <form onSubmit={handleCreateTest}>
                  <FileInput />
                  {store.file && (
                    <>
                      <TextInput mt="md" label="Название теста" placeholder="Название теста" />
                      <TextInput mt="md" label="Описание теста" placeholder="Описание теста" />
                      <Button 
                        color="var(--gradient-primary-secondary-light)" 
                        type="submit"
                        className={styles.createTestButton}
                        mt="md" 
                        style={{ color: 'black', borderRadius: '4px' }}
                        >
                          Создать тест
                      </Button>
                    </>
                  )}
                </form>
            </Stack>
          </Paper>
        )}

        {store.user?.roles.includes("Student") && (
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
