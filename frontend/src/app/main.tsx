import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./global.css";
import App from "./App.tsx";

// Load env.js dynamically using the base URL from Vite config
function loadEnvScript(): Promise<void> {
  return new Promise((resolve) => {
    const script = document.createElement("script");
    const basePath = import.meta.env.BASE_URL || "/";
    // Ensure proper path concatenation
    const envPath = basePath.endsWith("/")
      ? `${basePath}env.js`
      : `${basePath}/env.js`;
    script.src = envPath;
    script.onload = () => resolve();
    script.onerror = () => {
      console.warn("Failed to load env.js, using defaults");
      resolve(); // Don't block app initialization
    };
    document.head.appendChild(script);
  });
}

// Initialize app after env.js is loaded
loadEnvScript().then(() => {
  createRoot(document.getElementById("root")!).render(
    <StrictMode>
      <App />
    </StrictMode>
  );
});
