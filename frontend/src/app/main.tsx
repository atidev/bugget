import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./global.css";
import App from "./App.tsx";
import { loadEnvScript } from "@/utils/envLoader";

// Initialize app after env.js is loaded
loadEnvScript().then(async () => {
  // Export libraries for extensions support BEFORE rendering
  // (only if building with VITE_APP_EXTENSIONS)
  if (import.meta.env.VITE_APP_EXTENSIONS) {
    await import("@/extensions-support");
    console.log("[main] Extensions support loaded");
  }

  createRoot(document.getElementById("root")!).render(
    <StrictMode>
      <App />
    </StrictMode>
  );
});
