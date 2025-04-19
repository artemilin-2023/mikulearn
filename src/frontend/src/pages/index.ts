import { createRoutesView } from '@argon-router/react';
import { HomePageRoute } from './home';
import { NotFoundPage } from './not-found/not-found-page';

export const RoutesView = createRoutesView({
  routes: [HomePageRoute],
  otherwise: NotFoundPage,
});
