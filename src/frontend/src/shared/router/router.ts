import { createRoute, createRouter } from '@argon-router/core';

export const routes = {
  home: createRoute({ path: '/' }),
  dashboard: createRoute({ path: '/dashboard' }),
  about: createRoute({ path: '/about' }),
  signIn: createRoute({ path: '/sign-in' }),
  signUp: createRoute({ path: '/sign-up' }),
  course: createRoute({ path: '/courses/:name' }),
  testGenerator: createRoute({ path: '/test-generator' })
};

export const router = createRouter({
  routes: [
    routes.home,
    routes.about,
    routes.dashboard,
    routes.signIn,
    routes.signUp,
    routes.course,
    routes.testGenerator,
  ],
});
