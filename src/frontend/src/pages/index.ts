import { createRoutesView } from '@argon-router/react';
import { HomePageRoute } from './home';
import { DashboardPageRoute } from './dashboard';
import { NotFoundPage } from './not-found';
import { TestGeneratorPageRoute } from './test-generator';

export const RoutesView = createRoutesView({
  routes: [HomePageRoute, DashboardPageRoute, TestGeneratorPageRoute],
  otherwise: NotFoundPage,
});
