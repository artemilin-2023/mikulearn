import { createRouteView } from '@argon-router/react';
import { SignUpPage } from './sign-up-page';
import { MainLayout } from '@shared/layouts';
import { routes } from '@shared/router';

export const SignUpPageRoute = createRouteView({
	view: SignUpPage,
	layout: MainLayout,
	route: routes.signUp,
});
