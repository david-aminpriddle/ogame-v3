import { defineConfig } from 'vite'
import preact from '@preact/preset-vite';

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [
    preact({ devtoolsInProd: true, prefreshEnabled: true })
  ],
  server: {
    port: 3000,
    strictPort: true,
    https: false,
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
