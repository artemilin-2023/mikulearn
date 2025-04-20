export interface Message {
  sender: string;
  content: string;
  timestamp: string;
}

export interface ConnectionStatus {
  isConnected: boolean;
  lastError?: string;
}

export interface TestGenerationStatus {
  status: unknown;
  message: unknown;

}

export interface TestGenerationResult {
  result: unknown;
  message: unknown;
}

export interface SignalREvents {
  ReceiveMessage: (message: Message) => void;
  UserJoined: (username: string) => void;
  UserLeft: (username: string) => void;
  RecieveTestGenerationStatus: (status: TestGenerationStatus) => void;
  RecieveTestGenerationResult: (result: TestGenerationResult) => void;
} 