import {
  Button,
  Title,
  Stack,
  Paper,
  Text,
} from '@mantine/core';
import { FiLogOut, FiUser } from 'react-icons/fi';
import { useNavigate } from 'react-router-dom';
import { useEffect } from 'react';

import AuthService from '@shared/services/AuthService/AuthService';
import { SignalRService, useSignalR, type Message } from '@shared/services/SignalRService';

import styles from './Sidebar.module.css';  
import { TestGenerationStatus, TestGenerationResult } from '@shared/services/SignalRService/types';

import { observer } from 'mobx-react-lite';
import { useStore } from '@shared/store/store';

export const Sidebar = observer(() => {
  const navigate = useNavigate();
  const hubUrl = `${window.location.origin}/api/realtime/generation`;
  const { status, toggleConnection, onEvent } = useSignalR(hubUrl);

  const store = useStore();

  useEffect(() => {
    console.log("guid has been changed: ", store.test_guid)

    if (store.test_guid) {
      if (status.isConnected) {
        SignalRService.invokeMethod('SubscribeStatusUpdates', store.test_guid)
          .then(() => console.log('Subscribed to status updates for:', store.test_guid))
          .catch(error => console.error('Failed to subscribe to status updates:', error));
      }
      
      SignalRService.sendMessage({
        sender: 'client',
        content: store.test_guid,
      });
    }
  }, [store.test_guid, status.isConnected])

  useEffect(() => {
    if (status.isConnected) {
      onEvent('RecieveTestGenerationStatus', (status: TestGenerationStatus) => {
        console.log('Получено сообщение:', status);
      });

      onEvent('RecieveTestGenerationResult', (result: TestGenerationResult) => {
        console.log('Получено сообщение:', result);
      });
      
      onEvent('ReceiveMessage', (message: Message) => {
        console.log('Получено сообщение:', message);
      });
      
    }
  }, [status.isConnected, onEvent]);

  const handleLogout = async () => {
    await AuthService.logout();
    navigate('/sign-in');
  };

  return (
    <Paper shadow="sm" radius="md" p="lg" withBorder className={styles.sidebar}>
      <Stack gap="lg">
        <Title order={2} className={styles.sidebarTitle}>
          <FiUser className={styles.titleIcon} /> Личный кабинет
        </Title>
                
        {/* <div className={styles.spacer} /> */}
                
        <Button 
          onClick={toggleConnection}
          color={status.isConnected ? "red" : "blue"}
        >
          {status.isConnected ? "Отключиться" : "Подключиться"}
        </Button>
        
        {status.isConnected && (
          <Text size="sm" c="green">Соединение установлено</Text>
        )}
        
        {status.lastError && (
          <Text size="sm" c="red">Ошибка: {status.lastError}</Text>
        )}
                
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
}) ;
