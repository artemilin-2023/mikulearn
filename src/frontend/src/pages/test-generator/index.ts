import { createRouteView } from '@argon-router/react';
import { routes } from '@shared/router';
import { MainLayout } from '@shared/layouts';
import { TestGeneratorPage } from './test-generator';

export const TestGeneratorPageRoute = createRouteView({
  route: routes.testGenerator,
  view: TestGeneratorPage,
  layout: MainLayout,
});
