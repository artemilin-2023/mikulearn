import { MantineProvider } from '@mantine/core';
import { RoutesView } from '@pages';

export function App() {
  return (
    <MantineProvider>
      <RoutesView />
    </MantineProvider>
  );
}
