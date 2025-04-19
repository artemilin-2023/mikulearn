import { MantineProvider } from '@mantine/core';
import { RoutesView } from '@pages';

import { theme } from "@app/styles/app-theme"

export function App() {
  return (
    <MantineProvider theme={theme} defaultColorScheme="light">
      <RoutesView />
    </MantineProvider>
  );
}
