import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tsconfigpaths from 'vite-tsconfig-paths';
import path from 'path';

// https://vitejs.dev/config/
export default defineConfig({
  define: {
    // Set the value of NODE_ENV to "production" when building
    'process.env.NODE_ENV': JSON.stringify(process.env.NODE_ENV || 'development'),
  },
  plugins: [react(), tsconfigpaths()],
  resolve: {
    alias: [
      { find: '~api', replacement: path.resolve(__dirname, 'api') },
      { find: '~components', replacement: path.resolve(__dirname, 'components') },
      { find: '~hooks', replacement: path.resolve(__dirname, 'hooks') },
      { find: '~locales', replacement: path.resolve(__dirname, 'locales') },
      { find: '~models', replacement: path.resolve(__dirname, 'models') },
      { find: '~pages', replacement: path.resolve(__dirname, 'pages') },
      { find: '~themes', replacement: path.resolve(__dirname, 'themes') },
    ],
  },
});
