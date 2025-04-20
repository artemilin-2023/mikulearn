import { createRoot } from 'react-dom/client';
import { App } from '@app';

// global styles
import '@mantine/core/styles.css';
import '@app/styles/layout.css';
import '@app/styles/colors.css';
import '@app/styles/reset.css';
import '@app/styles/transitions.css';

const root = createRoot(document.getElementById('root')!);

async function render() {
  root.render(
    <App />
  );
}

render();
