import { createRoutesView } from '@argon-router/react';
import { HomePageRoute } from './home';
import { DashboardPageRoute } from './dashboard';
import { NotFoundPage } from './not-found';

export const RoutesView = createRoutesView({
  routes: [HomePageRoute, DashboardPageRoute],
  otherwise: NotFoundPage,
});
