import { defineConfig } from "vite";
import react from "@vitejs/plugin-react-swc";
import tailwindcss from "@tailwindcss/vite";

// https://vite.dev/config/
export default defineConfig(() => {
  return {
    build: {
      target: "esnext",
      outDir: "build",
    },
    plugins: [react(), tailwindcss()],
    server: {
      port: 5173,
    },
  };
});
