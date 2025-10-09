import { defineConfig } from "vite";
import { resolve } from "path";
import react from "@vitejs/plugin-react-swc";
import tailwindcss from "@tailwindcss/vite";

// https://vite.dev/config/
export default defineConfig(() => {
  const basePath = process.env.VITE_BASE_PATH || "/";
  const hasExtensions = !!process.env.VITE_APP_EXTENSIONS;

  // In standalone builds (no VITE_APP_EXTENSIONS), alias SDK to stub
  const sdkAlias = hasExtensions
    ? {}
    : {
        "@bugget/host-sdk/init": resolve(
          __dirname,
          "./src/extensions/sdk-stub.ts"
        ),
        "@bugget/host-sdk/loader": resolve(
          __dirname,
          "./src/extensions/sdk-stub.ts"
        ),
      };

  return {
    base: basePath,
    build: {
      target: "esnext",
      outDir: "dist",
    },
    resolve: {
      alias: {
        "@": resolve(__dirname, "./src"),
        ...sdkAlias,
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
