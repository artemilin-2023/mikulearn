import { createRouteView } from '@argon-router/react';
import { SignInPage } from './sign-in-page';
import { MainLayout } from '@shared/layouts';
import { routes } from '@shared/router';

export const SignInPageRoute = createRouteView({
	view: SignInPage,
	layout: MainLayout,
	route: routes.signIn,
});
