import { createRouteView } from '@argon-router/react';
import { PersonalCabinetPage } from './personal-cabinet-page';
import { MainLayout } from '@shared/layouts';
import { routes } from '@shared/router';

export const PersonalCabinetPageRoute = createRouteView({
	view: PersonalCabinetPage,
	layout: MainLayout,
	route: routes.personalCabinet,
});
