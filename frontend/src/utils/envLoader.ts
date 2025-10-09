/**
 * Динамическая загрузка env.js с учетом BASE_URL из Vite-конфига.
 * Загружает внешний скрипт конфигурации окружения перед инициализацией приложения.
 * При ошибке загрузки не блокирует запуск приложения.
 *
 * @returns Promise, который резолвится после загрузки скрипта или при ошибке
 */
export async function loadEnvScript(): Promise<void> {
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
