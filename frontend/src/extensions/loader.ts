import { AppExtension } from "./extension";

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
      const ext = mod.default as AppExtension | AppExtension[];
      result.push(...(Array.isArray(ext) ? ext : [ext]));
    } catch (e) {
      console.error(`[extensions] Failed to load "${name}"`, e);
    }
  }
  return result;
}
