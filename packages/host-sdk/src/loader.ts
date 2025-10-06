/**
 * Extension loader for Bugreport host applications
 */

import type { AppExtension, AppExtensionFactory, HostApi } from "./types";

/**
 * Configuration for extension loading
 */
export interface LoaderConfig {
  /**
   * Comma-separated list of extension URLs
   * Can be provided via config or will be read from window.env.VITE_APP_EXTENSIONS
   */
  extensions?: string;
  
  /**
   * Whether to log debug information
   * @default false
   */
  debug?: boolean;
}

/**
 * Load and initialize extensions
 * 
 * @param hostApi - API to provide to extensions
 * @param config - Optional loader configuration
 * @returns Promise resolving to array of loaded extensions
 * 
 * @example
 * ```ts
 * import "@bugreport/host-sdk/init"; // Side-effect: initialize __SHARED__
 * import { initExtensions } from "@bugreport/host-sdk/loader";
 * import { hostApi } from "./hostApi";
 * 
 * const extensions = await initExtensions(hostApi);
 * ```
 */
export async function initExtensions(
  hostApi: HostApi,
  config: LoaderConfig = {}
): Promise<AppExtension[]> {
  const debug = config.debug ?? false;

  // Get extensions list from config or environment
  const extensionsConfig =
    config.extensions ??
    window.env?.VITE_APP_EXTENSIONS ??
    import.meta.env.VITE_APP_EXTENSIONS ??
    "";

  const urls = extensionsConfig
    .split(",")
    .map((s: string) => s.trim())
    .filter(Boolean);

  if (debug) {
    console.log(`[host-sdk] Loading ${urls.length} extension(s):`, urls);
  }

  // Validate __SHARED__ is initialized
  if (!window.__SHARED__) {
    throw new Error(
      "[host-sdk] __SHARED__ is not initialized. " +
      "Make sure to import '@bugreport/host-sdk/init' before loading extensions."
    );
  }

  const result: AppExtension[] = [];

  for (const url of urls) {
    try {
      if (debug) {
        console.log(`[host-sdk] Loading extension from: ${url}`);
      }

      // Load extension via script tag (for IIFE format)
      await loadScriptExtension(url);

      // Extract extension name from URL (last segment before .js)
      const name = extractExtensionName(url);
      
      // IIFE modules expose via window.<name>
      const extensionGlobal = (window as any)[name];
      
      if (!extensionGlobal) {
        console.error(
          `[host-sdk] Extension "${url}" did not expose "${name}" global. ` +
          `Make sure the extension is built with correct name in vite.config.ts`
        );
        continue;
      }

      // Call factory function
      const maybe = extensionGlobal.default || extensionGlobal;
      const ext =
        typeof maybe === "function"
          ? (maybe as AppExtensionFactory)(hostApi)
          : maybe;

      const extensions = Array.isArray(ext) ? ext : [ext];
      result.push(...extensions);

      if (debug) {
        console.log(
          `[host-sdk] ✅ Loaded extension "${name}" (${extensions.length} extension(s))`
        );
      }
    } catch (error) {
      console.error(`[host-sdk] ❌ Failed to load "${url}":`, error);
    }
  }

  if (debug || result.length > 0) {
    console.log(
      `[host-sdk] ✅ Loaded ${result.length} extension(s) total:`,
      result.map((e) => e.id)
    );
  }

  return result;
}

/**
 * Load extension via script tag (for IIFE format)
 */
function loadScriptExtension(url: string): Promise<void> {
  return new Promise((resolve, reject) => {
    const script = document.createElement("script");
    script.src = url;
    script.onload = () => resolve();
    script.onerror = () => reject(new Error(`Failed to load script: ${url}`));
    document.head.appendChild(script);
  });
}

/**
 * Extract extension name from URL
 * Examples:
 *   http://localhost:5174/extension.js -> bugreportSaasExt (from window global)
 *   /app/ext/saas/extension.js -> bugreportSaasExt
 * 
 * Note: This is a heuristic. Extensions should use consistent naming.
 */
function extractExtensionName(url: string): string {
  // Common extension names mapping
  const nameMap: Record<string, string> = {
    "saas": "bugreportSaasExt",
    "seo": "bugreportSeoExt",
  };

  // Try to extract from path
  const match = url.match(/\/([\w-]+)\/extension\.js/);
  if (match && match[1]) {
    const key = match[1].toLowerCase();
    if (nameMap[key]) {
      return nameMap[key];
    }
  }

  // Fallback: try common default
  return "bugreportSaasExt";
}

