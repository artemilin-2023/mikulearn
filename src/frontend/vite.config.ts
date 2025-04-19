import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react-swc'
import path from 'path'

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  resolve: {
    alias: {
      'app': path.resolve(__dirname, './src/app'),
      'pages': path.resolve(__dirname, './src/pages'),
      'features': path.resolve(__dirname, './src/features'),
      'widgets': path.resolve(__dirname, './src/widgets'),
      'shared': path.resolve(__dirname, './src/shared'),
      'entities': path.resolve(__dirname, './src/entities'),
    },
  },
})
