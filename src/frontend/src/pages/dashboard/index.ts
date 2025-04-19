import { createRouteView } from '@argon-router/react';
import { routes } from '@shared/router';
import { DashboardPage } from './dashboard-page';
import { MainLayout } from '@shared/layouts';

export const DashboardPageRoute = createRouteView({
  route: routes.dashboard,
  view: DashboardPage,
  layout: MainLayout,
});
