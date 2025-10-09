import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import "./global.css";
import { loadEnvScript } from "@/utils/envLoader";

// инициализируем приложение после загрузки env.js
loadEnvScript().then(async () => {
  // Откладываем импорт App до тех пор, пока runtime env не станет доступным
  const App = (await import("./App.tsx")).default;
  createRoot(document.getElementById("root")!).render(
    <StrictMode>
      <App />
    </StrictMode>
  );
});
