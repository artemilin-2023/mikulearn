import { MantineProvider } from '@mantine/core';
import { useEffect } from 'react';

import { RouterProvider } from '@app/providers';
// import { SignalrProvider } from '@app/providers/Signalr.provider';
import { theme } from '@app/styles/app-theme';
import { StoreContext, store } from '@shared/store/store.js';
import '@app/styles/global.css';

export function App() {
  useEffect(() => {
    store.loadTokenFromStorage();
    
    const savedTheme = localStorage.getItem('mantine-color-scheme');
    if (savedTheme) {
      document.documentElement.setAttribute('data-mantine-color-scheme', savedTheme);
      if (savedTheme === 'dark') {
        document.documentElement.classList.add('dark-theme');
      } else {
        document.documentElement.classList.remove('dark-theme');
      }
    }
  }, []);

  const initialColorScheme = 
    typeof window !== 'undefined' 
      ? localStorage.getItem('mantine-color-scheme') as 'light' | 'dark' | null 
      : 'light';

  return (
    <StoreContext.Provider value={store}>
      <MantineProvider 
        theme={theme} 
        defaultColorScheme={initialColorScheme || 'light'}
      >
        {/* <SignalrProvider> */}
          <RouterProvider />
        {/* </SignalrProvider> */}
      </MantineProvider>
    </StoreContext.Provider>
  );
}
