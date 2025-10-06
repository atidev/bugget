// Extension loader now uses @bugreport/host-sdk
import "@bugreport/host-sdk/init"; // Side-effect: initialize window.__SHARED__
import { initExtensions } from "@bugreport/host-sdk/loader";
import type { AppExtension } from "@bugreport/host-sdk/types";
import { hostApi } from "./hostApi";

/**
 * Load extensions using the host SDK
 * All the magic (shared deps, loading, etc.) is handled by @bugreport/host-sdk
 */
export async function loadExtensions(): Promise<AppExtension[]> {
  return initExtensions(hostApi, {
    debug: import.meta.env.DEV, // Enable debug logs in development
  });
}
