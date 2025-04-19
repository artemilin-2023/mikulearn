import { createRouteView } from '@argon-router/react';
import { HomePage } from './home-page';
import { MainLayout } from '@shared/layouts';
import { routes } from '@shared/router';

export const HomePageRoute = createRouteView({
  view: HomePage,
  layout: MainLayout,
  route: routes.home,
});
