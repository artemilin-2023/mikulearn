import { HubConnection, HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { Message, SignalREvents } from './types';

class SignalRService {
  private connection: HubConnection | null = null;
  
  async startConnection(url: string) {
    if (this.connection) {
      console.log('Connection already established');
      return this.connection;
    }
    
    try {
      this.connection = new HubConnectionBuilder()
        .withUrl(url)
        .configureLogging(LogLevel.Information)
        .withAutomaticReconnect()
        .build();
      
      await this.connection.start();
      console.log('SignalR connection established');
      
      return this.connection;
    } catch (error) {
      console.error('Error establishing SignalR connection:', error);
      this.connection = null;
      throw error;
    }
  }
  
  async stopConnection() {
    if (this.connection) {
      try {
        await this.connection.stop();
        console.log('SignalR connection stopped');
        this.connection = null;
      } catch (error) {
        console.error('Error stopping SignalR connection:', error);
        throw error;
      }
    }
  }
  
  getConnection() {
    return this.connection;
  }
  
  isConnected() {
    return this.connection !== null && this.connection.state === 'Connected';
  }
  
  onEvent<K extends keyof SignalREvents>(eventName: K, callback: SignalREvents[K]) {
    if (this.connection) {
      this.connection.on(eventName as string, callback as (...args: any[]) => void);
    } else {
      console.error('Cannot subscribe to event, connection not established');
    }
  }
  
  offEvent<K extends keyof SignalREvents>(eventName: K, callback?: SignalREvents[K]) {
    if (this.connection) {
      if (callback) {
        this.connection.off(eventName as string, callback as (...args: any[]) => void);
      } else {
        this.connection.off(eventName as string);
      }
    }
  }
  
  async sendMessage(message: Omit<Message, 'timestamp'>) {
    return this.invokeMethod('SendMessage', message);
  }
  
  async invokeMethod(methodName: string, ...args: any[]) {
    if (!this.connection) {
      throw new Error('Connection not established');
    }
    
    try {
      return await this.connection.invoke(methodName, ...args);
    } catch (error) {
      console.error(`Error invoking method ${methodName}:`, error);
      throw error;
    }
  }
}

export default new SignalRService(); 