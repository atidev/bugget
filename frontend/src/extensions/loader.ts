import type { AppExtension } from "./extension";
import { hostApi } from "./hostApi";

export async function loadExtensions(): Promise<AppExtension[]> {
  const extensionsConfig =
    (typeof window !== "undefined" && window.env?.VITE_APP_EXTENSIONS) ||
    import.meta.env.VITE_APP_EXTENSIONS;

  if (!extensionsConfig || extensionsConfig === "") {
    return [];
  }

  // Conditional import: only load SDK module if it was built with extensions support
  if (import.meta.env.VITE_APP_EXTENSIONS) {
    try {
      const { loadExtensionsWithSdk } = await import("./loader-with-sdk");
      return loadExtensionsWithSdk(hostApi, import.meta.env.DEV);
    } catch (error) {
      console.error("[extensions] Failed to load SDK:", error);
      return [];
    }
  }

  // Standalone mode - extensions configured but SDK not available
  console.warn(
    "[extensions] Extensions configured but SDK not available. Running in standalone mode."
  );
  return [];
}
