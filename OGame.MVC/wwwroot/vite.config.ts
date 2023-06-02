import { defineConfig } from 'vite'
import preact from '@preact/preset-vite';

// https://vitejs.dev/config/
export default defineConfig({
  base: '/dist/',
  plugins: [
    preact({ devtoolsInProd: true })
  ],
  server: {
    port: 3000,
    strictPort: true,
    cors: false,
  },
  build: {
    sourcemap: true,
    rollupOptions: {
      output: {
        entryFileNames: 'assets/[name].js',
        chunkFileNames: 'assets/[name].js',
        assetFileNames: 'assets/[name].[ext]',
      },
    },
  },
});
