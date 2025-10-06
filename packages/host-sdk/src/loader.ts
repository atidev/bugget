import type { AppExtension, AppExtensionFactory, HostApi } from "./types";

export interface LoaderConfig {
  debug?: boolean;
}

export async function initExtensions(
  hostApi: HostApi,
  config: LoaderConfig = {}
): Promise<AppExtension[]> {
  const debug = config.debug ?? false;

  const extensionsConfig =
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

      // Get extension name from config or try to auto-detect
      const name = extractExtensionNameFromUrl(url);
      
      const extensionGlobal = (window as any)[name];
      
      if (!extensionGlobal) {
        console.error(
          `[host-sdk] Extension "${url}" did not expose "${name}" global. ` +
          `Expected window.${name} to be defined. ` +
          `Provide explicit mapping via config.extensionNames if needed.`
        );
        continue;
      }

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

function loadScriptExtension(url: string): Promise<void> {
  return new Promise((resolve, reject) => {
    const script = document.createElement("script");
    script.src = url;
    script.onload = () => resolve();
    script.onerror = () => reject(new Error(`Failed to load script: ${url}`));
    document.head.appendChild(script);
  });
}

function extractExtensionNameFromUrl(url: string): string {
  const match = url.match(/\/([\w-]+)\/extension\.js/);
  if (match && match[1]) {
    const dirname = match[1];
    const camelCase = dirname
      .split('-')
      .map((word, idx) => 
        idx === 0 ? word : word.charAt(0).toUpperCase() + word.slice(1)
      )
      .join('');
    return `bugreport${camelCase.charAt(0).toUpperCase() + camelCase.slice(1)}Ext`;
  }

  console.warn(
    `[host-sdk] Could not extract extension name from URL "${url}". ` +
    `Using fallback "bugreportExt". Consider providing explicit extensionNames config.`
  );
  return "bugreportExt";
}

