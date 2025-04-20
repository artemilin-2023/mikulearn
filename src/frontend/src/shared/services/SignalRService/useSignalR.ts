import { useState, useEffect, useCallback } from 'react';
import SignalRService from './SignalRService';
import { ConnectionStatus, SignalREvents } from './types';

export const useSignalR = (hubUrl: string) => {
  const [status, setStatus] = useState<ConnectionStatus>({
    isConnected: SignalRService.isConnected(),
  });

  const connect = useCallback(async () => {
    console.log('connect');

    try {
      if (!status.isConnected) {
        await SignalRService.startConnection(hubUrl);
        setStatus({ isConnected: true });
      }
    } catch (error) {
      setStatus({ 
        isConnected: false, 
        lastError: error instanceof Error ? error.message : String(error) 
      });
    }
  }, [hubUrl, status.isConnected]);

  const disconnect = useCallback(async () => {
    try {
      await SignalRService.stopConnection();
      setStatus({ isConnected: false });
    } catch (error) {
      setStatus({ 
        isConnected: SignalRService.isConnected(), 
        lastError: error instanceof Error ? error.message : String(error) 
      });
    }
  }, []);

  const toggleConnection = useCallback(async () => {
    if (status.isConnected) {
      await disconnect();
    } else {
      await connect();
    }
  }, [status.isConnected, connect, disconnect]);

  const onEvent = useCallback(<K extends keyof SignalREvents>(
    eventName: K, 
    callback: SignalREvents[K]
  ) => {
    SignalRService.onEvent(eventName, callback);
  }, []);

  const offEvent = useCallback(<K extends keyof SignalREvents>(
    eventName: K, 
    callback?: SignalREvents[K]
  ) => {
    SignalRService.offEvent(eventName, callback);
  }, []);

  const sendMessage = useCallback((sender: string, content: string) => {
    return SignalRService.sendMessage({ sender, content });
  }, []);

  const invokeMethod = useCallback((methodName: string, ...args: any[]) => {
    return SignalRService.invokeMethod(methodName, ...args);
  }, []);

  useEffect(() => {
    return () => {
      // При размонтировании компонента не отключаемся автоматически,
      // так как соединение может использоваться другими компонентами
    };
  }, []);

  return {
    status,
    connect,
    disconnect,
    toggleConnection,
    onEvent,
    offEvent,
    sendMessage,
    invokeMethod,
  };
}; 