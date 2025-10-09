import type { AppExtension } from "./extension";
import { hostApi } from "./hostApi";

export async function loadExtensions(): Promise<AppExtension[]> {
  // Check if extensions are configured at runtime
  const extensionsConfig =
    (typeof window !== "undefined" && window.env?.VITE_APP_EXTENSIONS) ||
    import.meta.env.VITE_APP_EXTENSIONS;

  if (!extensionsConfig || extensionsConfig === "") {
    return [];
  }

  try {
    // Normal dynamic imports - Vite will:
    // - In standalone: resolve to stub via alias
    // - In Docker: resolve to actual SDK from node_modules
    // @ts-ignore - Resolved via alias in standalone, from node_modules in Docker
    const { initShared } = await import("@bugget/host-sdk/init");
    // @ts-ignore - Resolved via alias in standalone, from node_modules in Docker
    const { initExtensions } = await import("@bugget/host-sdk/loader");

    // Initialize shared dependencies
    initShared();

    // Load extensions
    return await initExtensions(hostApi, {
      debug: import.meta.env.DEV,
    });
  } catch (error) {
    console.error("[extensions] Failed to load SDK:", error);
    return [];
  }
}
