import { MantineProvider } from '@mantine/core';
import { RoutesView } from '@pages';
import { useEffect } from 'react';
import { loadTokenFromStorage } from '@shared/user';

import { theme } from "@app/styles/app-theme"

export function App() {
  useEffect(() => {
    loadTokenFromStorage();
  }, []);

  return (
    <MantineProvider theme={theme} defaultColorScheme="light">
      <RoutesView />
    </MantineProvider>
  );
}
