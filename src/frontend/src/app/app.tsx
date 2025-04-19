import { MantineProvider, createTheme } from '@mantine/core';
import { RoutesView } from '@pages';

const theme = createTheme({
  primaryColor: 'blue',
  fontFamily: 'Inter, sans-serif',
  defaultRadius: 'md',
});

export function App() {
  return (
    <MantineProvider theme={theme} defaultColorScheme="light">
      <RoutesView />
    </MantineProvider>
  );
}
