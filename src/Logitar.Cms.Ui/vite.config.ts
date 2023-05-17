import { defineConfig } from 'vite';
import react from '@vitejs/plugin-react';
import tsconfigpaths from 'vite-tsconfig-paths';
import path from 'path';

// https://vitejs.dev/config/
export default defineConfig({
  build: {
    outDir: '../Logitar.Cms.Web/wwwroot/dist',
    emptyOutDir: true,
    rollupOptions: {
      output: {
        entryFileNames: `assets/[name].js`,
        chunkFileNames: `assets/[name].js`,
        assetFileNames: `assets/[name].[ext]`,
      },
    },
  },
  plugins: [react(), tsconfigpaths()],
  resolve: {
    alias: [
      { find: '@/', replacement: path.resolve(__dirname, 'src') },
      { find: '~/', replacement: path.resolve(__dirname, 'src/modules') },
    ],
  },
  server: {
    port: 3000,
  },
});
