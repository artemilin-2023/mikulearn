import { createRoot } from 'react-dom/client';
import { allSettled, fork } from 'effector';
import { router } from '@shared/router';
import { createBrowserHistory } from 'history';
import { Provider } from 'effector-react';
import { App } from '@app';

// global styles
import '@app/styles/layout.css';
import '@app/styles/colors.css';
import '@app/styles/reset.css';
import '@app/styles/transitions.css';

const root = createRoot(document.getElementById('root')!);

async function render() {
  const scope = fork();

  await allSettled(router.setHistory, {
    scope,
    params: createBrowserHistory(),
  });

  root.render(
    <Provider value={scope}>
      <App />
    </Provider>
  );
}

render();
