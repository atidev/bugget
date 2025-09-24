import { AppExtension, AppExtensionFactory } from "./extension";
import { hostApi } from "./hostApi";

// загрузчик плагинов из переменной окружения
export async function loadExtensions(): Promise<AppExtension[]> {
  // "@app/sample-pages,@app/experiments"
  const names = (import.meta.env.VITE_APP_EXTENSIONS ?? "")
    .split(",")
    .map((s: string) => s.trim())
    .filter(Boolean);

  const result: AppExtension[] = [];
  for (const name of names) {
    try {
      const mod = await import(/* @vite-ignore */ name);
      const maybe = mod.default;
      const ext =
        typeof maybe === "function"
          ? (maybe as AppExtensionFactory)(hostApi)
          : maybe;
      result.push(...(Array.isArray(ext) ? ext : [ext]));
    } catch (e) {
      console.error(`[extensions] Failed to load "${name}"`, e);
    }
  }
  return result;
}
