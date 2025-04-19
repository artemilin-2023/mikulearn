import { RouterProvider } from '@argon-router/react';
import { RoutesView } from '@pages';
import { router } from '@shared/router';

export function App() {
  return (
    <RouterProvider router={router}>
      <RoutesView />
    </RouterProvider>
  );
}
