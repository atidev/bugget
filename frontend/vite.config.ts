import { defineConfig } from "vite";
import { resolve } from "path";
import react from "@vitejs/plugin-react-swc";
import tailwindcss from "@tailwindcss/vite";

// https://vite.dev/config/
export default defineConfig(() => {
  const basePath = process.env.VITE_BASE_PATH || "/";
  return {
    base: basePath,
    build: {
      target: "esnext",
      outDir: "dist",
    },
    resolve: {
      alias: {
        "@": resolve(__dirname, "./src"),
      },
      // Ensure only one instance of these packages in the bundle
      dedupe: [
        "react",
        "react-dom",
        "effector",
        "effector-react",
        "react-router-dom",
      ],
    },
    plugins: [react(), tailwindcss()],
    server: {
      port: 5173,
    },
  };
});
