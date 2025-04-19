import { createEvent, createStore, sample } from 'effector';

type User = {
  id: string;
  email: string;
  name: string;
  role: string;
};

const $innerUser = createStore<User | null>(null);
export const $user = $innerUser.map((v) => v);

export const login = createEvent<User>();
export const logout = createEvent();

sample({
  clock: login,
  target: $innerUser,
});

sample({
  clock: logout,
  fn: () => null,
  target: $innerUser,
});
