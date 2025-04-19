import { MantineProvider } from '@mantine/core';
import { RoutesView } from '@pages';
import { useEffect } from 'react';
import { loadTokenFromStorage } from '@shared/user';
import { StoreContext, store } from "@shared/store/store.js";

import { theme } from "@app/styles/app-theme"

export function App() {
  useEffect(() => {
    loadTokenFromStorage();
  }, []);

  return (
    <StoreContext.Provider value={store}>
      <MantineProvider theme={theme} defaultColorScheme="light">
        <RoutesView />
      </MantineProvider>
    </StoreContext.Provider>
  );
}
