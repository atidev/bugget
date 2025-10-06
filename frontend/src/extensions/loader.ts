import type { AppExtension } from "./extension";
import { hostApi } from "./hostApi";

/**
 * Load extensions if configured.
 *
 * This function conditionally imports @bugget/host-sdk only when VITE_APP_EXTENSIONS
 * is set. In standalone builds, the SDK won't be bundled at all.
 */
export async function loadExtensions(): Promise<AppExtension[]> {
  // Check if extensions are configured
  const extensionsConfig =
    (typeof window !== "undefined" && window.env?.VITE_APP_EXTENSIONS) ||
    import.meta.env.VITE_APP_EXTENSIONS;

  if (!extensionsConfig || extensionsConfig === "") {
    return [];
  }

  try {
    // Dynamically import SDK - it will only be bundled if VITE_APP_EXTENSIONS is set at build time
    const { initShared } = await import("@bugget/host-sdk/init");
    const { initExtensions } = await import("@bugget/host-sdk/loader");

    // Initialize shared dependencies (React, Effector, etc.)
    initShared();

    // Load and initialize extensions
    return await initExtensions(hostApi, {
      debug: import.meta.env.DEV,
    });
  } catch (error) {
    console.error("[extensions] Failed to load SDK:", error);
    return [];
  }
}
