import type { AppExtension } from "./extension";
import { hostApi } from "./hostApi";

/**
 * Wait for SDK to initialize (with timeout)
 */
async function waitForSDK(timeoutMs = 2000): Promise<boolean> {
  const startTime = Date.now();

  while (Date.now() - startTime < timeoutMs) {
    if ((window as any).__SHARED__ && (window as any).__initExtensions) {
      return true;
    }
    await new Promise((resolve) => setTimeout(resolve, 50));
  }

  return false;
}

/**
 * Load extensions if available.
 *
 * Extensions are loaded via window.__SHARED__ which is initialized by the host SDK.
 * This function doesn't import the SDK directly - it only checks if the SDK
 * has already been loaded and initialized the global __SHARED__ object.
 *
 * This approach keeps bugget/frontend completely independent from SDK packages.
 */
export async function loadExtensions(): Promise<AppExtension[]> {
  // Check if extensions are configured
  const extensionsConfig =
    (typeof window !== "undefined" && window.env?.VITE_APP_EXTENSIONS) ||
    import.meta.env.VITE_APP_EXTENSIONS;

  if (!extensionsConfig || extensionsConfig === "") {
    return [];
  }

  // Wait for SDK to initialize (libraries are exported, now waiting for host-sdk.js)
  const sdkReady = await waitForSDK();

  if (!sdkReady) {
    console.log(
      "[extensions] SDK initialization timeout. Running in standalone mode."
    );
    return [];
  }

  try {
    // SDK is ready - call the global loader function
    return (window as any).__initExtensions(hostApi, {
      debug: import.meta.env.DEV,
    });
  } catch (error) {
    console.error("[extensions] Failed to initialize extensions:", error);
    return [];
  }
}
