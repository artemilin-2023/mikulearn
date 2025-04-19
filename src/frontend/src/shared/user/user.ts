import { createEvent, createStore, sample } from 'effector';

type User = {
  id: string;
  email: string;
  name: string;
  role: string;
};

const $innerUser = createStore<User | null>(null);
export const $user = $innerUser.map((v) => v);

export const $isAuth = $innerUser.map(Boolean);
export const $token = createStore<string | null>(null);

export const login = createEvent<User>();
export const setToken = createEvent<string>();
export const logout = createEvent();

sample({
  clock: login,
  target: $innerUser,
});

sample({
  clock: setToken,
  target: $token,
});

sample({
  clock: logout,
  fn: () => null,
  target: [$innerUser, $token],
});

export const loadTokenFromStorage = createEvent();

sample({
  clock: loadTokenFromStorage,
  fn: () => {
    const token = localStorage.getItem('token');
    return token;
  },
  target: $token,
});

sample({
  clock: $token,
  filter: (token) => token !== null,
  fn: (token) => {
    if (token) {
      localStorage.setItem('token', token);
    }
    return token;
  },
});

sample({
  clock: logout,
  fn: () => {
    localStorage.removeItem('token');
  },
});
