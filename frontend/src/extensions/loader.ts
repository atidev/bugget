import type { AppExtension } from "./extension";
import { hostApi } from "./hostApi";

export async function loadExtensions(): Promise<AppExtension[]> {
  // Check if extensions are configured
  const extensionsConfig =
    (typeof window !== "undefined" && window.env?.VITE_APP_EXTENSIONS) ||
    import.meta.env.VITE_APP_EXTENSIONS;

  if (!extensionsConfig || extensionsConfig === "") {
    return [];
  }

  try {
    // Dynamically import SDK via computed specifiers to avoid TS/Vite resolution when disabled
    const sdkBase = "@bugget/host-sdk";
    const { initShared } = await import(/* @vite-ignore */ `${sdkBase}/init`);
    const { initExtensions } = await import(
      /* @vite-ignore */ `${sdkBase}/loader`
    );

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
