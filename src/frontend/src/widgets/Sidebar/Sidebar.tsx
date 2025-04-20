import {
  Button,
  Title,
  Stack,
  Paper,
  Text,
} from '@mantine/core';
import { FiLogOut, FiUser } from 'react-icons/fi';
import { useNavigate } from 'react-router-dom';
import { useEffect, useState } from 'react';

import AuthService from '@shared/services/AuthService/AuthService';
import { SignalRService, useSignalR, type Message } from '@shared/services/SignalRService';

import styles from './Sidebar.module.css';  
import { TestGenerationStatus, TestGenerationResult } from '@shared/services/SignalRService/types';

import { observer } from 'mobx-react-lite';
import { useStore } from '@shared/store/store';

export const Sidebar = observer(() => {
  const navigate = useNavigate();
  const hubUrl = `${window.location.origin}/api/realtime/generation`;
  const { status, connect, disconnect, onEvent } = useSignalR(hubUrl);
  const [connectionAttempted, setConnectionAttempted] = useState(false);

  const store = useStore();
  const isTeacher = store.user?.roles?.includes('Teacher');

  // Auto-connect to SignalR when the component loads if user is a Teacher
  useEffect(() => {
    // Only try to connect once after user data is loaded
    if (isTeacher && !connectionAttempted && !status.isConnected && store.user) {
      console.log('Attempting to connect automatically for Teacher role');
      connect().then(() => {
        console.log('SignalR connection established automatically for Teacher');
        setConnectionAttempted(true);
      }).catch(error => {
        console.error('Failed to auto-connect to SignalR:', error);
        setConnectionAttempted(true);
      });
    }
  }, [store.user, status.isConnected, connect, isTeacher, connectionAttempted]);

  // Handle guid changes
  useEffect(() => {
    console.log("guid has been changed: ", store.test_guid);

    if (store.test_guid && status.isConnected) {
      SignalRService.invokeMethod('SubscribeStatusUpdates', store.test_guid)
        .then(() => console.log('Subscribed to status updates for:', store.test_guid))
        .catch(error => console.error('Failed to subscribe to status updates:', error));
    }
  }, [store.test_guid, status.isConnected]);

  // Set up event handlers
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
    // Disconnect from SignalR when logging out
    if (status.isConnected) {
      await SignalRService.stopConnection();
    }
    await AuthService.logout();
    navigate('/sign-in');
  };

  // Connection toggle handler
  const handleConnectionToggle = async () => {
    if (status.isConnected) {
      await disconnect();
    } else {
      await connect();
    }
  };

  return (
    <Paper shadow="sm" radius="md" p="lg" withBorder className={styles.sidebar}>
      <Stack gap="lg">
        <Title order={2} className={styles.sidebarTitle}>
          <FiUser className={styles.titleIcon} /> Личный кабинет
        </Title>
        
        {/* {isTeacher && (
          <>
            <Button 
              onClick={handleConnectionToggle}
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
          </>
        )} */}
        
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
