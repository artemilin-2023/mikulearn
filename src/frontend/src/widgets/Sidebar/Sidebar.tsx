import {
  Button,
  Title,
  Stack,
  Paper,
} from '@mantine/core';
import { FiLogOut, FiUser } from 'react-icons/fi';
import { useNavigate } from 'react-router-dom';

import AuthService from '@shared/services/AuthService/AuthService';

import styles from './Sidebar.module.css';  

export const Sidebar = () => {
  const navigate = useNavigate();

  const handleLogout = async () => {
    await AuthService.logout();
    navigate('/sign-in');
  };

  const setupConnection = () => {

  }

  return (
    <Paper shadow="sm" radius="md" p="lg" withBorder className={styles.sidebar}>
      <Stack gap="lg">
        <Title order={2} className={styles.sidebarTitle}>
          <FiUser className={styles.titleIcon} /> Личный кабинет
        </Title>
                
        {/* <div className={styles.spacer} /> */}
                
        <Button onClick={setupConnection}>
            Подключиться
        </Button>
                
        <Button 
          variant="outline" 
          color="red" 
          onClick={handleLogout} 
          leftSection={<FiLogOut size="1rem" />}
          fullWidth
        >
            Выйти
        </Button>
      </Stack>
    </Paper>
  );
};
