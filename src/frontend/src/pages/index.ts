import { createRoutesView } from '@argon-router/react';
import { HomePageRoute } from './home';
import { DashboardPageRoute } from './dashboard';
import { NotFoundPage } from './not-found';
import { SignInPageRoute } from './sign-in';
import { SignUpPageRoute } from './sign-up';

export const RoutesView = createRoutesView({
	routes: [HomePageRoute, DashboardPageRoute, SignInPageRoute, SignUpPageRoute],
	otherwise: NotFoundPage,
});
